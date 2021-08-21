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
        /// The priority of the component.
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// The path of the component.
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// The relative path where the component will be copied.
        /// </summary>
        public string OffsetPath { get; set; }

        /// <summary>
        /// The password of the component.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// The navigation property of the package metadata.
        /// </summary>
        public virtual PackageMetadata PackageMetadataNavigation { get; set; }

        /// <summary>
        /// Creates a new instance of Component.
        /// </summary>
        public Component()
        {

        }

        /// <summary>
        /// Creates a new instance of Component.
        /// </summary>
        /// <param name="type">The type of the component.</param>
        /// <param name="index">The priority of the component.</param>
        /// <param name="filePath">The path of the component.</param>
        public Component(int index, string filePath)
        {
            Index = index;
            FilePath = filePath;
        }

        /// <summary>
        /// Creates a new instance of Component.
        /// </summary>
        /// <param name="type">The type of the component.</param>
        /// <param name="index">The priority of the component.</param>
        /// <param name="filePath">The path of the component.</param>
        /// <param name="password">The password of the component.</param>
        public Component(int index, string filePath, string password)
        {
            Index = index;
            FilePath = filePath;
            Password = password;
        }
    }
}
