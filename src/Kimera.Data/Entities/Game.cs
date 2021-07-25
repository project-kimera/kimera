﻿using Kimera.Data.Enums;
using System;
using System.Collections.Generic;

namespace Kimera.Data.Entities
{
    /// <summary>
    /// Stores the information of the game.
    /// </summary>
    public class Game
    {
        /// <summary>
        /// The Primary Key(PK) of the game.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The Principal Key(PK) of the game, which used to interact with other entities.
        /// </summary>
        public Guid SystemId { get; set; }

        /// <summary>
        /// The Foreign Key(FK) of the game metadata.
        /// </summary>
        public Guid GameMetadata { get; set; }

        /// <summary>
        /// The Foreign Key(FK) of the package metadata.
        /// </summary>
        public Guid PackageMetadata { get; set; }

        /// <summary>
        /// The package status.
        /// </summary>
        public PackageStatus PackageStatus { get; set; }

        /// <summary>
        /// The navigation property of the game metadata.
        /// </summary>
        public virtual GameMetadata GameMetadataNavigation { get; set; }

        /// <summary>
        /// The navigation property of the package metadata.
        /// </summary>
        public virtual PackageMetadata PackageMetadataNavigation { get; set; }

        /// <summary>
        /// The collection of <see cref="CategorySubscription"/>, which used to check the subscription status of category and game.
        /// </summary>
        public virtual ICollection<CategorySubscription> CategorySubscriptions { get; set; } = new HashSet<CategorySubscription>();
    }
}
