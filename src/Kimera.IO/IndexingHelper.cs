using Kimera.IO.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
        /// <returns>Indexed file list</returns>
        public static List<string> GetFiles(string targetPath, List<string> extensions = null)
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
                    files.AddRange(GetFiles(subdir.FullName, extensions));
                }

                return files;
            }
            catch (Exception)
            {
                throw;
            }
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
    }
}
