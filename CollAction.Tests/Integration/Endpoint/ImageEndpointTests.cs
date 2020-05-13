using CollAction.Data;
using CollAction.Services;
using CollAction.Services.Image;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace CollAction.Tests.Integration.Endpoint
{
    [Trait("Category", "Integration")]
    public sealed class ImageEndpointTests : IntegrationTestBase
    {
        private readonly byte[] testImage = new byte[] { 0x42, 0x4D, 0x1E, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x1A, 0x00, 0x00, 0x00, 0x0C, 0x00, 0x00, 0x00, 0x01, 0x00, 0x01, 0x00, 0x01, 0x00, 0x18, 0x00, 0x00, 0x00, 0xFF, 0x00 };

        [Fact]
        public async Task TestImageUpload()
        {
            using var content = new MultipartFormDataContent();
            using var memoryStream = new MemoryStream(testImage);
            using var streamContent = new StreamContent(memoryStream);
            using var descriptionContent = new StringContent("My Description");
            using var sizeContent = new StringContent("400");
            using var client = TestServer.CreateClient();
            SeedOptions seedOptions = Scope.ServiceProvider.GetRequiredService<IOptions<SeedOptions>>().Value;
            client.DefaultRequestHeaders.Add("Cookie", await GetAuthCookie(client, seedOptions).ConfigureAwait(false));
            content.Add(streamContent, "Image", "test.png");
            content.Add(descriptionContent, "ImageDescription");
            content.Add(sizeContent, "ImageResizeThreshold");
            using var response = await client.PostAsync(new Uri("/image", UriKind.Relative), content).ConfigureAwait(false);
            string body = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            Assert.True(response.IsSuccessStatusCode, body);
            int imageId = int.Parse(body, CultureInfo.InvariantCulture);

            // Cleanup
            var context = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var imageService = Scope.ServiceProvider.GetRequiredService<IImageService>();
            var image = await context.ImageFiles.FindAsync(imageId).ConfigureAwait(false);
            await imageService.DeleteImage(image, CancellationToken.None).ConfigureAwait(false);
       }
    }
}
