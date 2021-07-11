using System;

namespace Kimera.Data.Entities
{
    public class Plugin
    {
        public int Id { get; set; }

        public Guid SystemId { get; set; }

        public Guid PackageMetadata { get; set; }

        public string Name { get; set; }

        public string Author { get; set; }

        public string Copyright { get; set; }

        public Version Version { get; set; }

        public virtual PackageMetadata PackageMetadataNavigation { get; set; }
    }
}
