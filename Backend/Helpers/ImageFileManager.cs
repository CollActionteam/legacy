using CollAction.Data;
using CollAction.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
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

        public async Task<ImageFile> UploadFormFile(IFormFile formFile, string fileName)
        {
            if (formFile == null) { return null; }
            string extension = Path.GetExtension(formFile.FileName).ToLower().Substring(1); // Strip the "."
            await SaveFileToFileSystem(formFile, fileName, extension);
            return CreateImageFileModel(fileName, extension);
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

        private ImageFile CreateImageFileModel(string fileName, string extension)
        {
            ImageFile imageFile = null;
            var webPath = GetWebPath(fileName, extension);
            var fullPath = Path.Combine(_webRoot, webPath);
            using (var input = File.OpenRead(fullPath))
            {
                var image = Image.FromStream(input);
                imageFile = new ImageFile
                {
                    Name = fileName,
                    Filepath = "\\" + webPath,
                    Format = extension,
                    Width = image.Width,
                    Height = image.Height,
                    Date = DateTime.Now
                };
            }
            return imageFile;
        }

        private string GetWebPath(string fileName, string extension)
        {
            return Path.Combine(_webFolder, String.Format("{0}.{1}", fileName, extension));
        }

    }
}
