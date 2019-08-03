using CollAction.Data;
using CollAction.Services.Image;
using CollAction.Services.Newsletter;
using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace CollAction.Tests.Integration
{
    [TestClass]
    [TestCategory("Integration")]
    public sealed class ImageServicesTests
    {
        private readonly byte[] testImage = new byte[] { 0x42, 0x4D, 0x1E, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x1A, 0x00, 0x00, 0x00, 0x0C, 0x00, 0x00, 0x00, 0x01, 0x00, 0x01, 0x00, 0x01, 0x00, 0x18, 0x00, 0x00, 0x00, 0xFF, 0x00 };
        private readonly ImageServiceOptions options;
        private readonly ImageProcessingOptions imageProcessingOptions;
        private Mock<IBackgroundJobClient> jobClient;
        private AmazonS3ImageService imageService;
        private Mock<IFormFile> upload;

        public ImageServicesTests()
        {
            IConfiguration configuration = 
                new ConfigurationBuilder().AddUserSecrets<Startup>()
                                          .AddEnvironmentVariables()
                                          .Build();
            options = new ImageServiceOptions();
            imageProcessingOptions = new ImageProcessingOptions();
            configuration.Bind(options);
        }

        [TestInitialize]
        public void Initialize()
        {
            jobClient = new Mock<IBackgroundJobClient>();
            jobClient.Setup(jc => jc.Create(It.IsAny<Job>(), It.IsAny<IState>()))
                      .Returns<Job, IState>(
                          (job, state) =>
                          {
                              Task.Run(() => (Task)job.Method.Invoke(imageService, job.Args.ToArray())).Wait();
                              return string.Empty;
                          });
            IConfiguration configuration =
                new ConfigurationBuilder().AddUserSecrets<Startup>()
                                          .AddEnvironmentVariables()
                                          .Build();
            string connectionString = $"Host={configuration["DbHost"]};Username={configuration["DbUser"]};Password={configuration["DbPassword"]};Database={configuration["Db"]};Port={configuration["DbPort"]}";
            imageService = new AmazonS3ImageService(
                new OptionsWrapper<ImageServiceOptions>(options),
                new OptionsWrapper<ImageProcessingOptions>(imageProcessingOptions),
                jobClient.Object, 
                new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>().UseNpgsql(connectionString).Options),
                new LoggerFactory().CreateLogger<AmazonS3ImageService>()); 
            upload = new Mock<IFormFile>();
            upload.Setup(u => u.OpenReadStream()).Returns(new MemoryStream(testImage));
        }

        [TestCleanup]
        public void Cleanup()
        {
            imageService.Dispose();
        }

        [TestMethod]
        public async Task TestUpload()
        {
            Models.ImageFile imageFile = await imageService.UploadImage(upload.Object, "test", CancellationToken.None);

            Assert.IsNotNull(imageFile);
            Assert.IsNotNull(imageFile.Filepath);
            Assert.AreEqual("test", imageFile.Description);
            upload.Verify(f => f.OpenReadStream());
            upload.VerifyNoOtherCalls();
            jobClient.Verify(jc => jc.Create(It.IsAny<Job>(), It.IsAny<IState>()));
            jobClient.VerifyNoOtherCalls();
            Assert.AreEqual(HttpStatusCode.OK, await CheckUrl(imageService.GetUrl(imageFile)));

            await imageService.DeleteImage(imageFile, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestDelete()
        {
            Models.ImageFile imageFile = await imageService.UploadImage(upload.Object, "test", CancellationToken.None);
            await imageService.DeleteImage(imageFile, CancellationToken.None);

            upload.Verify(f => f.OpenReadStream());
            upload.VerifyNoOtherCalls();
            jobClient.Verify(jc => jc.Create(It.IsAny<Job>(), It.IsAny<IState>()));
            jobClient.VerifyNoOtherCalls();
            Assert.AreEqual(HttpStatusCode.Forbidden, await CheckUrl(imageService.GetUrl(imageFile)));
        }

        private async Task<HttpStatusCode> CheckUrl(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage response = await client.GetAsync(url))
                {
                    return response.StatusCode;
                }
            }
        }
    }
}
