using CollAction.Data;
using CollAction.GraphQl.Mutations.Input;
using CollAction.Helpers;
using CollAction.Models;
using CollAction.Services.Crowdactions.Models;
using CollAction.Services.Email;
using CollAction.Services.HtmlValidator;
using CollAction.ViewModels.Email;
using Hangfire;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace CollAction.Services.Crowdactions
{
    public sealed class CrowdactionService : ICrowdactionService
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ApplicationDbContext context;
        private readonly IEmailSender emailSender;
        private readonly ILogger<CrowdactionService> logger;
        private readonly SiteOptions siteOptions;
        private readonly IHtmlInputValidator htmlInputValidator;
        private readonly IServiceProvider serviceProvider;
        private readonly IBackgroundJobClient jobClient;
        private readonly IRecurringJobManager recurringJobManager;
        private const int MaxNumberCrowdactionEmails = 4;
        private static readonly TimeSpan TimeEmailAllowedAfterCrowdactionEnd = TimeSpan.FromDays(180);

        public CrowdactionService(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context,
            IEmailSender emailSender,
            ILogger<CrowdactionService> logger,
            IOptions<SiteOptions> siteOptions,
            IHtmlInputValidator htmlInputValidator,
            IServiceProvider serviceProvider,
            IBackgroundJobClient jobClient,
            IRecurringJobManager recurringJobManager)
        {
            this.userManager = userManager;
            this.context = context;
            this.emailSender = emailSender;
            this.logger = logger;
            this.siteOptions = siteOptions.Value;
            this.htmlInputValidator = htmlInputValidator;
            this.serviceProvider = serviceProvider;
            this.jobClient = jobClient;
            this.recurringJobManager = recurringJobManager;
        }

        public async Task<Crowdaction> CreateCrowdactionInternal(NewCrowdactionInternal newCrowdaction, CancellationToken token)
        {
            if (await context.Crowdactions.AnyAsync(c => c.Name == newCrowdaction.Name).ConfigureAwait(false))
            {
                throw new InvalidOperationException($"A crowdaction with this name already exists: {newCrowdaction.Name}");
            }

            logger.LogInformation("Creating crowdaction: {0}", newCrowdaction.Name);
            var tagMap = new Dictionary<string, int>();
            List<string> tags = newCrowdaction.Tags.Distinct().ToList();
            if (tags.Any())
            {
                var missingTags = tags.Except(
                        await context.Tags
                                     .Where(t => tags.Contains(t.Name))
                                     .Select(t => t.Name)
                                     .ToListAsync(token).ConfigureAwait(false))
                    .Select(t => new Tag(t));

                if (missingTags.Any())
                {
                    context.Tags.AddRange(missingTags);
                    await context.SaveChangesAsync(token).ConfigureAwait(false);
                }

                tagMap = await context.Tags
                                      .Where(t => tags.Contains(t.Name))
                                      .ToDictionaryAsync(t => t.Name, t => t.Id, token).ConfigureAwait(false);
            }

            List<CrowdactionTag> crowdactionTags =
                tags.Select(t => new CrowdactionTag(tagId: tagMap[t]))
                    .ToList();

            var crowdaction = new Crowdaction(
                name: newCrowdaction.Name,
                status: newCrowdaction.Status,
                ownerId: newCrowdaction.OwnerId,
                target: newCrowdaction.Target,
                start: newCrowdaction.Start,
                end: newCrowdaction.End.Date.AddHours(23).AddMinutes(59).AddSeconds(59),
                description: newCrowdaction.Description,
                goal: newCrowdaction.Goal,
                proposal: newCrowdaction.Proposal,
                creatorComments: newCrowdaction.CreatorComments,
                descriptionVideoLink: newCrowdaction.DescriptionVideoLink?.Replace("www.youtube.com", "www.youtube-nocookie.com", StringComparison.Ordinal),
                displayPriority: newCrowdaction.DisplayPriority,
                anonymousUserParticipants: newCrowdaction.AnonymousUserParticipants,
                bannerImageFileId: newCrowdaction.BannerImageFileId,
                cardImageFileId: newCrowdaction.CardImageFileId,
                descriptiveImageFileId: newCrowdaction.DescriptiveImageFileId,
                categories: newCrowdaction.Categories.Select(c => new CrowdactionCategory((c))).ToList(),
                tags: crowdactionTags);

            context.Crowdactions.Add(crowdaction);
            await context.SaveChangesAsync(token).ConfigureAwait(false);
            await RefreshParticipantCount(token).ConfigureAwait(false);

            if (crowdaction.IsClosed)
            {
                crowdaction.FinishJobId = jobClient.Schedule(() => CrowdactionEndProcess(crowdaction.Id, CancellationToken.None), crowdaction.End);
                await context.SaveChangesAsync().ConfigureAwait(false);
            }

            return crowdaction;
        }

        public async Task<CrowdactionResult> CreateCrowdaction(NewCrowdaction newCrowdaction, ClaimsPrincipal user, CancellationToken token)
        {
            logger.LogInformation("Validating new crowdaction");

            IEnumerable<ValidationResult> validationResults = ValidationHelper.Validate(newCrowdaction, serviceProvider);
            if (validationResults.Any())
            {
                return new CrowdactionResult(validationResults);
            }

            ApplicationUser? owner = await userManager.GetUserAsync(user).ConfigureAwait(false);

            if (owner == null)
            {
                return new CrowdactionResult(new ValidationResult("Crowdaction owner could not be found"));
            }

            if (await context.Crowdactions.AnyAsync(c => c.Name == newCrowdaction.Name).ConfigureAwait(false))
            {
                return new CrowdactionResult(new ValidationResult("A crowdaction with this name already exists", new[] { nameof(Crowdaction.Name) }));
            }

            logger.LogInformation("Creating crowdaction: {0}", newCrowdaction.Name);
            var tagMap = new Dictionary<string, int>();
            List<string> tags = newCrowdaction.Tags.Distinct().ToList();
            if (tags.Any())
            {
                var missingTags = tags.Except(
                        await context.Tags
                                     .Where(t => tags.Contains(t.Name))
                                     .Select(t => t.Name)
                                     .ToListAsync(token).ConfigureAwait(false))
                    .Select(t => new Tag(t));

                if (missingTags.Any())
                {
                    context.Tags.AddRange(missingTags);
                    await context.SaveChangesAsync(token).ConfigureAwait(false);
                }

                tagMap = await context.Tags
                                      .Where(t => tags.Contains(t.Name))
                                      .ToDictionaryAsync(t => t.Name, t => t.Id, token).ConfigureAwait(false);
            }

            List<CrowdactionTag> crowdactionTags =
                tags.Select(t => new CrowdactionTag(tagId: tagMap[t]))
                    .ToList();

            var crowdaction = new Crowdaction(
                name: newCrowdaction.Name,
                status: CrowdactionStatus.Hidden,
                ownerId: owner.Id,
                target: newCrowdaction.Target,
                start: newCrowdaction.Start,
                end: newCrowdaction.End.Date.AddHours(23).AddMinutes(59).AddSeconds(59),
                description: newCrowdaction.Description,
                goal: newCrowdaction.Goal,
                proposal: newCrowdaction.Proposal,
                creatorComments: newCrowdaction.CreatorComments,
                descriptionVideoLink: newCrowdaction.DescriptionVideoLink?.Replace("www.youtube.com", "www.youtube-nocookie.com", StringComparison.Ordinal),
                displayPriority: CrowdactionDisplayPriority.Medium,
                bannerImageFileId: newCrowdaction.BannerImageFileId,
                cardImageFileId: newCrowdaction.CardImageFileId,
                descriptiveImageFileId: newCrowdaction.DescriptiveImageFileId,
                categories: newCrowdaction.Categories.Select(c => new CrowdactionCategory((c))).ToList(),
                tags: crowdactionTags);

            context.Crowdactions.Add(crowdaction);
            await context.SaveChangesAsync(token).ConfigureAwait(false);

            await RefreshParticipantCount(token).ConfigureAwait(false);

            await emailSender.SendEmailTemplated(owner.Email, $"Thank you for submitting \"{crowdaction.Name}\" on CollAction", "CrowdactionConfirmation").ConfigureAwait(false);

            IList<ApplicationUser> administrators = await userManager.GetUsersInRoleAsync(AuthorizationConstants.AdminRole).ConfigureAwait(false);
            await emailSender.SendEmailsTemplated(administrators.Select(a => a.Email), $"A new crowdaction was submitted on CollAction: \"{crowdaction.Name}\"", "CrowdactionAddedAdmin", crowdaction.Name).ConfigureAwait(false);

            logger.LogInformation("Created crowdaction: {0}", newCrowdaction.Name);

            return new CrowdactionResult(crowdaction);
        }

        public async Task<int> DeleteCrowdaction(int id, CancellationToken token)
        {
            Crowdaction? crowdaction = await context.Crowdactions.SingleOrDefaultAsync(c => c.Id == id, token).ConfigureAwait(false);

            if (crowdaction == null)
            {
                throw new InvalidOperationException($"Crowdaction {id} doesn't exist");
            }

            crowdaction.Status = CrowdactionStatus.Deleted;
            await context.SaveChangesAsync().ConfigureAwait(false);

            return id;
        }

        public async Task<CrowdactionResult> UpdateCrowdaction(UpdatedCrowdaction updatedCrowdaction, CancellationToken token)
        {
            logger.LogInformation("Validating updated crowdaction");

            IEnumerable<ValidationResult> validationResults = ValidationHelper.Validate(updatedCrowdaction, serviceProvider);
            if (validationResults.Any())
            {
                return new CrowdactionResult(validationResults);
            }

            ApplicationUser? owner = await userManager.FindByIdAsync(updatedCrowdaction.OwnerId).ConfigureAwait(false);
            if (owner == null && updatedCrowdaction.OwnerId != null)
            {
                return new CrowdactionResult(new ValidationResult("New user owner does not exist"));
            }

            Crowdaction? crowdaction = await context
                .Crowdactions
                .Include(c => c.Tags).ThenInclude(t => t.Tag)
                .Include(c => c.Categories)
                .FirstOrDefaultAsync(c => c.Id == updatedCrowdaction.Id, token).ConfigureAwait(false);

            if (crowdaction == null)
            {
                return new CrowdactionResult(new ValidationResult("Crowdaction not found", new[] { nameof(Crowdaction.Id) }));
            }

            if (crowdaction.Name != updatedCrowdaction.Name && await context.Crowdactions.AnyAsync(c => c.Name == updatedCrowdaction.Name).ConfigureAwait(false))
            {
                return new CrowdactionResult(new ValidationResult("A crowdaction with this name already exists", new[] { nameof(Crowdaction.Name) }));
            }

            logger.LogInformation("Updating crowdaction: {0}", updatedCrowdaction.Name);

            bool approved = updatedCrowdaction.Status == CrowdactionStatus.Running && crowdaction.Status != CrowdactionStatus.Running;
            bool changeFinishJob = (approved || crowdaction.End != updatedCrowdaction.End) && updatedCrowdaction.End < DateTime.UtcNow;
            bool removeFinishJob = updatedCrowdaction.Status != CrowdactionStatus.Running && crowdaction.FinishJobId != null;
            bool deleted = updatedCrowdaction.Status == CrowdactionStatus.Deleted;

            crowdaction.Name = updatedCrowdaction.Name;
            crowdaction.Description = updatedCrowdaction.Description;
            crowdaction.Proposal = updatedCrowdaction.Proposal;
            crowdaction.Goal = updatedCrowdaction.Goal;
            crowdaction.CreatorComments = updatedCrowdaction.CreatorComments;
            crowdaction.Target = updatedCrowdaction.Target;
            crowdaction.Start = updatedCrowdaction.Start;
            crowdaction.End = updatedCrowdaction.End.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
            crowdaction.BannerImageFileId = updatedCrowdaction.BannerImageFileId;
            crowdaction.CardImageFileId = updatedCrowdaction.CardImageFileId;
            crowdaction.DescriptiveImageFileId = updatedCrowdaction.DescriptiveImageFileId;
            crowdaction.DescriptionVideoLink = updatedCrowdaction.DescriptionVideoLink?.Replace("www.youtube.com", "www.youtube-nocookie.com", StringComparison.Ordinal);
            crowdaction.Status = updatedCrowdaction.Status;
            crowdaction.DisplayPriority = updatedCrowdaction.DisplayPriority;
            crowdaction.NumberCrowdactionEmailsSent = updatedCrowdaction.NumberCrowdactionEmailsSent;
            crowdaction.OwnerId = updatedCrowdaction.OwnerId;
            context.Crowdactions.Update(crowdaction);

            var crowdactionTags = crowdaction.Tags.Select(t => t.Tag!.Name);
            var tags = updatedCrowdaction.Tags.Distinct().ToList();
            if (!Enumerable.SequenceEqual(tags.OrderBy(t => t), crowdactionTags.OrderBy(t => t)))
            {
                IEnumerable<string> missingTags =
                    tags.Except(await context.Tags
                                             .Where(t => tags.Contains(t.Name))
                                             .Select(t => t.Name)
                                             .ToListAsync(token).ConfigureAwait(false));

                if (missingTags.Any())
                {
                    context.Tags.AddRange(missingTags.Select(t => new Tag(t)));
                    await context.SaveChangesAsync(token).ConfigureAwait(false);
                }

                var tagMap =
                    await context.Tags
                                 .Where(t => tags.Contains(t.Name) || crowdactionTags.Contains(t.Name))
                                 .ToDictionaryAsync(t => t.Name, t => t.Id, token).ConfigureAwait(false);

                IEnumerable<CrowdactionTag> newTags =
                    tags.Except(crowdactionTags)
                        .Select(t => new CrowdactionTag(crowdactionId: crowdaction.Id, tagId: tagMap[t]));

                IEnumerable<CrowdactionTag> removedTags =
                    crowdaction.Tags
                               .Where(t => crowdactionTags.Except(tags).Contains(t.Tag!.Name));
                context.CrowdactionTags.AddRange(newTags);
                context.CrowdactionTags.RemoveRange(removedTags);
            }

            var categories = crowdaction.Categories.Select(c => c.Category);
            if (!Enumerable.SequenceEqual(categories.OrderBy(c => c), updatedCrowdaction.Categories.OrderBy(c => c)))
            {
                IEnumerable<Category> newCategories = updatedCrowdaction.Categories.Except(categories);
                IEnumerable<CrowdactionCategory> removedCategories = crowdaction.Categories.Where(pc => !updatedCrowdaction.Categories.Contains(pc.Category));

                context.CrowdactionCategories.RemoveRange(removedCategories);
                context.CrowdactionCategories.AddRange(newCategories.Select(c => new CrowdactionCategory(crowdactionId: crowdaction.Id, category: c)));
            }

            if (approved && owner != null)
            {
                await emailSender.SendEmailTemplated(owner.Email, $"Approval - {crowdaction.Name}", "CrowdactionApproval").ConfigureAwait(false);
            }
            else if (deleted && owner != null)
            {
                await emailSender.SendEmailTemplated(owner.Email, $"Deleted - {crowdaction.Name}", "CrowdactionDeleted").ConfigureAwait(false);
            }

            if (changeFinishJob)
            {
                if (crowdaction.FinishJobId != null)
                {
                    jobClient.Delete(crowdaction.FinishJobId);
                }

                crowdaction.FinishJobId = jobClient.Schedule(() => CrowdactionEndProcess(crowdaction.Id, CancellationToken.None), crowdaction.End);
            }
            else if (removeFinishJob)
            {
                jobClient.Delete(crowdaction.FinishJobId);
            }

            await context.SaveChangesAsync(token).ConfigureAwait(false);
            logger.LogInformation("Updated crowdaction: {0}", updatedCrowdaction.Name);

            return new CrowdactionResult(crowdaction);
        }

        public async Task CrowdactionEndProcess(int crowdactionId, CancellationToken token)
        {
            logger.LogInformation("Processing end of crowdaction: {0}", crowdactionId);
            await RefreshParticipantCount(token).ConfigureAwait(false); // Ensure the participant count is up-to-date
            Crowdaction? crowdaction = await context.Crowdactions.Include(c => c.ParticipantCounts).FirstAsync(c => c.Id == crowdactionId, token).ConfigureAwait(false);

            if (crowdaction == null)
            {
                throw new InvalidOperationException($"Crowdaction {crowdactionId} does not exist");
            }

            if (crowdaction.IsSuccessfull && crowdaction.Owner != null)
            {
                await emailSender.SendEmailTemplated(crowdaction.Owner.Email, $"Success - {crowdaction.Name}", "CrowdactionSuccess").ConfigureAwait(false);
            }
            else if (crowdaction.IsFailed && crowdaction.Owner != null)
            {
                await emailSender.SendEmailTemplated(crowdaction.Owner.Email, $"Failed - {crowdaction.Name}", "CrowdactionFailed").ConfigureAwait(false);
            }
        }

        public async Task<CrowdactionResult> SendCrowdactionEmail(int crowdactionId, string subject, string message, ClaimsPrincipal performingUser, CancellationToken token)
        {
            Crowdaction? crowdaction = await context.Crowdactions.SingleOrDefaultAsync(c => c.Id == crowdactionId, token).ConfigureAwait(false);
            if (crowdaction == null)
            {
                return new CrowdactionResult(new ValidationResult("Crowdaction could not be found", new[] { nameof(crowdactionId) }));
            }

            if (!(htmlInputValidator.IsSafe(message) && htmlInputValidator.IsSafe(subject)))
            {
                return new CrowdactionResult(new ValidationResult("Unsafe HTML in e-mail message", new[] { nameof(message) }));
            }

            ApplicationUser? user = await userManager.GetUserAsync(performingUser).ConfigureAwait(false);
            if (crowdaction.OwnerId != user.Id)
            {
                return new CrowdactionResult(new ValidationResult("Unauthorized"));
            }

            IEnumerable<CrowdactionParticipant> participants =
                await context.CrowdactionParticipants
                             .Include(part => part.User)
                             .Where(part => part.CrowdactionId == crowdaction.Id && part.SubscribedToCrowdactionEmails)
                             .ToListAsync(token).ConfigureAwait(false);

            logger.LogInformation("sending crowdaction email for '{0}' on {1} to {2} users", crowdaction.Name, subject, participants.Count());

            foreach (CrowdactionParticipant participant in participants)
            {
                Uri unsubscribeLink = new Uri(siteOptions.PublicUrl, $"{crowdaction.Url}/unsubscribe-email?userId={WebUtility.UrlEncode(participant.UserId)}&token={WebUtility.UrlEncode(participant.UnsubscribeToken.ToString())}");
                emailSender.SendEmail(participant.User!.Email, subject, FormatEmailMessage(message, participant.User, unsubscribeLink));
            }

            IList<ApplicationUser> adminUsers = await userManager.GetUsersInRoleAsync(AuthorizationConstants.AdminRole).ConfigureAwait(false);
            foreach (ApplicationUser admin in adminUsers)
            {
                emailSender.SendEmail(admin.Email, subject, FormatEmailMessage(message, admin, null));
            }

            if (crowdaction.Owner != null)
            {
                emailSender.SendEmail(crowdaction.Owner.Email, subject, FormatEmailMessage(message, crowdaction.Owner, null));
            }

            crowdaction.NumberCrowdactionEmailsSent += 1;
            await context.SaveChangesAsync(token).ConfigureAwait(false);

            logger.LogInformation("done sending crowdaction email for '{0}' on {1} to {2} users", crowdaction.Name, subject, participants.Count());
            return new CrowdactionResult(crowdaction);
        }

        public bool CanSendCrowdactionEmail(Crowdaction crowdaction)
            => crowdaction.CanSendCrowdactionEmail(MaxNumberCrowdactionEmails, TimeEmailAllowedAfterCrowdactionEnd);

        public async Task<CrowdactionParticipant> SetEmailCrowdactionSubscription(int crowdactionId, string userId, Guid token, bool isSubscribed, CancellationToken cancellationToken)
        {
            CrowdactionParticipant participant = await context
                 .CrowdactionParticipants
                 .Include(c => c.Crowdaction)
                 .FirstAsync(c => c.CrowdactionId == crowdactionId && c.UserId == userId).ConfigureAwait(false);

            if (participant != null && participant.UnsubscribeToken == token)
            {
                logger.LogInformation("Setting crowdaction subscription for user");
                participant.SubscribedToCrowdactionEmails = isSubscribed;
                await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
            else
            {
                logger.LogError("Unable to set crowdaction subscription for user, invalid token");
                throw new InvalidOperationException("Not authorized");
            }

            return participant;
        }

        public async Task<CrowdactionComment> CreateComment(string comment, int crowdactionId, ClaimsPrincipal user, CancellationToken token)
        {
            if (!htmlInputValidator.IsSafe(comment))
            {
                throw new InvalidOperationException("Comment contains unsafe html");
            }

            comment = SanitizeComment(comment);

            ApplicationUser? applicationUser = await userManager.GetUserAsync(user).ConfigureAwait(false);

            if (applicationUser == null)
            {
                throw new InvalidOperationException($"User does not exist when adding comment");
            }

            Crowdaction? crowdaction = await context.Crowdactions.SingleAsync(c => c.Id == crowdactionId, token).ConfigureAwait(false);

            if (crowdaction == null)
            {
                throw new InvalidOperationException($"Crowdaction does not exist when adding comment");
            }
            else if (crowdaction.Status != CrowdactionStatus.Running)
            {
                throw new InvalidOperationException($"Crowdaction is not active when adding comment");
            }

            var crowdactionComment = new CrowdactionComment(comment, applicationUser.Id, null, crowdactionId, DateTime.UtcNow, CrowdactionCommentStatus.Approved);
            context.CrowdactionComments.Add(crowdactionComment);
            await context.SaveChangesAsync(token).ConfigureAwait(false);

            return crowdactionComment;
        }

        public async Task<CrowdactionComment> CreateCommentAnonymous(string comment, string name, int crowdactionId, CancellationToken token)
        {
            if (!htmlInputValidator.IsSafe(comment))
            {
                throw new InvalidOperationException("Comment contains unsafe html");
            }

            comment = SanitizeComment(comment);

            Crowdaction? crowdaction = await context.Crowdactions.SingleOrDefaultAsync(c => c.Id == crowdactionId, token).ConfigureAwait(false);

            if (crowdaction == null)
            {
                throw new InvalidOperationException($"Crowdaction does not exist when adding comment");
            }
            else if (crowdaction.Status != CrowdactionStatus.Running)
            {
                throw new InvalidOperationException($"Crowdaction is not active when adding comment");
            }

            var crowdactionComment = new CrowdactionComment(comment, null, name, crowdactionId, DateTime.UtcNow, CrowdactionCommentStatus.WaitingForApproval);
            context.CrowdactionComments.Add(crowdactionComment);
            await context.SaveChangesAsync(token).ConfigureAwait(false);

            IList<ApplicationUser> administrators = await userManager.GetUsersInRoleAsync(AuthorizationConstants.AdminRole).ConfigureAwait(false);
            await emailSender.SendEmailsTemplated(administrators.Select(a => a.Email), $"A new anonymous comment was submitted on CollAction", "CrowdactionCommentAddedAdmin").ConfigureAwait(false);

            return crowdactionComment;
        }

        public async Task<CrowdactionComment> ApproveComment(int commentId, CancellationToken token)
        {
            CrowdactionComment? crowdactionComment = await context.CrowdactionComments.SingleOrDefaultAsync(c => c.Id == commentId, token).ConfigureAwait(false);

            if (crowdactionComment == null)
            {
                throw new InvalidOperationException($"Comment does not exist when approving comment");
            }

            crowdactionComment.Status = CrowdactionCommentStatus.Approved;
            await context.SaveChangesAsync(token).ConfigureAwait(false);

            return crowdactionComment;
        }

        public async Task DeleteComment(int commentId, CancellationToken token)
        {
            CrowdactionComment comment = await context.CrowdactionComments.SingleAsync(c => c.Id == commentId, token).ConfigureAwait(false);
            context.CrowdactionComments.Remove(comment);
            await context.SaveChangesAsync(token).ConfigureAwait(false);
        }

        public async Task<AddParticipantResult> CommitToCrowdactionAnonymous(string email, int crowdactionId, CancellationToken token)
        {
            Crowdaction? crowdaction = await context.Crowdactions.SingleOrDefaultAsync(c => c.Id == crowdactionId, token).ConfigureAwait(false);
            if (crowdaction == null || !crowdaction.IsActive)
            {
                logger.LogError("Crowdaction not found or active: {0}", crowdactionId);
                return new AddParticipantResult($"Crowdaction not found or is not active");
            }

            if (!IsValidEmail(email))
            {
                logger.LogWarning("Invalid e-mail signing up to crowdaction");
                return new AddParticipantResult("Invalid e-mail address");
            }

            var result = new AddParticipantResult();
            ApplicationUser? user = await userManager.FindByEmailAsync(email).ConfigureAwait(false);
            if (user == null)
            {
                user = new ApplicationUser(email: email, registrationDate: DateTime.UtcNow);
                IdentityResult creationResult = await userManager.CreateAsync(user).ConfigureAwait(false);
                if (!creationResult.Succeeded)
                {
                    string errors = string.Join(',', creationResult.Errors.Select(e => $"{e.Code}: {e.Description}"));
                    logger.LogError("Could not create new unregistered user for crowdaction commit: {0}", errors);
                    return new AddParticipantResult($"Could not create new unregistered user: {errors}");
                }
                result.UserCreated = true;
            }

            result.UserAdded = await InsertParticipant(crowdaction.Id, user.Id, token).ConfigureAwait(false);
            result.UserAlreadyActive = user.Activated;

            logger.LogInformation("Added participant to crowdaction: {0}, {1}", user.Id, crowdactionId);

            if (!user.Activated)
            {
                result.ParticipantEmail = user.Email;
                result.PasswordResetToken = await userManager.GeneratePasswordResetTokenAsync(user).ConfigureAwait(false);
            }

            if (result.Scenario != AddParticipantScenario.AnonymousNotRegisteredPresentAndAlreadyParticipating &&
                result.Scenario != AddParticipantScenario.AnonymousAlreadyRegisteredAndAlreadyParticipating)
            {
                await SendCommitEmail(crowdaction, result, user, user.Email).ConfigureAwait(false);
            }

            logger.LogInformation("Added participant '{0}' to crowdaction '{1}' with scenario '{2}'", user.Id, crowdactionId, result.Scenario);

            return result;
        }

        public async Task<AddParticipantResult> CommitToCrowdactionLoggedIn(ClaimsPrincipal user, int crowdactionId, CancellationToken token)
        {
            Crowdaction? crowdaction = await context.Crowdactions.SingleOrDefaultAsync(c => c.Id == crowdactionId, token).ConfigureAwait(false);
            if (crowdaction == null || !crowdaction.IsActive)
            {
                logger.LogError("Crowdaction not found or active: {0}", crowdactionId);
                return new AddParticipantResult("Crowdaction not found or not active");
            }

            ApplicationUser? applicationUser = await userManager.GetUserAsync(user).ConfigureAwait(false);
            if (applicationUser == null)
            {
                logger.LogError("User not logged in when committing");
                return new AddParticipantResult(error: "User not logged in");
            }

            bool added = await InsertParticipant(crowdaction.Id, applicationUser.Id, token).ConfigureAwait(false);
            var result = new AddParticipantResult(loggedIn: true, userAdded: added);

            if (added)
            {
                await SendCommitEmail(crowdaction, result, applicationUser, applicationUser.Email).ConfigureAwait(false);
            }

            logger.LogInformation("Added participant '{0}' to crowdaction '{1}' with scenario '{2}'", applicationUser.Id, crowdactionId, result.Scenario);

            return result;
        }

        public IQueryable<Crowdaction> SearchCrowdactions(Category? category, SearchCrowdactionStatus? searchCrowdactionStatus)
        {
            IQueryable<Crowdaction> crowdactions = context
                .Crowdactions
                .Include(c => c.ParticipantCounts)
                .OrderBy(c => c.DisplayPriority).ThenBy(c => c.Id)
                .AsQueryable();

            crowdactions = searchCrowdactionStatus switch
            {
                SearchCrowdactionStatus.Closed => crowdactions.Where(c => c.End < DateTime.UtcNow && c.Status == CrowdactionStatus.Running),
                SearchCrowdactionStatus.ComingSoon => crowdactions.Where(c => c.Start > DateTime.UtcNow && c.Status == CrowdactionStatus.Running),
                SearchCrowdactionStatus.Open => crowdactions.Where(c => c.Start <= DateTime.UtcNow && c.End >= DateTime.UtcNow && c.Status == CrowdactionStatus.Running),
                SearchCrowdactionStatus.Active => crowdactions.Where(c => c.Status == CrowdactionStatus.Running),
                _ => crowdactions.Where(c => c.Status != CrowdactionStatus.Deleted)
            };

            if (category != null)
            {
                crowdactions = crowdactions.Include(c => c.Categories).Where(c => c.Categories.Any(crowdactionCategory => crowdactionCategory.Category == category));
            }

            return crowdactions;
        }

        public IQueryable<CrowdactionComment> SearchCrowdactionComments(int? crowdactionId, CrowdactionCommentStatus? status)
        {
            var comments =
                crowdactionId != null ?
                    context.CrowdactionComments.Where(c => c.CrowdactionId == crowdactionId) :
                    context.CrowdactionComments;

            return status != null ?
                       comments.Where(c => c.Status == status) :
                       comments;
        }

        public Task RefreshParticipantCount(CancellationToken token)
            => context.Database.ExecuteSqlRawAsync("REFRESH MATERIALIZED VIEW CONCURRENTLY \"CrowdactionParticipantCounts\";", token);

        public void InitializeRefreshParticipantCountJob()
        {
            recurringJobManager.AddOrUpdate(
                "refresh-participant-count-job",
                () => RefreshParticipantCount(CancellationToken.None),
                "*/5 * * * *" // Every 5 minutes
                );
        }

        private static string FormatEmailMessage(string message, ApplicationUser user, Uri? unsubscribeLink)
        {
            if (unsubscribeLink != null)
            {
                message += $"<br><a href=\"{unsubscribeLink}\">Unsubscribe</a>";
            }

            if (string.IsNullOrEmpty(user.FirstName))
            {
                message = message.Replace(" {firstname}", string.Empty, StringComparison.Ordinal)
                                 .Replace("{firstname}", string.Empty, StringComparison.Ordinal);
            }
            else
            {
                message = message.Replace("{firstname}", user.FirstName, StringComparison.Ordinal);
            }

            if (string.IsNullOrEmpty(user.LastName))
            {
                message = message.Replace(" {lastname}", string.Empty, StringComparison.Ordinal)
                                 .Replace("{lastname}", string.Empty, StringComparison.Ordinal);
            }
            else
            {
                message = message.Replace("{lastname}", user.LastName, StringComparison.Ordinal);
            }

            return message;
        }

        private async Task SendCommitEmail(Crowdaction crowdaction, AddParticipantResult result, ApplicationUser applicationUser, string email)
        {
            var commitEmailViewModel = new CrowdactionCommitEmailViewModel(crowdaction: crowdaction, result: result, user: applicationUser, publicUrl: siteOptions.PublicUrl, crowdactionUrl: new Uri(siteOptions.PublicUrl, crowdaction.Url));
            await emailSender.SendEmailTemplated(email, $"Thank you for participating in the \"{crowdaction.Name}\" crowdaction on CollAction", "CrowdactionCommit", commitEmailViewModel).ConfigureAwait(false);
        }

        private async Task<bool> InsertParticipant(int crowdactionId, string userId, CancellationToken token)
        {
            logger.LogInformation("Adding participant '{0}' to crowdaction '{1}'", userId, crowdactionId);

            if (await context.CrowdactionParticipants.AnyAsync(part => part.UserId == userId && part.CrowdactionId == crowdactionId).ConfigureAwait(false))
            {
                return false;
            }

            CrowdactionParticipant participant = new CrowdactionParticipant(userId: userId, crowdactionId: crowdactionId, subscribedToCrowdactionEmails: true, participationDate: DateTime.UtcNow, unsubscribeToken: Guid.NewGuid());

            try
            {
                context.CrowdactionParticipants.Add(participant);

                await context.SaveChangesAsync(token).ConfigureAwait(false);

                logger.LogInformation("Added participant '{0}' to crowdaction '{1}'", userId, crowdactionId);

                return true;
            }
            catch (DbUpdateException e)
            {
                logger.LogWarning(e, "Duplicate crowdaction subscription, failure adding participant '{0}' to crowdaction '{1}'", userId, crowdactionId);
                // User is already participating
                return false;
            }
        }

        private static string SanitizeComment(string comment)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(comment);
            foreach (HtmlNode link in doc.DocumentNode.Descendants("a"))
            {
                // Add nofollow ugc to all links so search engines don't follow the links
                link.SetAttributeValue("rel", "nofollow ugc");
                string? href = link.GetAttributeValue<string?>("href", null);
                if (href == null)
                {
                    throw new InvalidOperationException("Links without href are not allowed");
                }
                // Other schemas than http/https have already been excluded before this check
                else if (!href.StartsWith("http://", StringComparison.OrdinalIgnoreCase) && !href.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                {
                    // Add https schema if no schema specified
                    link.SetAttributeValue("href", $"https://{href}");
                }
            }
            using var writer = new StringWriter();
            doc.Save(writer);
            return writer.ToString();
        }

        private static bool IsValidEmail(string email)
        {
            try
            {
                _ = new MailAddress(email);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
