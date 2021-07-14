using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kimera.Data.Structs
{
    public struct IndexedFilePath
    {
        public int Index { get; set; }

        public string FilePath { get; set; }

        public IndexedFilePath(int index, string filePath)
        {
            Index = index;
            FilePath = filePath;
        }
    }
}
