using System;

namespace Kimera.Data.Entities
{
    /// <summary>
    /// Stores the subscription status of the category and the game.
    /// </summary>
    public class CategorySubscription
    {
        /// <summary>
        /// The Primary Key(PK) of the category subscription.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The Foreign Key(FK) of the category.
        /// </summary>
        public Guid Category { get; set; }

        /// <summary>
        /// The Foreign Key(FK) of the game.
        /// </summary>
        public Guid Game { get; set; }

        /// <summary>
        /// The navigation property of the category.
        /// </summary>
        public virtual Category CategoryNavigation { get; set; }

        /// <summary>
        /// The navigation property of the game.
        /// </summary>
        public virtual Game GameNavigation { get; set; }

    }
}
