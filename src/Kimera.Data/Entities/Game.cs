using Kimera.Data.Enums;
using System;
using System.Collections.Generic;

namespace Kimera.Data.Entities
{
    public class Game
    {
        public int Id { get; set; }

        public Guid SystemId { get; set; }

        public Guid GameMetadata { get; set; }

        public Guid PackageMetadata { get; set; }

        public PackageStatus PackageStatus { get; set; }

        public virtual GameMetadata GameMetadataNavigation { get; set; }

        public virtual PackageMetadata PackageMetadataNavigation { get; set; }

        public virtual ICollection<CategorySubscription> CategorySubscription { get; set; } = new HashSet<CategorySubscription>();
    }
}
