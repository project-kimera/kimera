using Kimera.IO.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kimera.IO
{
    public static class ArchiveFileManager
    {
        /*
        private static readonly string[] _normalZipSignature = new string[] { "50", "4B", "03", "04" };

        private static readonly string[] _emptyZipSignature = new string[] { "50", "4B", "05", "06" };

        private static readonly string[] _spannedZipSignature = new string[] { "50", "4B", "07", "08" };

        private static readonly string[] _sevenZipSignature = new string[] { "37", "7A", "BC", "AF", "27", "1C" };

        private static readonly string[] _roshalArchive150Signature = new string[] { "52", "61", "72", "21", "1A", "07", "00" };

        private static readonly string[] _roshalArchive500Signature = new string[] { "52", "61", "72", "21", "1A", "07", "01", "00" };
        */

        private static readonly byte[] _normalZipSignature = new byte[] { 80, 75, 3, 4 };

        private static readonly byte[] _emptyZipSignature = new byte[] { 80, 75, 5, 6 };

        private static readonly byte[] _spannedZipSignature = new byte[] { 80, 75, 7, 8 };

        private static readonly byte[] _sevenZipSignature = new byte[] { 55, 122, 188, 175, 39, 28 };

        private static readonly byte[] _roshalArchive150Signature = new byte[] { 82, 97, 114, 33, 26, 7, 0};

        private static readonly byte[] _roshalArchive500Signature = new byte[] { 82, 97, 114, 33, 26, 7, 1, 0 };

        public static ArchiveType GetArchiveType(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException("The file to get type does not exist.");
                }

                using (Stream stream = File.OpenRead(filePath))
                {
                    // Get header from the file.
                    byte[] buffer = new byte[10];
                    stream.Read(buffer, 0, 10);

                    byte[] header4 = new byte[4];
                    byte[] header6 = new byte[6];
                    byte[] header7 = new byte[7];
                    byte[] header8 = new byte[8];

                    Array.Copy(buffer, 0, header4, 0, 4);
                    Array.Copy(buffer, 0, header6, 0, 6);
                    Array.Copy(buffer, 0, header7, 0, 7);
                    Array.Copy(buffer, 0, header8, 0, 8);

                    // Check the header with signature arrays.
                    if (header4.SequenceEqual(_normalZipSignature) || header4.SequenceEqual(_emptyZipSignature) || header4.SequenceEqual(_spannedZipSignature))
                    {
                        return ArchiveType.Zip;
                    }
                    else if (header6.SequenceEqual(_sevenZipSignature))
                    {
                        return ArchiveType.SevenZip;
                    }
                    else if (header7.SequenceEqual(_roshalArchive150Signature) || header8.SequenceEqual(_roshalArchive500Signature))
                    {
                        return ArchiveType.RoshalArchive;
                    }
                    else
                    {
                        return ArchiveType.None;
                    }
                }
            }
            catch
            {
                throw;
            }
        }


    }
}
