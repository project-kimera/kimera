using System.Collections.Generic;

namespace Kimera.IO.Entities
{
    public class DirectoryItem
    {
        public DirectoryItemType Type { get; set; }

        public string Name { get; set; }

        public string FullName { get; set; }

        public bool IsExpanded { get; set; }

        public bool IsSelected { get; set; }

        public List<DirectoryItem> SubItems { get; set; }

        public DirectoryItem()
        {
            SubItems = new List<DirectoryItem>();
        }
    }
}
