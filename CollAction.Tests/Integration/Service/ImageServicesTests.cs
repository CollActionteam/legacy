using CollAction.Services.Image;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace CollAction.Tests.Integration.Service
{
    [Trait("Category", "Integration")]
    public sealed class ImageServicesTests : IntegrationTestBase
    {
        private readonly byte[] testImage = new byte[] { 0x42, 0x4D, 0x1E, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x1A, 0x00, 0x00, 0x00, 0x0C, 0x00, 0x00, 0x00, 0x01, 0x00, 0x01, 0x00, 0x01, 0x00, 0x18, 0x00, 0x00, 0x00, 0xFF, 0x00 };
        private readonly MemoryStream imageMs;
        private readonly IImageService imageService;
        private readonly Mock<IFormFile> upload;

        public ImageServicesTests() : base(false)
        {
            imageMs = new MemoryStream(testImage);
            imageService = Scope.ServiceProvider.GetRequiredService<IImageService>();
            upload = new Mock<IFormFile>();
            upload.Setup(u => u.OpenReadStream()).Returns(imageMs);
        }

        [Fact]
        public async Task TestUpload()
        {
            Models.ImageFile imageFile = await imageService.UploadImage(upload.Object, "test", 1600, CancellationToken.None).ConfigureAwait(false);

            Assert.NotNull(imageFile);
            Assert.NotNull(imageFile.Filepath);
            Assert.Equal("test", imageFile.Description);
            Assert.Equal(HttpStatusCode.OK, await CheckUrl(imageService.GetUrl(imageFile)).ConfigureAwait(false));

            await imageService.DeleteImage(imageFile, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestDelete()
        {
            Models.ImageFile imageFile = await imageService.UploadImage(upload.Object, "test", 1600, CancellationToken.None).ConfigureAwait(false);
            await imageService.DeleteImage(imageFile, CancellationToken.None).ConfigureAwait(false);
            Assert.Equal(HttpStatusCode.Forbidden, await CheckUrl(imageService.GetUrl(imageFile)).ConfigureAwait(false));
        }

        private static async Task<HttpStatusCode> CheckUrl(Uri url)
        {
            using HttpClient client = new HttpClient();
            using HttpResponseMessage response = await client.GetAsync(url).ConfigureAwait(false);
            return response.StatusCode;
        }
    }
}
