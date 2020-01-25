using CollAction.Services.Image;
using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace CollAction.Tests.Integration.Service
{
    [TestClass]
    [TestCategory("Integration")]
    public sealed class ImageServicesTests : IntegrationTestBase
    {
        private readonly byte[] testImage = new byte[] { 0x42, 0x4D, 0x1E, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x1A, 0x00, 0x00, 0x00, 0x0C, 0x00, 0x00, 0x00, 0x01, 0x00, 0x01, 0x00, 0x01, 0x00, 0x18, 0x00, 0x00, 0x00, 0xFF, 0x00 };
        private readonly Mock<IBackgroundJobClient> jobClient;
        private readonly Mock<IFormFile> upload;
        private IImageService imageService;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "MemoryStream is mocked")]
        public ImageServicesTests()
        {
            upload = new Mock<IFormFile>();
            upload.Setup(u => u.OpenReadStream()).Returns(new MemoryStream(testImage));
            jobClient = new Mock<IBackgroundJobClient>();
            jobClient.Setup(jc => jc.Create(It.IsAny<Job>(), It.IsAny<IState>()))
                      .Returns<Job, IState>(
                          (job, state) =>
                          {
                              Task.Run(() => (Task)job.Method.Invoke(imageService, job.Args.ToArray())).Wait();
                              return string.Empty;
                          });
        }

        [TestMethod]
        public Task TestUpload()
            => WithServiceProvider(
                   async scope =>
                   {
                       imageService = scope.ServiceProvider.GetRequiredService<IImageService>();
                       Models.ImageFile imageFile = await imageService.UploadImage(upload.Object, "test", CancellationToken.None).ConfigureAwait(false);

                       Assert.IsNotNull(imageFile);
                       Assert.IsNotNull(imageFile.Filepath);
                       Assert.AreEqual("test", imageFile.Description);
                       upload.Verify(f => f.OpenReadStream());
                       upload.VerifyNoOtherCalls();
                       jobClient.Verify(jc => jc.Create(It.IsAny<Job>(), It.IsAny<IState>()));
                       jobClient.VerifyNoOtherCalls();
                       Assert.AreEqual(HttpStatusCode.OK, await CheckUrl(imageService.GetUrl(imageFile)).ConfigureAwait(false));

                       await imageService.DeleteImage(imageFile, CancellationToken.None).ConfigureAwait(false);
                   });

        [TestMethod]
        public Task TestDelete()
            => WithServiceProvider(
                   async scope =>
                   {
                       imageService = scope.ServiceProvider.GetRequiredService<IImageService>();
                       Models.ImageFile imageFile = await imageService.UploadImage(upload.Object, "test", CancellationToken.None).ConfigureAwait(false);
                       await imageService.DeleteImage(imageFile, CancellationToken.None).ConfigureAwait(false);

                       upload.Verify(f => f.OpenReadStream());
                       upload.VerifyNoOtherCalls();
                       jobClient.Verify(jc => jc.Create(It.IsAny<Job>(), It.IsAny<IState>()));
                       jobClient.VerifyNoOtherCalls();
                       Assert.AreEqual(HttpStatusCode.Forbidden, await CheckUrl(imageService.GetUrl(imageFile)).ConfigureAwait(false));
                   });

        protected override void ConfigureReplacementServicesProvider(IServiceCollection collection)
        {
            collection.AddScoped(s => jobClient.Object);
        }

        private async Task<HttpStatusCode> CheckUrl(Uri url)
        {
            using HttpClient client = new HttpClient();
            using HttpResponseMessage response = await client.GetAsync(url).ConfigureAwait(false);
            return response.StatusCode;
        }
    }
}
