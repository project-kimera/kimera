using Kimera.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kimera.Data.Entities
{
    public class Component
    {
        public int Id { get; set; }

        public Guid PackageMetadata { get; set; }

        public ComponentType Type { get; set; }

        public int Index { get; set; }

        public string FilePath { get; set; }

        public string Password { get; set; }

        public virtual PackageMetadata PackageMetadataNavigation { get; set; }
    }
}
