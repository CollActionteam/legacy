using CollAction.Data;
using CollAction.Models;
using ImageSharp;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace CollAction.Helpers
{
    public class ImageFileManager
    {
        private ApplicationDbContext _context;
        private string _webRoot;
        private string _webFolder;

        public ImageFileManager(ApplicationDbContext context, string webRoot, string webFolder)
        {
            _context = context;
            _webRoot = webRoot;
            _webFolder = webFolder;
        }

        public async Task<ImageFile> UploadFormFile(IFormFile formFile, string fileName, string description)
        {
            if (formFile == null) { return null; }
            string extension = Path.GetExtension(formFile.FileName).ToLower().Substring(1); // Strip the "."
            await SaveFileToFileSystem(formFile, fileName, extension);
            var imageModel = await CreateImageFileModel(fileName, extension, description);
            _context.ImageFiles.Add(imageModel);
            await _context.SaveChangesAsync(); // need to save to the database to get an ID
            return imageModel;
        }

        private async Task SaveFileToFileSystem(IFormFile formFile, string fileName, string extension)
        {
            var fullPath = Path.Combine(_webRoot, GetWebPath(fileName, extension));
            using (var output = new FileStream(fullPath, FileMode.Create))
            {
                await formFile.CopyToAsync(output);
            }
        }

        public void DeleteImageFile(ImageFile imageFile)
        {
            if (imageFile == null) { return; }
            DeleteImageFileFromFileSystem(imageFile);
            DeleteImageFileFromModel(imageFile);
        }

        private void DeleteImageFileFromFileSystem(ImageFile imageFile)
        {
            var fullPath = Path.Combine(_webRoot, imageFile.Filepath.TrimStart(new char[] { '\\' }));
            var fileInfo = new FileInfo(fullPath);
            if (fileInfo != null) { fileInfo.Delete(); }
        }

        private void DeleteImageFileFromModel(ImageFile imageFile)
        {
            _context.ImageFiles.Remove(imageFile);
        }

        private async Task<ImageFile> CreateImageFileModel(string fileName, string extension, string description)
        {
            var webPath = GetWebPath(fileName, extension);
            var fullPath = Path.Combine(_webRoot, webPath);
            using (var input = File.OpenRead(fullPath))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    await input.CopyToAsync(ms);
                    Image image = Image.Load(ms.ToArray());
                    return new ImageFile
                    {
                        Name = fileName,
                        Filepath = "/" + webPath,
                        Format = extension,
                        Width = image.Width,
                        Height = image.Height,
                        Date = DateTime.UtcNow,
                        Description = description ?? string.Empty
                    };
                }
            }
        }

        private string GetWebPath(string fileName, string extension)
        {
            return Path.Combine(_webFolder, String.Format("{0}.{1}", fileName, extension));
        }
    }
}
