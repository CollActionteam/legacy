using CollAction.Data;
using CollAction.Models;
using CollAction.Services;
using CollAction.Services.Image;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace CollAction.Tests.Integration
{
    [TestClass]
    [TestCategory("Integration")]
    public class ImageEndpointTests : IntegrationTestBase
    {
        const string Image = "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mNkqAcAAIUAgUW0RjgAAAAASUVORK5CYII=";

        [TestMethod]
        public Task TestImageUpload()
            => WithTestServer(
                   async (scope, testServer) =>
                   {
                       int? imageId = null;

                       try
                       {
                           byte[] imageBytes = Convert.FromBase64String(Image);
                           using (var content = new MultipartFormDataContent())
                           using (var memoryStream = new MemoryStream(imageBytes))
                           using (var streamContent = new StreamContent(memoryStream))
                           using (var descriptionContent = new StringContent("My Description"))
                           using (var client = testServer.CreateClient())
                           {
                               SeedOptions seedOptions = scope.ServiceProvider.GetRequiredService<IOptions<SeedOptions>>().Value;
                               client.DefaultRequestHeaders.Add("Cookie", await GetAuthCookie(client, seedOptions));
                               content.Add(streamContent, "Image", "test.png");
                               content.Add(descriptionContent, "ImageDescription");
                               using (var response = await client.PostAsync("/upload", content))
                               {
                                   string body = await response.Content.ReadAsStringAsync();
                                   Assert.IsTrue(response.IsSuccessStatusCode, body);
                                   imageId = int.Parse(body);
                               }
                           }
                       }
                       finally
                       {
                           if (imageId != null)
                           {
                               var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                               var imageService = scope.ServiceProvider.GetRequiredService<IImageService>();
                               var image = await context.ImageFiles.FindAsync(imageId);
                               await imageService.DeleteImage(image, CancellationToken.None);
                           }
                       }
                   });
    }
}
