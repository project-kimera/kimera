using Kimera.Data.Structs;
using System;
using System.Collections.Generic;

namespace Kimera.Data.Entities
{
    public class PackageMetadata
    {
        public int Id { get; set; }

        public Guid Game { get; set; }

        public Guid PackageManager { get; set; }

        public virtual Game GameNavigation { get; set; }

        public virtual Plugin PackageManagerNavigation { get; set; }

        public virtual ICollection<Setting> Settings { get; set; } = new HashSet<Setting>();
    }
}
