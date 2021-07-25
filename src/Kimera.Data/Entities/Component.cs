using Kimera.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kimera.Data.Entities
{
    /// <summary>
    /// Stores the information of the component.
    /// </summary>
    public class Component
    {
        /// <summary>
        /// The Primary Key(PK) of the component.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The Foreign Key(FK) of the package metadata.
        /// </summary>
        public Guid PackageMetadata { get; set; }

        /// <summary>
        /// The type of the component.
        /// </summary>
        public ComponentType Type { get; set; }
        
        /// <summary>
        /// The priority of the component.
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// The path of the component.
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// The password of the component.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// The navigation property of the package metadata.
        /// </summary>
        public virtual PackageMetadata PackageMetadataNavigation { get; set; }
    }
}
