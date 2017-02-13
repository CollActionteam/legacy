using CollAction.Data;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CollAction.Helpers
{
    // Interface for a FileManager that keeps the model consistent with the file system when uploading IFormFiles.
    abstract public class IFileManager
    {
        protected IFormFile _formFile = null;

        public ApplicationDbContext Context { get; set; }

        public string WebRoot { get; set; }

        abstract protected Task SaveFileToFileSystem();
        abstract protected void SaveFileReferenceToModel();
        abstract protected void DeleteFileFromFileSystem();
        abstract protected void DeleteFileReferenceFromModel();
        abstract protected bool ModelHasFileReference();

        public async Task UploadFormFile(IFormFile formFile)
        {
            if (formFile == null) { return; }

            _formFile = formFile;

            // Clear prexisting file.
            if (ModelHasFileReference())
            {
                DeleteFileFromFileSystem();
                DeleteFileReferenceFromModel();
            }

            // Save new file.
            await SaveFileToFileSystem();
            SaveFileReferenceToModel();
        }
    }
}
