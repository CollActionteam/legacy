using CollAction.Models;
using System;
using System.IO;

namespace CollAction.Helpers
{
    abstract public class IImageFileManager : IFileManager
    {
        public IImageMetaDataReader ImageMetaDataReader { get; set; }

        protected ImageFile CreateImageFileModel(string webPath)
        {
            ImageFile imageFile = null;
            var fileName = Path.GetFileName(webPath);
            var extension = Path.GetExtension(webPath).ToLower().Substring(1); // Strip the dot in ".<extension>".
            var fullPath = Path.Combine(WebRoot, webPath);

            using (var input = File.OpenRead(fullPath))
            {
                ImageMetaDataReader.ReadFromStream(input);
                imageFile = new ImageFile
                {
                    Name = fileName,
                    Filepath = String.Format("\\{0}", webPath),
                    Format = extension,
                    Width = ImageMetaDataReader.Width,
                    Height = ImageMetaDataReader.Height,
                    Date = DateTime.Now
                };
            }

            return imageFile;
        }
    }
}
