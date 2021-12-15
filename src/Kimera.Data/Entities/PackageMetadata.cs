using Kimera.Data.Enums;
using System;
using System.Collections.Generic;

namespace Kimera.Data.Entities
{
    /// <summary>
    /// Stores the package metadata.
    /// </summary>
    public class PackageMetadata
    {
        /// <summary>
        /// The Primary Key(PK) of the package metadata.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The Principal Key(PK) of the package metadata, which used to interact with other entities.
        /// </summary>
        public Guid SystemId { get; set; }

        /// <summary>
        /// The Foreign Key(FK) of the game.
        /// </summary>
        public Guid Game { get; set; }

        /// <summary>
        /// The type of package.
        /// </summary>
        public PackageType Type { get; set; }

        /// <summary>
        /// The entry point file path of the package. It uses relative path.
        /// </summary>
        public string EntryPointFilePath { get; set; }

        /// <summary>
        /// The executable file path of the package.
        /// </summary>
        public string ExecutableFilePath { get; set; }

        /// <summary>
        /// The commandline used to start the game.
        /// </summary>
        public string CommandLineArguments { get; set; }

        /// <summary>
        /// The components used for the game.
        /// </summary>
        public virtual ICollection<Component> Components { get; set; } = new HashSet<Component>();

        /// <summary>
        /// The navigation property of the game.
        /// </summary>
        public virtual Game GameNavigation { get; set; }

        /// <summary>
        /// Creates a new instance of PackageMetadata.
        /// </summary>
        public PackageMetadata()
        {

        }

        /// <summary>
        /// Creates a new instance of PackageMetadata.
        /// </summary>
        /// <param name="type">The type of package.</param>
        /// <param name="entryPointFilePath">The entry point file path of the package. <para/>If the package type is not <see cref="PackageType.Executable"/>, it uses relative path.</param>
        /// <param name="commandLineArguments">The commandline used to start the game.</param>
        public PackageMetadata(PackageType type, string entryPointFilePath, string commandLineArguments)
        {
            SystemId = Guid.NewGuid();
            Type = type;
            EntryPointFilePath = entryPointFilePath;
            CommandLineArguments = commandLineArguments;
        }

        /// <summary>
        /// Copies and returns current package metadata.
        /// </summary>
        /// <returns>The copyed game metadata.</returns>
        public PackageMetadata Copy()
        {
            return (PackageMetadata)this.MemberwiseClone();
        }
    }
}
