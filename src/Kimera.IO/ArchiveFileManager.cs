using Kimera.IO.Entities;
using SevenZip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Kimera.IO
{
    public class ArchiveFileManager
    {
        private string _filePath = string.Empty;

        public event EventHandler<FileInfoEventArgs> FileExtractionStartedEvent;

        public event EventHandler<FileInfoEventArgs> FileExtractionFinishedEvent;

        public event EventHandler<EventArgs> ExtractionFinishedEvent;

        public event EventHandler<ProgressEventArgs> ExtractingEvent;

        public event EventHandler<FileOverwriteEventArgs> FileExistsEvent;

        public ArchiveFileManager(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("The archive file does not found.");
            }

            EnsureSevenZipLibrary();

            _filePath = filePath;
        }

        private static void EnsureSevenZipLibrary()
        {
            string filePath = Path.Combine(Environment.CurrentDirectory, "7z.dll");

            if (!File.Exists(filePath))
            {
                Assembly currentAssembly = Assembly.GetExecutingAssembly();

                if (Environment.Is64BitOperatingSystem)
                {
                    using (FileStream stream = new FileStream(filePath, FileMode.CreateNew))
                    {
                        currentAssembly.GetManifestResourceStream("Kimera.IO.Resources.x64.7z.dll").CopyTo(stream);
                    }
                }
                else
                {
                    using (FileStream stream = new FileStream(filePath, FileMode.CreateNew))
                    {
                        currentAssembly.GetManifestResourceStream("Kimera.IO.Resources.x86.7z.dll").CopyTo(stream);
                    }
                }
            }

            SevenZipBase.SetLibraryPath(filePath);
        }

        public InArchiveFormat GetArchiveType()
        {
            using (SevenZipExtractor extractor = new SevenZipExtractor(_filePath))
            {
                return extractor.Format;
            }
        }

        public List<string> GetEntryPoints(List<string> targetFileNames, string password = null)
        {
            try
            {
                if (targetFileNames == null || targetFileNames.Count == 0)
                {
                    targetFileNames = new List<string>();
                    targetFileNames.Add("Game.exe");
                    targetFileNames.Add("Start.exe");
                    targetFileNames.Add("Play.exe");
                }

                using (SevenZipExtractor extractor = new SevenZipExtractor(_filePath, password))
                {
                    List<string> result = new List<string>();

                    foreach (string file in extractor.ArchiveFileNames)
                    {
                        string fileName = Path.GetFileName(file);

                        foreach (string target in targetFileNames)
                        {
                            if (fileName.Equals(target, StringComparison.OrdinalIgnoreCase))
                            {
                                result.Add(file);
                            }
                        }
                    }

                    return result;
                }
            }
            catch
            {
                throw;
            }
        }

        public async Task<List<string>> GetEntryPointsAsync(List<string> targetFileNames, string password = null)
        {
            var task = Task.Factory.StartNew(() =>
            {
                return GetEntryPoints(targetFileNames, password);
            });

            return await task.ConfigureAwait(false);
        }

        public bool IsEncrypted()
        {
            try
            {
                using (SevenZipExtractor extractor = new SevenZipExtractor(_filePath))
                {
                    string fileName = extractor.ArchiveFileNames.FirstOrDefault();
                }

                return false;
            }
            catch (SevenZipArchiveException)
            {
                return true;
            }
        }

        public async Task<bool> IsEncryptedAsync()
        {
            var task = Task.Factory.StartNew(() =>
            {
                return IsEncrypted();
            });

            return await task.ConfigureAwait(false);
        }

        public bool IsValidPassword(string password)
        {
            try
            {
                using (SevenZipExtractor extractor = new SevenZipExtractor(_filePath, password))
                {
                    string fileName = extractor.ArchiveFileNames.FirstOrDefault();
                }

                return true;
            }
            catch (SevenZipArchiveException)
            {
                return false;
            }
        }

        public async Task<bool> IsValidPasswordAsync(string password)
        {
            var task = Task.Factory.StartNew(() =>
            {
                return IsValidPassword(password);
            });

            return await task.ConfigureAwait(false);
        }

        public void DecompressArchive(string targetDirectory, string password = null)
        {
            try
            {
                if (!Directory.Exists(targetDirectory))
                {
                    Directory.CreateDirectory(targetDirectory);
                }

                using (SevenZipExtractor extractor = new SevenZipExtractor(_filePath, password))
                {
                    extractor.PreserveDirectoryStructure = true;

                    extractor.EventSynchronization = EventSynchronizationStrategy.AlwaysSynchronous;
                    extractor.FileExtractionStarted += FileExtractionStartedEvent;
                    extractor.FileExtractionFinished += FileExtractionFinishedEvent;
                    extractor.ExtractionFinished += ExtractionFinishedEvent;
                    extractor.Extracting += ExtractingEvent;
                    extractor.FileExists += FileExistsEvent;

                    extractor.ExtractArchive(targetDirectory);
                }
            }
            catch
            {
                throw;
            }
        }

        public async Task DecompressArchiveAsync(string targetDirectory, string password = null)
        {
            try
            {
                if (!Directory.Exists(targetDirectory))
                {
                    Directory.CreateDirectory(targetDirectory);
                }

                using (SevenZipExtractor extractor = new SevenZipExtractor(_filePath, password))
                {
                    extractor.PreserveDirectoryStructure = true;

                    extractor.EventSynchronization = EventSynchronizationStrategy.AlwaysSynchronous;
                    extractor.FileExtractionStarted += FileExtractionStartedEvent;
                    extractor.FileExtractionFinished += FileExtractionFinishedEvent;
                    extractor.ExtractionFinished += ExtractionFinishedEvent;
                    extractor.Extracting += ExtractingEvent;
                    extractor.FileExists += FileExistsEvent;

                    await extractor.ExtractArchiveAsync(targetDirectory);
                }
            }
            catch
            {
                throw;
            }
        }
    }
}
