using Kimera.IO.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kimera.IO
{
    /// <summary>
    /// Provides functions related to file indexing.
    /// </summary>
    public static class IndexingHelper
    {
        /// <summary>
        /// Index all files that exist within the directory.
        /// </summary>
        /// <param name="targetPath">Directory to index.</param>
        /// <param name="extensions">The extensions of the files to index. If the value is null, index all files.</param>
        /// <param name="cancellationToken">A token to cancel task.</param>
        /// <returns>Indexed file list</returns>
        public static List<string> GetFiles(string targetPath, List<string> extensions, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(targetPath))
            {
                throw new ArgumentNullException("Directory cannot be null or empty.");
            }

            if (Directory.GetDirectoryRoot(targetPath).ToLower() == targetPath.ToLower())
            {
                throw new InvalidOperationException("The root directory cannot be a target path.");
            }

            try
            {
                List<string> files = new List<string>();
                DirectoryInfo dir = new DirectoryInfo(targetPath);

                // Index files in current directory.
                foreach (FileInfo file in dir.GetFiles())
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return null;
                    }

                    if (file.IsReadOnly)
                    {
                        continue;
                    }

                    if (extensions == null)
                    {
                        files.Add(file.FullName);
                    }
                    else
                    {
                        if (extensions.Contains(file.Extension.ToLower()))
                        {
                            files.Add(file.FullName);
                        }
                    }
                }

                // Re-index files in sub-directory.
                foreach (DirectoryInfo subdir in dir.GetDirectories())
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return null;
                    }

                    files.AddRange(GetFiles(subdir.FullName, extensions, cancellationToken));
                }

                return files;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Index all files that exist within the directory.
        /// </summary>
        /// <param name="targetPath">Directory to index.</param>
        /// <param name="extensions">The extensions of the files to index. If the value is null, index all files.</param>
        /// <param name="cancellationToken">A token to cancel task.</param>
        /// <returns>Indexed file list</returns>
        public static async Task<List<string>> GetFilesAsync(string targetPath, List<string> extensions, CancellationToken cancellationToken)
        {
            var task = Task.Factory.StartNew(() =>
            {
                return GetFiles(targetPath, extensions, cancellationToken);
            });

            return await task.ConfigureAwait(false);
        }

        /// <summary>
        /// Returns a directory item made with a file info.
        /// </summary>
        /// <param name="file">The file info.</param>
        /// <returns>The directory item.</returns>
        public static DirectoryItem GetFileItem(FileInfo file)
        {
            DirectoryItem item = new DirectoryItem();

            item.Type = DirectoryItemType.File;
            item.Name = file.Name;
            item.FullName = file.FullName;
            item.IsExpanded = false;
            item.IsSelected = false;
            item.SubItems = new List<DirectoryItem>();

            return item;
        }

        /// <summary>
        /// Returns a directory item made with a directory info.
        /// </summary>
        /// <param name="directory">The directory info to get information.</param>
        /// <param name="isExpanded">Whether the item is expanded or not.</param>
        /// <returns>The directory item.</returns>
        public static DirectoryItem GetFolderItem(DirectoryInfo directory, bool isExpanded = false)
        {
            DirectoryItem item = new DirectoryItem();

            item.Type = DirectoryItemType.Folder;
            item.Name = directory.Name;
            item.FullName = directory.FullName;
            item.IsExpanded = isExpanded;
            item.IsSelected = false;

            List<DirectoryItem> items = new List<DirectoryItem>();

            try
            {
                var subDirectories = directory.EnumerateDirectories();

                foreach (DirectoryInfo subDirectory in subDirectories)
                {
                    items.Add(GetFolderItem(subDirectory, false));
                }
            }
            catch { }

            try
            {
                var files = directory.EnumerateFiles();

                foreach (FileInfo file in files)
                {
                    items.Add(GetFileItem(file));
                }
            }
            catch { }

            item.SubItems = items;

            return item;
        }

        /// <summary>
        /// Gets the string for the file size.
        /// </summary>
        /// <param name="length">The size of file(in byte).</param>
        /// <returns>The file size string.</returns>
        public static string GetFileSizeString(long length)
        {
            double byteCount = length;

            string size = "0 Byte";

            if (byteCount >= 1099511627776.0)
                size = string.Format("{0:##.##}", byteCount / 1099511627776.0) + " TB";
            else if (byteCount >= 1073741824.0)
                size = string.Format("{0:##.##}", byteCount / 1073741824.0) + " GB";
            else if (byteCount >= 1048576.0)
                size = string.Format("{0:##.##}", byteCount / 1048576.0) + " MB";
            else if (byteCount >= 1024.0)
                size = string.Format("{0:##.##}", byteCount / 1024.0) + " KB";
            else if (byteCount > 0 && byteCount < 1024.0)
                size = byteCount.ToString() + " Byte";

            return size;
        }
    }
}
