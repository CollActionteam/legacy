using CollAction.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CollAction.Helpers
{
    public class ProjectBannerImageManager : IImageFileManager
    {
        public Project Project { get; set; }

        private string WebFolder { get { return Path.Combine("usercontent", "bannerimages"); } }

        // Don't trust the users _formFile.FileName to be unique, so generate our own unique one "banner_<Project.Id>.<FileType>"
        private string FileName
        {
            get
            {
                var regex = new Regex(@"[^A-Za-z0-9]+");
                var uniqueName = regex.Replace(Project.Name, "_").Trim(new char[] { '_' });
                return String.Format("banner_{0}{1}", uniqueName, Path.GetExtension(_formFile.FileName).ToLower());
            }
        }
        
        private string WebPath { get { return Path.Combine(WebFolder, FileName); } }

        protected override void SaveFileReferenceToModel()
        {
            Project.BannerImage = CreateImageFileModel(WebPath);
        }

        protected override async Task SaveFileToFileSystem()
        {
            // Save the uploaded image to the file system.
            var fullPath = Path.Combine(WebRoot, WebPath);
            using (var output = new FileStream(fullPath, FileMode.Create))
            {
                await _formFile.CopyToAsync(output);
            }
        }

        protected override void DeleteFileFromFileSystem()
        {
            // Delete the Project's old BannerImage from the file system.
            var fullPath = Path.Combine(WebRoot, Project.BannerImage.Filepath.TrimStart(new char[] { '\\' }));
            var fileInfo = new FileInfo(fullPath);
            if (fileInfo != null)
            {
                fileInfo.Delete();
            }
        }

        protected override void DeleteFileReferenceFromModel()
        {
            Context.ImageFiles.Remove(Project.BannerImage);
            Project.BannerImage = null;
        }
        
        protected override bool ModelHasFileReference()
        {
            return Project.BannerImage != null;
        }
    }
}
