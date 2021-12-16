using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kimera.IO
{
    /// <summary>
    /// Provides functions related to text I/O.
    /// </summary>
    public static class TextManager
    {
        /// <summary>
        /// Writes a text file in the specified path.
        /// </summary>
        /// <param name="filePath">Path where the text file will be saved.</param>
        /// <param name="text">Text to be writed.</param>
        /// <param name="encoding">The text encoding to use.</param>
        public static void WriteTextFile(string filePath, string text, Encoding encoding)
        {
            if (!Directory.Exists(Path.GetDirectoryName(filePath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            }

            using (Stream stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                using (StreamWriter writer = new StreamWriter(stream, encoding))
                {
                    writer.Write(text);
                }
            }
        }

        /// <summary>
        /// Reads a text file in the specified path.
        /// </summary>
        /// <param name="filePath">Path where the text file will be read.</param>
        /// <param name="encoding">The text encoding to use.</param>
        /// <returns>Text</returns>
        public static string ReadTextFile(string filePath, Encoding encoding)
        {
            string temp = string.Empty;

            using (StreamReader reader = new StreamReader(filePath, encoding))
            {
                temp = reader.ReadToEnd();
            }

            return temp;
        }

        /// <summary>
        /// Determines a text file's encoding by analyzing its byte order mark (BOM).
        /// Defaults to ASCII when detection of the text file's endianness fails.
        /// </summary>
        /// <param name="filePath">The text file to analyze.</param>
        /// <returns>The detected encoding.</returns>
        public static Encoding GetEncoding(string filePath)
        {
            // Read the BOM
            var bom = new byte[4];

            using (var file = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                file.Read(bom, 0, 4);
            }

            // Analyze the BOM
            if (bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf)
            {
                return Encoding.UTF8;
            }
            if (bom[0] == 0xff && bom[1] == 0xfe && bom[2] == 0 && bom[3] == 0)
            {
                return Encoding.UTF32; //UTF-32LE
            }
            if (bom[0] == 0 && bom[1] == 0 && bom[2] == 0xfe && bom[3] == 0xff)
            {
                return new UTF32Encoding(true, true);  //UTF-32BE
            }
            if (bom[0] == 0xff && bom[1] == 0xfe)
            {
                return Encoding.Unicode; //UTF-16LE
            }
            if (bom[0] == 0xfe && bom[1] == 0xff)
            {
                return Encoding.BigEndianUnicode; //UTF-16BE
            }

            // We actually have no idea what the encoding is if we reach this point, so
            // you may wish to return null instead of defaulting to ASCII
            return Encoding.ASCII;
        }

        /// <summary>
        /// Remove the BOM from a text.
        /// </summary>
        /// <param name="text">A text to remove BOM.</param>
        /// <returns>BOM-removed text</returns>
        public static string RemoveBOM(string text)
        {
            string UTF8ByteOrderMark = Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble());

            if (text.StartsWith(UTF8ByteOrderMark, StringComparison.OrdinalIgnoreCase))
                text = text.Remove(0, UTF8ByteOrderMark.Length);

            return text.Replace("\0", "");
        }

        private static void Resize(ref string[] array)
        {
            int i = array.Length;
            Array.Resize(ref array, i + 1);
            array[i] = null;
        }

        /// <summary>
        /// Parse a text in the middle of start and end.
        /// </summary>
        /// <param name="text">A text to parse.</param>
        /// <param name="start">Start</param>
        /// <param name="end">End</param>
        /// <returns>Result</returns>
        public static string ParseText(this string text, string start, string end)
        {
            string result = string.Empty;
            result = text.Substring(text.IndexOf(start) + start.Length);
            result = result.Substring(0, result.IndexOf(end));
            return result;
        }

        /// <summary>
        /// Parse texts in the middle of start and end.
        /// </summary>
        /// <param name="text">A text to parse.</param>
        /// <param name="start">Start</param>
        /// <param name="end">End</param>
        /// <returns>Result</returns>
        public static string[] ParseTexts(this string text, string start, string end)
        {
            string source = text;
            string[] result = { null };
            int Count = 0;

            while (source.IndexOf(start) > -1)
            {
                Resize(ref result);

                source = source.Substring(source.IndexOf(start) + start.Length);

                if (source.IndexOf(end) != -1)
                {
                    result[Count] = source.Substring(0, source.IndexOf(end));
                }
                else
                {
                    return result;
                }

                Count++;
            }
            return result;
        }
    }
}
