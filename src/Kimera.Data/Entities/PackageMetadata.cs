using Kimera.Data.Structs;
using System;
using System.Collections.Generic;

namespace Kimera.Data.Entities
{
    public class PackageMetadata
    {
        public int Id { get; set; }

        public Guid Game { get; set; }

        public ICollection<IndexedFilePath> Components { get; set; } = new HashSet<IndexedFilePath>();

        public string EntryPointFilePath { get; set; }

        public string Arguments { get; set; }

        public virtual Game GameNavigation { get; set; }
    }
}
