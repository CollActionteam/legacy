using CollAction.Services.Email;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading.Tasks;

namespace CollAction.Tests.Integration
{
    [TestClass]
    [TestCategory("Integration")]
    public class UserServiceTests : IntegrationTestBase // TODO
    {
        [TestMethod]
        public Task TestUserCreate()
            => WithServiceProvider(
                ConfigureReplacementServices,
                async scope =>
                {
                });

        private void ConfigureReplacementServices(ServiceCollection sc)
        {
            sc.AddTransient(s => new Mock<IEmailSender>().Object);
        }
    }
}
