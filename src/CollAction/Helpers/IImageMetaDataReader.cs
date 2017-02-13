using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CollAction.Helpers
{
    public interface IImageMetaDataReader
    {
        void ReadFromStream(Stream stream);
        int Width { get; }
        int Height { get; }
    }
}
