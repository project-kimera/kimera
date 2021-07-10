using Kimera.Data.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kimera.Data.Entities
{
    public class PackageMetadata
    {
        public Guid Id { get; set; }

        public Guid PackageManagerId { get; set; }

        public virtual ICollection<Setting> Settings { get; set; } = new List<Setting>();
    }
}
