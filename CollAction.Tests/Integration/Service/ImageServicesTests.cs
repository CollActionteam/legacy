using CollAction.Services.Image;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.IO;
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
        private readonly Mock<IFormFile> upload;
        private IImageService imageService;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "MemoryStream is mocked")]
        public ImageServicesTests()
        {
            upload = new Mock<IFormFile>();
            upload.Setup(u => u.OpenReadStream()).Returns(new MemoryStream(testImage));
        }

        [TestMethod]
        public Task TestUpload()
            => WithServiceProvider(
                   async scope =>
                   {
                       imageService = scope.ServiceProvider.GetRequiredService<IImageService>();
                       Models.ImageFile imageFile = await imageService.UploadImage(upload.Object, "test", 1600, CancellationToken.None).ConfigureAwait(false);

                       Assert.IsNotNull(imageFile);
                       Assert.IsNotNull(imageFile.Filepath);
                       Assert.AreEqual("test", imageFile.Description);
                       Assert.AreEqual(HttpStatusCode.OK, await CheckUrl(imageService.GetUrl(imageFile)).ConfigureAwait(false));

                       await imageService.DeleteImage(imageFile, CancellationToken.None).ConfigureAwait(false);
                   });

        [TestMethod]
        public Task TestDelete()
            => WithServiceProvider(
                   async scope =>
                   {
                       imageService = scope.ServiceProvider.GetRequiredService<IImageService>();
                       Models.ImageFile imageFile = await imageService.UploadImage(upload.Object, "test", 1600, CancellationToken.None).ConfigureAwait(false);
                       await imageService.DeleteImage(imageFile, CancellationToken.None).ConfigureAwait(false);
                       Assert.AreEqual(HttpStatusCode.Forbidden, await CheckUrl(imageService.GetUrl(imageFile)).ConfigureAwait(false));
                   });

        private static async Task<HttpStatusCode> CheckUrl(Uri url)
        {
            using HttpClient client = new HttpClient();
            using HttpResponseMessage response = await client.GetAsync(url).ConfigureAwait(false);
            return response.StatusCode;
        }
    }
}
