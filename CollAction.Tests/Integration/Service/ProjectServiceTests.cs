using CollAction.Data;
using CollAction.GraphQl.Mutations.Input;
using CollAction.Models;
using CollAction.Services.Email;
using CollAction.Services.Projects;
using CollAction.Services.Projects.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace CollAction.Tests.Integration.Service
{
    [TestClass]
    [TestCategory("Integration")]
    public sealed class ProjectServiceTests : IntegrationTestBase
    {
        [TestMethod]
        public Task TestProjectCreate()
            => WithServiceProvider(
                   async scope =>
                   {
                       IProjectService projectService = scope.ServiceProvider.GetRequiredService<IProjectService>();
                       ApplicationDbContext context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                       SignInManager<ApplicationUser> signInManager = scope.ServiceProvider.GetRequiredService<SignInManager<ApplicationUser>>();
                       var user = await context.Users.FirstAsync();
                       var claimsPrincipal = await signInManager.CreateUserPrincipalAsync(user);
                       var r = new Random();

                       var newProject =
                           new NewProject()
                           {
                               Name = "test" + Guid.NewGuid(),
                               Categories = new List<Category>() { Category.Other, Category.Health },
                               Description = Guid.NewGuid().ToString(),
                               DescriptionVideoLink = "https://www.youtube.com/watch?v=a1",
                               End = DateTime.Now.AddDays(30),
                               Start = DateTime.Now.AddDays(10),
                               Goal = Guid.NewGuid().ToString(),
                               CreatorComments = Guid.NewGuid().ToString(),
                               Proposal = Guid.NewGuid().ToString(),
                               Target = 40,
                               Tags = new string[3] { r.Next(1000).ToString(), r.Next(1000).ToString(), r.Next(1000).ToString() }
                           };
                       ProjectResult projectResult = await projectService.CreateProject(newProject, claimsPrincipal, CancellationToken.None);
                       int? projectId = projectResult.Project?.Id;
                       Assert.IsNotNull(projectId);
                       Project retrievedProject = await context.Projects.Include(p => p.Tags).ThenInclude(t => t.Tag).FirstOrDefaultAsync(p => p.Id == projectId);
                       Assert.IsNotNull(retrievedProject);

                       Assert.IsTrue(projectResult.Succeeded);
                       Assert.IsFalse(projectResult.Errors.Any());
                       Assert.AreEqual(projectResult.Project?.Name, retrievedProject.Name);
                       Assert.IsTrue(Enumerable.SequenceEqual(projectResult.Project?.Tags.Select(t => t.Tag.Name).OrderBy(t => t), retrievedProject.Tags.Select(t => t.Tag.Name).OrderBy(t => t)));
                   });

        [TestMethod]
        public Task TestProjectUpdate()
            => WithServiceProvider(
                   async scope =>
                   {
                       IProjectService projectService = scope.ServiceProvider.GetRequiredService<IProjectService>();
                       ApplicationDbContext context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                       SignInManager<ApplicationUser> signInManager = scope.ServiceProvider.GetRequiredService<SignInManager<ApplicationUser>>();
                       UserManager<ApplicationUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                       Project currentProject = await context.Projects.Include(p => p.Owner).FirstAsync(p => p.OwnerId != null);
                       var admin = (await userManager.GetUsersInRoleAsync(AuthorizationConstants.AdminRole)).First();
                       var adminClaims = await signInManager.CreateUserPrincipalAsync(admin);
                       var owner = await signInManager.CreateUserPrincipalAsync(currentProject.Owner ?? throw new InvalidOperationException("Owner is null"));
                       var r = new Random();
                       var updatedProject =
                           new UpdatedProject()
                           {
                               Name = Guid.NewGuid().ToString(),
                               BannerImageFileId = currentProject.BannerImageFileId,
                               Categories = new[] { Category.Community, Category.Environment },
                               CreatorComments = currentProject.CreatorComments,
                               Description = currentProject.Description,
                               OwnerId = currentProject.OwnerId,
                               DescriptionVideoLink = "https://www.youtube.com/watch?v=a1",
                               DescriptiveImageFileId = currentProject.DescriptiveImageFileId,
                               DisplayPriority = ProjectDisplayPriority.Top,
                               End = DateTime.Now.AddDays(30),
                               Start = DateTime.Now.AddDays(10),
                               Goal = Guid.NewGuid().ToString(),
                               Tags = new string[3] { r.Next(1000).ToString(), r.Next(1000).ToString(), r.Next(1000).ToString() },
                               Id = currentProject.Id,
                               NumberProjectEmailsSend = 3,
                               Proposal = currentProject.Proposal,
                               Status = ProjectStatus.Running,
                               Target = 33
                           };
                       var newProjectResult = await projectService.UpdateProject(updatedProject, adminClaims, CancellationToken.None);
                       Assert.IsTrue(newProjectResult.Succeeded);
                       int? newProjectId = newProjectResult.Project?.Id;
                       Assert.IsNotNull(newProjectId);
                       var retrievedProject = await context.Projects.Include(p => p.Tags).ThenInclude(t => t.Tag).FirstOrDefaultAsync(p => p.Id == newProjectId);

                       Assert.AreEqual(updatedProject.Name, retrievedProject.Name);
                       Assert.IsTrue(Enumerable.SequenceEqual(updatedProject.Tags.OrderBy(t => t), retrievedProject.Tags.Select(t => t.Tag.Name).OrderBy(t => t)));
                   });

        [TestMethod]
        public Task TestProjectCommit()
            => WithServiceProvider(
                   async scope =>
                   {
                       IProjectService projectService = scope.ServiceProvider.GetRequiredService<IProjectService>();
                       ApplicationDbContext context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                       SignInManager<ApplicationUser> signInManager = scope.ServiceProvider.GetRequiredService<SignInManager<ApplicationUser>>();
                       UserManager<ApplicationUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                       var user = await context.Users.FirstAsync();
                       var currentProject = await context.Projects.Include(p => p.Owner).FirstOrDefaultAsync();

                       string testEmail = GetTestEmail();
                       var result = await projectService.CommitToProject(testEmail, currentProject.Id, new ClaimsPrincipal(), CancellationToken.None);
                       Assert.AreEqual(AddParticipantScenario.AnonymousCreatedAndAdded, result.Scenario);
                   });

        [TestMethod]
        public Task TestProjectEmail()
            => WithServiceProvider(
                   async scope =>
                   {
                       IProjectService projectService = scope.ServiceProvider.GetRequiredService<IProjectService>();
                       ApplicationDbContext context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                       SignInManager<ApplicationUser> signInManager = scope.ServiceProvider.GetRequiredService<SignInManager<ApplicationUser>>();
                       UserManager<ApplicationUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                       var user = await context.Users.FirstAsync();
                       var claimsUser = await signInManager.CreateUserPrincipalAsync(user);
                       var newProject =
                           new Project(
                               name: $"test{Guid.NewGuid()}",
                               categories: new List<ProjectCategory>() { new ProjectCategory(Category.Environment), new ProjectCategory(Category.Community) },
                               tags: new List<ProjectTag>(),
                               description: Guid.NewGuid().ToString(),
                               descriptionVideoLink: Guid.NewGuid().ToString(),
                               start: DateTime.Now.AddDays(-10),
                               end: DateTime.Now.AddDays(30),
                               goal: Guid.NewGuid().ToString(),
                               creatorComments: Guid.NewGuid().ToString(),
                               proposal: Guid.NewGuid().ToString(),
                               target: 40,
                               status: ProjectStatus.Running,
                               displayPriority: ProjectDisplayPriority.Medium,
                               ownerId: user.Id);
                       context.Projects.Add(newProject);
                       await context.SaveChangesAsync();

                       Assert.AreEqual(0, newProject.NumberProjectEmailsSend);
                       Assert.IsTrue(projectService.CanSendProjectEmail(newProject));
                       await projectService.SendProjectEmail(newProject.Id, "test", "test", claimsUser, CancellationToken.None);
                       Assert.AreEqual(1, newProject.NumberProjectEmailsSend);
                       Assert.IsTrue(projectService.CanSendProjectEmail(newProject));
                       for (int i = 0; i < 3; i++)
                       {
                           await projectService.SendProjectEmail(newProject.Id, "test", "test", claimsUser, CancellationToken.None);
                       }

                       Assert.AreEqual(4, newProject.NumberProjectEmailsSend);
                       Assert.IsFalse(projectService.CanSendProjectEmail(newProject));
                   });

        [TestMethod]
        public Task TestProjectSearch()
            => WithServiceProvider(
                   async scope =>
                   {
                       IProjectService projectService = scope.ServiceProvider.GetRequiredService<IProjectService>();
                       Random r = new Random();

                       Assert.IsTrue(await projectService.SearchProjects(null, null).AnyAsync());

                       Category searchCategory = (Category)r.Next(7);
                       Assert.IsTrue(await projectService.SearchProjects(searchCategory, null).Include(p => p.Categories).AllAsync(p => p.Categories.Any(pc => pc.Category == searchCategory)));
                       Assert.IsTrue(await projectService.SearchProjects(null, SearchProjectStatus.Closed).AllAsync(p => p.End < DateTime.UtcNow));
                       Assert.IsTrue(await projectService.SearchProjects(searchCategory, SearchProjectStatus.Closed).AllAsync(p => p.End < DateTime.UtcNow));
                       Assert.IsTrue(await projectService.SearchProjects(null, SearchProjectStatus.ComingSoon).AllAsync(p => p.Start > DateTime.UtcNow && p.Status == ProjectStatus.Running));
                       Assert.IsTrue(await projectService.SearchProjects(searchCategory, SearchProjectStatus.ComingSoon).AllAsync(p => p.Start > DateTime.UtcNow && p.Status == ProjectStatus.Running));
                       Assert.IsTrue(await projectService.SearchProjects(null, SearchProjectStatus.Open).AllAsync(p => p.Start <= DateTime.UtcNow && p.End >= DateTime.UtcNow && p.Status == ProjectStatus.Running));
                       Assert.IsTrue(await projectService.SearchProjects(searchCategory, SearchProjectStatus.Open).AllAsync(p => p.Start <= DateTime.UtcNow && p.End >= DateTime.UtcNow && p.Status == ProjectStatus.Running));
                   });

        protected override void ConfigureReplacementServicesProvider(IServiceCollection collection)
        {
            collection.AddTransient(s => new Mock<IEmailSender>().Object);
        }

        private string GetTestEmail()
            => $"collaction-test-email-{Guid.NewGuid()}@collaction.org";
    }
}
