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
        private IFormFile _formFile = null;
        private ImageFile _imageFile = null;
        private string _fileName = "";

        public ApplicationDbContext Context { get; set; }

        public string WebRoot { get; set; }

        public string WebFolder { get; set; }
        
        private string FileName { get { return String.Format("{0}{1}", _fileName, Path.GetExtension(_formFile.FileName).ToLower()); } }
        
        private string WebPath { get { return Path.Combine(WebFolder, FileName); } }

        public async Task<ImageFile> UploadFormFile(IFormFile formFile, string fileName)
        {
            if (formFile == null) { return null; }

            _formFile = formFile;
            _fileName = fileName;

            await SaveFileToFileSystem();

            return CreateImageFileModel();
        }

        private async Task SaveFileToFileSystem()
        {
            // Save the uploaded image to the file system.
            var fullPath = Path.Combine(WebRoot, WebPath);
            using (var output = new FileStream(fullPath, FileMode.Create))
            {
                await _formFile.CopyToAsync(output);
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
            var fullPath = Path.Combine(WebRoot, imageFile.Filepath.TrimStart(new char[] { '\\' }));
            var fileInfo = new FileInfo(fullPath);
            if (fileInfo != null)
            {
                fileInfo.Delete();
            }
        }

        private void DeleteImageFileFromModel(ImageFile imageFile)
        {
            Context.ImageFiles.Remove(imageFile);
        }

        private ImageFile CreateImageFileModel()
        {
            ImageFile imageFile = null;
            var fileName = Path.GetFileName(WebPath);
            var extension = Path.GetExtension(WebPath).ToLower().Substring(1); // Strip the dot in ".<extension>".
            var fullPath = Path.Combine(WebRoot, WebPath);

            using (var input = File.OpenRead(fullPath))
            {
                var image = Image.FromStream(input);
                imageFile = new ImageFile
                {
                    Name = fileName,
                    Filepath = String.Format("\\{0}", WebPath),
                    Format = extension,
                    Width = image.Width,
                    Height = image.Height,
                    Date = DateTime.Now
                };
            }

            return imageFile;
        }
    }
}
