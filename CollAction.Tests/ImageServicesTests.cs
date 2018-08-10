using CollAction.Services.Image;
using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace CollAction.Tests
{
    [TestClass]
    public sealed class ImageServicesTests
    {
        private readonly byte[] _image = new byte[] { 0x42, 0x4D, 0x1E, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x1A, 0x00, 0x00, 0x00, 0x0C, 0x00, 0x00, 0x00, 0x01, 0x00, 0x01, 0x00, 0x01, 0x00, 0x18, 0x00, 0x00, 0x00, 0xFF, 0x00 };
        private readonly ImageServiceOptions _options;
        private Mock<IBackgroundJobClient> _jobClient;
        private ImageService _imageService;
        private Mock<IFormFile> _upload;

        public ImageServicesTests()
        {
            IConfiguration configuration = 
                new ConfigurationBuilder().AddUserSecrets<Startup>()
                                          .AddEnvironmentVariables()
                                          .Build();
            _options = new ImageServiceOptions();
            configuration.Bind(_options);
        }

        [TestInitialize()]
        public void Initialize()
        {
            _jobClient = new Mock<IBackgroundJobClient>();
            _jobClient.Setup(jc => jc.Create(It.IsAny<Job>(), It.IsAny<IState>()))
                      .Returns<Job, IState>(
                          (job, state) => {
                              Task.Run(() => (Task)job.Method.Invoke(_imageService, job.Args.ToArray())).Wait();
                              return string.Empty;
                          });
            _imageService = new ImageService(null, null, new OptionsWrapper<ImageServiceOptions>(_options), _jobClient.Object);
            _upload = new Mock<IFormFile>();
            _upload.Setup(u => u.OpenReadStream()).Returns(new MemoryStream(_image));
        }

        [TestCleanup()]
        public void Cleanup()
        {
            _imageService.Dispose();
        }

        [TestMethod]
        public async Task TestUpload()
        {
            Models.ImageFile imageFile = await _imageService.UploadImage(null, _upload.Object, "test");

            Assert.IsNotNull(imageFile);
            Assert.IsNotNull(imageFile.Filepath);
            Assert.AreEqual("test", imageFile.Description);
            _upload.Verify(f => f.OpenReadStream());
            _upload.VerifyNoOtherCalls();
            _jobClient.Verify(jc => jc.Create(It.IsAny<Job>(), It.IsAny<IState>()));
            _jobClient.VerifyNoOtherCalls();
            Assert.AreEqual(HttpStatusCode.OK, await CheckUrl(_imageService.GetUrl(imageFile)));

            _imageService.DeleteImage(imageFile);
        }

        [TestMethod]
        public async Task TestDelete()
        {
            Models.ImageFile imageFile = await _imageService.UploadImage(null, _upload.Object, "test");
            _imageService.DeleteImage(imageFile);

            _upload.Verify(f => f.OpenReadStream());
            _upload.VerifyNoOtherCalls();
            _jobClient.Verify(jc => jc.Create(It.IsAny<Job>(), It.IsAny<IState>()));
            _jobClient.VerifyNoOtherCalls();
            Assert.AreEqual(HttpStatusCode.Forbidden, await CheckUrl(_imageService.GetUrl(imageFile)));
        }

        private async Task<HttpStatusCode> CheckUrl(string url)
        {
            using (HttpClient client = new HttpClient())
                using (HttpResponseMessage response = await client.GetAsync(url))
                    return response.StatusCode;
        }
    }
}
