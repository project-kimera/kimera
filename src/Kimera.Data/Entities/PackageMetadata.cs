using Kimera.Data.Enums;
using System;
using System.Collections.Generic;

namespace Kimera.Data.Entities
{
    public class PackageMetadata
    {
        public int Id { get; set; }

        public Guid SystemId { get; set; }

        public Guid Game { get; set; }

        public PackageType Type { get; set; }

        public string EntryPointFilePath { get; set; }

        public string Arguments { get; set; }

        public virtual ICollection<Component> Components { get; set; } = new HashSet<Component>();

        public virtual Game GameNavigation { get; set; }
    }
}
