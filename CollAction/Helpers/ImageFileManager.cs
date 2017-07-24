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

        public async Task<ImageFile> CreateOrReplaceImageFileIfNeeded(ImageFile imageFile, IFormFile fileToUpload, string imageDescription)
        {
            ImageFile outputImageFile = imageFile;

            if (ShouldCreateOrReplaceImageFile(imageFile, fileToUpload))
            {
                DeleteImageFileIfExists(imageFile);
                outputImageFile = await CreateImageFileWithUniqueName(fileToUpload);
            }

            if (ShouldUpdateImageFileDescription(outputImageFile, imageDescription))
            {
                outputImageFile.Description = imageDescription ?? "";
            }

            await _context.SaveChangesAsync(); // Need to save to the database to get an ID for the new ImageFile.

            return outputImageFile;
        }

        private bool ShouldCreateOrReplaceImageFile(ImageFile imageFile, IFormFile formFileToUpload)
        {
            return ShouldCreateImageFile(imageFile, formFileToUpload) || ShouldReplaceImageFile(imageFile, formFileToUpload);
        }

        private bool ShouldCreateImageFile(ImageFile imageFile, IFormFile formFileToUpload)
        {
            return imageFile == null && formFileToUpload != null;
        }

        private bool ShouldReplaceImageFile(ImageFile imageFile, IFormFile formFileToUpload)
        {
            return imageFile != null && formFileToUpload != null;
        }

        private bool ShouldUpdateImageFileDescription(ImageFile imageFile, string imageDescription)
        {
            string nonNullImageDescription = imageDescription != null ? imageDescription : "";
            return imageFile != null && imageFile.Description != nonNullImageDescription;
        }

        private async Task<ImageFile> CreateImageFileWithUniqueName(IFormFile formFileToUpload)
        {
            string filename = GetUniqueFileName();
            string extension = GetFormFileExtension(formFileToUpload);
            await SaveFormFileToFileSystem(formFileToUpload, filename, extension);
            ImageFile imageFile = await CreateImageFile(filename, extension);
            SaveImageFileToModel(imageFile);
            return imageFile;
        }

        private string GetUniqueFileName()
        {
            return Guid.NewGuid().ToString();
        }

        private string GetFormFileExtension(IFormFile formFile)
        {
            return Path.GetExtension(formFile.FileName).ToLower().Substring(1); // Strip the "."
        }

        private async Task<ImageFile> ReplaceImageFile(ImageFile imageFile, IFormFile formFileToUpload)
        {
            DeleteImageFileIfExists(imageFile);
            return await CreateImageFileWithUniqueName(formFileToUpload);
        }

        private void DeleteImageFileIfExists(ImageFile imageFile)
        {
            if (imageFile == null)
            {
                return;
            }

            DeleteImageFileFromFileSystem(imageFile);
            DeleteImageFileFromModel(imageFile);
        }

        private async Task SaveFormFileToFileSystem(IFormFile formFile, string filename, string extension)
        {
            var webPath = GetWebPath(filename, extension);
            var absolutePath = Path.Combine(_webRoot, webPath);
            Directory.CreateDirectory(Path.GetDirectoryName(absolutePath));
            using (var output = new FileStream(absolutePath, FileMode.Create))
            {
                await formFile.CopyToAsync(output);
            }
        }

        private void DeleteImageFileFromFileSystem(ImageFile imageFile)
        {
            var absolutePath = GetImageFileAbsolutePath(imageFile);
            var fileInfo = new FileInfo(absolutePath);
            if (fileInfo != null)
            {
                fileInfo.Delete();
            }
        }

        private string GetImageFileAbsolutePath(ImageFile imageFile)
        {
            string relativePath = TrimInitialFilePathDirectorySeparator(imageFile.Filepath);
            string result = Path.Combine(_webRoot, relativePath);
            return result;
        }

        private string TrimInitialFilePathDirectorySeparator(string filepath)
        {
            return filepath.TrimStart(new char[] { '/' });
        }

        private void SaveImageFileToModel(ImageFile imageFile)
        {
            _context.ImageFiles.Add(imageFile);
        }

        private void DeleteImageFileFromModel(ImageFile imageFile)
        {
            _context.ImageFiles.Remove(imageFile);
        }

        private async Task<ImageFile> CreateImageFile(string filename, string extension)
        {
            string webPath = GetWebPath(filename, extension);
            string absolutePath = Path.Combine(_webRoot, webPath);
            string filepath = '/' + webPath;
            using (var input = File.OpenRead(absolutePath))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    await input.CopyToAsync(ms);
                    Image image = Image.Load(ms.ToArray());
                    return new ImageFile
                    {
                        Name = filename,
                        Filepath = filepath,
                        Format = extension,
                        Width = image.Width,
                        Height = image.Height,
                        Date = DateTime.UtcNow,
                        Description = string.Empty
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
