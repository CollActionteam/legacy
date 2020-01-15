using CollAction.Data;
using CollAction.Services;
using CollAction.Services.Image;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace CollAction.Tests.Integration.Endpoint
{
    [TestClass]
    [TestCategory("Integration")]
    public class ImageEndpointTests : IntegrationTestBase
    {
        private readonly byte[] testImage = new byte[] { 0x42, 0x4D, 0x1E, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x1A, 0x00, 0x00, 0x00, 0x0C, 0x00, 0x00, 0x00, 0x01, 0x00, 0x01, 0x00, 0x01, 0x00, 0x18, 0x00, 0x00, 0x00, 0xFF, 0x00 };

        [TestMethod]
        public Task TestImageUpload()
            => WithTestServer(
                   async (scope, testServer) =>
                   {
                       using var content = new MultipartFormDataContent();
                       using var memoryStream = new MemoryStream(testImage);
                       using var streamContent = new StreamContent(memoryStream);
                       using var descriptionContent = new StringContent("My Description");
                       using var client = testServer.CreateClient();
                       SeedOptions seedOptions = scope.ServiceProvider.GetRequiredService<IOptions<SeedOptions>>().Value;
                       client.DefaultRequestHeaders.Add("Cookie", await GetAuthCookie(client, seedOptions));
                       content.Add(streamContent, "Image", "test.png");
                       content.Add(descriptionContent, "ImageDescription");
                       using var response = await client.PostAsync("/image", content);
                       string body = await response.Content.ReadAsStringAsync();
                       Assert.IsTrue(response.IsSuccessStatusCode, body);
                       int imageId = int.Parse(body);

                       // Cleanup
                       var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                       var imageService = scope.ServiceProvider.GetRequiredService<IImageService>();
                       var image = await context.ImageFiles.FindAsync(imageId);
                       await imageService.DeleteImage(image, CancellationToken.None);
                   });
    }
}
