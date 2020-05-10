using CollAction.Data;
using CollAction.Models;
using CollAction.Services.Crowdactions;
using CollAction.Services.Crowdactions.Models;
using CollAction.Services.Image;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace CollAction.Services.Initialization
{
    public class InitializationService : IInitializationService
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly SeedOptions seedOptions;
        private readonly ICrowdactionService crowdactionService;
        private readonly IImageService imageService;
        private readonly IBackgroundJobClient jobClient;
        private readonly ApplicationDbContext context;
        private readonly ILogger<InitializationService> logger;
        private const int MaxImageBannerDimensionPixels = 1600;
        private const int MaxImageCardDimensionPixels = 370;

        public InitializationService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IOptions<SeedOptions> seedOptions, ICrowdactionService crowdactionService, IImageService imageService, IBackgroundJobClient jobClient, ApplicationDbContext context, ILogger<InitializationService> logger)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.seedOptions = seedOptions.Value;
            this.crowdactionService = crowdactionService;
            this.imageService = imageService;
            this.jobClient = jobClient;
            this.context = context;
            this.logger = logger;
        }

        public async Task InitializeDatabase()
        {
            logger.LogInformation("Migrating database");
            await context.Database.MigrateAsync().ConfigureAwait(false);
            logger.LogInformation("Creating admin user if absent");
            ApplicationUser admin = await CreateAdminRoleAndUser().ConfigureAwait(false);
            if (seedOptions.SeedTestData && !(await crowdactionService.SearchCrowdactions(null, null).AnyAsync().ConfigureAwait(false)))
            {
                logger.LogInformation("Seeding database");
                jobClient.Enqueue(() => SeedTestData(admin.Id, CancellationToken.None));
            }
            logger.LogInformation("Migrating card images, setting up jobs");
            await MigrateCardImages(CancellationToken.None).ConfigureAwait(false);
        }

        public async Task SeedTestData(string adminId, CancellationToken cancellationToken)
        {
            var admin = await userManager.FindByIdAsync(adminId).ConfigureAwait(false);
            var seededUsers = await SeedTestUsers(cancellationToken).ConfigureAwait(false);
            await SeedRandomCrowdactions(seededUsers.Concat(new[] { admin }), cancellationToken).ConfigureAwait(false);
        }

        // TODO: Remove once this has run in production
        public async Task MigrateCardImages(CancellationToken token)
        {
            List<int> crowdactionsToMigrate =
                await context.Crowdactions
                             .Where(c => c.CardImageFileId == null && c.BannerImageFileId != null)
                             .Select(c => c.Id)
                             .ToListAsync(token)
                             .ConfigureAwait(false);
            crowdactionsToMigrate.ForEach(crowdactionId => jobClient.Enqueue(() => MigrateCardImage(crowdactionId, CancellationToken.None)));
        }

        // TODO: Remove once this has run in production
        public async Task MigrateCardImage(int crowdactionId, CancellationToken token)
        {
            Crowdaction crowdaction =
                await context.Crowdactions
                             .Include(c => c.BannerImage)
                             .SingleAsync(c => c.Id == crowdactionId, token)
                             .ConfigureAwait(false);

            if (crowdaction.BannerImage == null)
            {
                throw new InvalidOperationException($"Banner image not found when migrating for crowdaction: {crowdaction.Id}");
            }

            Uri bannerImageUrl = imageService.GetUrl(crowdaction.BannerImage);
            byte[] bannerImage = await DownloadFile(bannerImageUrl, token).ConfigureAwait(false);
            ImageFile uploadedImage = await imageService.UploadImage(ToFormFile(bannerImage, bannerImageUrl), crowdaction.BannerImage.Description, MaxImageCardDimensionPixels, token).ConfigureAwait(false);
            crowdaction.CardImage = uploadedImage;
            await context.SaveChangesAsync(token).ConfigureAwait(false);
        }

        private static IFormFile ToFormFile(byte[] imageBytes, Uri url)
        {
            return new FormFile(new MemoryStream(imageBytes), 0, imageBytes.Length, url.LocalPath, url.LocalPath);
        }

        private static async Task<byte[]> DownloadFile(Uri url, CancellationToken cancellationToken)
        {
            using var client = new HttpClient();
            using var response = await client.GetAsync(url, cancellationToken).ConfigureAwait(false);
            return await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
        }

        private async Task<ApplicationUser> CreateAdminRoleAndUser()
        {
            // Create admin role if not exists
            IdentityRole? adminRole = await roleManager.FindByNameAsync(AuthorizationConstants.AdminRole).ConfigureAwait(false);
            if (adminRole == null)
            {
                adminRole = new IdentityRole(AuthorizationConstants.AdminRole) { NormalizedName = AuthorizationConstants.AdminRole };
                IdentityResult result = await roleManager.CreateAsync(adminRole).ConfigureAwait(false);
                if (!result.Succeeded)
                {
                    throw new InvalidOperationException($"Error creating role.{Environment.NewLine}{string.Join(Environment.NewLine, result.Errors.Select(e => $"{e.Code}: {e.Description}"))}");
                }
            }

            // Create admin user if not exists
            ApplicationUser? admin = await userManager.FindByEmailAsync(seedOptions.AdminEmail).ConfigureAwait(false);
            if (admin == null)
            {
                admin = new ApplicationUser(userName: seedOptions.AdminEmail, email: seedOptions.AdminEmail, emailConfirmed: true, registrationDate: DateTime.UtcNow, firstName: null, lastName: null);
                IdentityResult result = await userManager.CreateAsync(admin, seedOptions.AdminPassword).ConfigureAwait(false);
                if (!result.Succeeded)
                {
                    throw new InvalidOperationException($"Error creating user.{Environment.NewLine}{string.Join(Environment.NewLine, result.Errors.Select(e => $"{e.Code}: {e.Description}"))}");
                }
            }

            // Assign admin role if not assigned
            if (!(await userManager.IsInRoleAsync(admin, AuthorizationConstants.AdminRole).ConfigureAwait(false)))
            {
                IdentityResult result = await userManager.AddToRoleAsync(admin, AuthorizationConstants.AdminRole).ConfigureAwait(false);
                if (!result.Succeeded)
                {
                    throw new InvalidOperationException($"Error assigning admin role.{Environment.NewLine}{string.Join(Environment.NewLine, result.Errors.Select(e => $"{e.Code}: {e.Description}"))}");
                }
            }

            return admin;
        }

        private async Task<IEnumerable<ApplicationUser>> SeedTestUsers(CancellationToken cancellationToken)
        {
            Random r = new Random();
            var users = Enumerable.Range(0, r.Next(20, 100))
                                  .Select(i => new ApplicationUser(Faker.Name.First() + "@example.com", Faker.Name.First(), Faker.Name.Last(), DateTime.UtcNow))
                                  .ToList();
            context.Users.AddRange(users);
            await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return users;
        }

        private async Task SeedRandomCrowdactions(IEnumerable<ApplicationUser> users, CancellationToken cancellationToken)
        {
            Random r = new Random();
            DateTime now = DateTime.UtcNow;

            string?[] videoLinks = new[]
            {
                "https://www.youtube-nocookie.com/embed/aLzM_L5fjCQ",
                "https://www.youtube-nocookie.com/embed/Zvugem-tKyI",
                "https://www.youtube-nocookie.com/embed/xY0XTysJUDY",
                "https://www.youtube-nocookie.com/embed/2yfPLxQQG-k",
                null
            };

            List<(Uri bannerImageUrl, Task<byte[]> bannerImageBytes)> bannerImages = new[]
            {
                "https://collaction-production.s3.eu-central-1.amazonaws.com/57136ed4-b7f6-4dd2-a822-9341e2e60d1e.png",
                "https://collaction-production.s3.eu-central-1.amazonaws.com/765bc57b-748e-4bb8-a27e-08db6b99ea3e.png",
                "https://collaction-production.s3.eu-central-1.amazonaws.com/e06bbc2d-02f7-4a9b-a744-6923d5b21f51.png",
            }.Select(b => new Uri(b)).Select(b => (b, DownloadFile(b, cancellationToken))).ToList();

            List<(Uri descriptiveImageUrl, Task<byte[]> descriptiveImageBytes)> descriptiveImages = new[]
            {
                "https://collaction-production.s3.eu-central-1.amazonaws.com/107104bc-deeb-4f48-b3a5-f25585bebf89.png",
                "https://collaction-production.s3.eu-central-1.amazonaws.com/365f2dc9-1784-45ea-9cc7-d5f0ef1a480c.png",
                "https://collaction-production.s3.eu-central-1.amazonaws.com/6e6c12b1-eaae-4811-aa1c-c169d10f1a59.png",
            }.Select(b => new Uri(b)).Select(b => (b, DownloadFile(b, cancellationToken))).ToList();

            await Task.WhenAll(descriptiveImages.Select(d => d.descriptiveImageBytes).Concat(bannerImages.Select(b => b.bannerImageBytes))).ConfigureAwait(false);

            List<string> tags = Enumerable.Range(10, r.Next(60))
                                          .Select(r => Faker.Internet.DomainWord())
                                          .Distinct()
                                          .ToList();

            var crowdactionNames =
                Enumerable.Range(0, r.Next(40, 120))
                          .Select(i => Faker.Company.Name())
                          .Distinct()
                          .ToList();

            List<string> userIds = await context.Users.Select(u => u.Id).ToListAsync().ConfigureAwait(false);
            List<Crowdaction> crowdactions = new List<Crowdaction>(crowdactionNames.Count);

            // Generate random crowdactions
            foreach (string crowdactionName in crowdactionNames)
            {
                DateTime start = now.Date.AddDays(r.Next(-10, 20));

                IEnumerable<string> crowdactionTags =
                    Enumerable.Range(0, r.Next(5))
                              .Select(i => r.Next(tags.Count))
                              .Distinct()
                              .Select(i => tags[i])
                              .ToList();

                List<Category> categories = new[] { r.Next(7), r.Next(7) }.Distinct()
                                                                          .Select(i => (Category)i)
                                                                          .ToList();

                (Uri descriptiveImageUrl, Task<byte[]> descriptiveImageBytes) = descriptiveImages[r.Next(descriptiveImages.Count)];
                ImageFile? descriptiveImage = r.Next(3) == 0
                                                  ? null
                                                  : await imageService.UploadImage(ToFormFile(descriptiveImageBytes.Result, descriptiveImageUrl), Faker.Company.BS(), MaxImageBannerDimensionPixels, cancellationToken).ConfigureAwait(false);

                (Uri bannerImageUrl, Task<byte[]> bannerImageBytes) = bannerImages[r.Next(bannerImages.Count)];
                ImageFile? bannerImage = r.Next(3) == 0
                                             ? null
                                             : await imageService.UploadImage(ToFormFile(bannerImageBytes.Result, bannerImageUrl), Faker.Company.BS(), MaxImageBannerDimensionPixels, cancellationToken).ConfigureAwait(false);

                ImageFile? cardImage = bannerImage == null
                                          ? null
                                          : await imageService.UploadImage(ToFormFile(bannerImageBytes.Result, bannerImageUrl), Faker.Company.BS(), MaxImageCardDimensionPixels, cancellationToken).ConfigureAwait(false);

                ApplicationUser owner = users.ElementAt(users.Count() - 1);
                CrowdactionStatus status = (CrowdactionStatus)r.Next(3);
                NewCrowdactionInternal newCrowdaction =
                    new NewCrowdactionInternal(
                        name: crowdactionName,
                        description: $"<p>{string.Join("</p><p>", Faker.Lorem.Paragraphs(r.Next(3) + 1))}</p>",
                        start: start,
                        end: start.AddDays(r.Next(10, 40)).AddHours(23).AddMinutes(59).AddSeconds(59),
                        categories: categories,
                        tags: crowdactionTags,
                        bannerImageFileId: bannerImage?.Id,
                        descriptiveImageFileId: descriptiveImage?.Id,
                        cardImageFileId: cardImage?.Id,
                        creatorComments: r.Next(4) == 0 ? null : $"<p>{string.Join("</p><p>", Faker.Lorem.Paragraphs(r.Next(3) + 1))}</p>",
                        goal: Faker.Company.CatchPhrase(),
                        proposal: Faker.Company.BS(),
                        target: r.Next(1, 10000),
                        descriptionVideoLink: videoLinks.ElementAt(r.Next(videoLinks.Length)),
                        displayPriority: (CrowdactionDisplayPriority)r.Next(3),
                        status: (CrowdactionStatus)r.Next(3),
                        ownerId: owner.Id,
                        anonymousUserParticipants: r.Next(1, 8000));

                Crowdaction crowdaction = await crowdactionService.CreateCrowdactionInternal(newCrowdaction, cancellationToken).ConfigureAwait(false);

                context.CrowdactionParticipants.AddRange(
                    userIds.Where(userId => r.Next(2) == 0)
                           .Select(userId => new CrowdactionParticipant(userId, crowdaction.Id, r.Next(2) == 1, now, Guid.NewGuid())));

                await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
