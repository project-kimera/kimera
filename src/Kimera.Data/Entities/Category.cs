using System;
using System.Collections.Generic;

namespace Kimera.Data.Entities
{
    /// <summary>
    /// Stores the information of the category.
    /// </summary>
    public class Category
    {
        /// <summary>
        /// The Primary Key(PK) of the category.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The Principal Key(PK) of the category, which used to interact with other entities.
        /// </summary>
        public Guid SystemId { get; set; }

        /// <summary>
        /// The name of the category.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The collection of <see cref="CategorySubscription"/>, which used to check the subscription status of category and game.
        /// </summary>
        public virtual ICollection<CategorySubscription> CategorySubscriptions { get; set; } = new HashSet<CategorySubscription>();
    }
}
