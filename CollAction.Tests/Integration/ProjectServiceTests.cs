using CollAction.Data;
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
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace CollAction.Tests.Integration
{
    [TestClass]
    [TestCategory("Integration")]
    public class ProjectServiceTests : IntegrationTestBase
    {
        [TestMethod]
        public Task TestProjectCreate()
            => WithServiceProvider(
                   ConfigureReplacementServices,
                   async scope =>
                   {
                       IProjectService projectService = scope.ServiceProvider.GetRequiredService<IProjectService>();
                       ApplicationDbContext context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                       SignInManager<ApplicationUser> signInManager = scope.ServiceProvider.GetRequiredService<SignInManager<ApplicationUser>>();
                       var user = await context.Users.FirstAsync();
                       var claimsPrincipal = await signInManager.CreateUserPrincipalAsync(user);

                       var newProject =
                           new NewProject()
                           {
                               Name = "test" + Guid.NewGuid(),
                               CategoryId = 2,
                               Description = Guid.NewGuid().ToString(),
                               DescriptionVideoLink = Guid.NewGuid().ToString(),
                               End = DateTime.Now.AddDays(30),
                               Start = DateTime.Now.AddDays(10),
                               Goal = Guid.NewGuid().ToString(),
                               CreatorComments = Guid.NewGuid().ToString(),
                               Proposal = Guid.NewGuid().ToString(),
                               Target = 40,
                               Tags = new string[3] { Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString() }
                           };
                       var project = await projectService.CreateProject(newProject, claimsPrincipal, CancellationToken.None);
                       var retrievedProject = await context.Projects.Include(p => p.Tags).ThenInclude(t => t.Tag).FirstOrDefaultAsync(p => p.Id == project.Id);

                       Assert.AreEqual(project.Name, retrievedProject.Name);
                       Assert.IsTrue(Enumerable.SequenceEqual(project.Tags.Select(t => t.Tag.Name).OrderBy(t => t), retrievedProject.Tags.Select(t => t.Tag.Name).OrderBy(t => t)));
                   });

        [TestMethod]
        public Task TestProjectUpdate()
            => WithServiceProvider(
                   ConfigureReplacementServices,
                   async scope =>
                   {
                       IProjectService projectService = scope.ServiceProvider.GetRequiredService<IProjectService>();
                       ApplicationDbContext context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                       SignInManager<ApplicationUser> signInManager = scope.ServiceProvider.GetRequiredService<SignInManager<ApplicationUser>>();
                       UserManager<ApplicationUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                       var currentProject = await context.Projects.Include(p => p.Owner).FirstOrDefaultAsync();
                       var admin = (await userManager.GetUsersInRoleAsync(Constants.AdminRole)).First();
                       var adminClaims = await signInManager.CreateUserPrincipalAsync(admin);
                       var updatedProject =
                           new UpdatedProject()
                           {
                               Name = Guid.NewGuid().ToString(),
                               BannerImageFileId = currentProject.BannerImageFileId,
                               CategoryId = currentProject.CategoryId,
                               CreatorComments = currentProject.CreatorComments,
                               Description = currentProject.Description,
                               DescriptionVideoLink = Guid.NewGuid().ToString(),
                               DescriptiveImageFileId = currentProject.DescriptiveImageFileId,
                               DisplayPriority = ProjectDisplayPriority.Top,
                               End = DateTime.Now.AddDays(30),
                               Start = DateTime.Now.AddDays(10),
                               Goal = Guid.NewGuid().ToString(),
                               Tags = new string[3] { Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString() },
                               Id = currentProject.Id,
                               NumberProjectEmailsSend = 3,
                               Owner = await signInManager.CreateUserPrincipalAsync(currentProject.Owner),
                               Proposal = currentProject.Proposal,
                               Status = ProjectStatus.Running,
                               Target = 33
                           };
                       var newProject = await projectService.UpdateProject(updatedProject, adminClaims, CancellationToken.None);
                       var retrievedProject = await context.Projects.Include(p => p.Tags).ThenInclude(t => t.Tag).FirstOrDefaultAsync(p => p.Id == newProject.Id);

                       Assert.AreEqual(updatedProject.Name, retrievedProject.Name);
                       Assert.IsTrue(Enumerable.SequenceEqual(updatedProject.Tags.OrderBy(t => t), retrievedProject.Tags.Select(t => t.Tag.Name).OrderBy(t => t)));
                   });

        [TestMethod]
        public Task TestProjectCommit()
            => WithServiceProvider(
                   ConfigureReplacementServices,
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
                   ConfigureReplacementServices,
                   async scope =>
                   {
                       IProjectService projectService = scope.ServiceProvider.GetRequiredService<IProjectService>();
                       ApplicationDbContext context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                       SignInManager<ApplicationUser> signInManager = scope.ServiceProvider.GetRequiredService<SignInManager<ApplicationUser>>();
                       UserManager<ApplicationUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                       var user = await context.Users.FirstAsync();
                       var claimsUser = await signInManager.CreateUserPrincipalAsync(user);
                       var newProject =
                           new Project()
                           {
                               Name = "test" + Guid.NewGuid(),
                               CategoryId = 2,
                               Description = Guid.NewGuid().ToString(),
                               DescriptionVideoLink = Guid.NewGuid().ToString(),
                               End = DateTime.Now.AddDays(30),
                               Start = DateTime.Now.AddDays(-10),
                               Goal = Guid.NewGuid().ToString(),
                               CreatorComments = Guid.NewGuid().ToString(),
                               Proposal = Guid.NewGuid().ToString(),
                               Target = 40,
                               Status = ProjectStatus.Running,
                               OwnerId = user.Id
                           };
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

        private void ConfigureReplacementServices(ServiceCollection sc)
        {
            sc.AddTransient(s => new Mock<IEmailSender>().Object);
        }

        private string GetTestEmail()
            => $"collaction-test-email-{Guid.NewGuid()}@outlook.com";
    }
}
