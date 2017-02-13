using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CollAction.Helpers
{
    // System.Drawing.Image implementation of the IImageMetaDataReader interface.
    public class SDImageMetaDataReader : IImageMetaDataReader
    {
        Image _image = null;

        public void ReadFromStream(Stream stream) { _image = Image.FromStream(stream); }

        public int Height { get { return _image.Height; } }

        public int Width { get { return _image.Width; } }
    }
}
