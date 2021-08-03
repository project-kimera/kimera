using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Windows.Input;

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

        /// <summary>
        /// The command to change the selected category.
        /// </summary>
        [NotMapped]
        public ICommand ChangeCategoryCommand { get; set; }

        /// <summary>
        /// Creates a new instance of Category.
        /// </summary>
        public Category()
        {

        }

        /// <summary>
        /// Creates a new instance of Category.
        /// </summary>
        /// <param name="name"></param>
        public Category(string name)
        {
            SystemId = Guid.NewGuid();
            Name = name;
        }

        /// <summary>
        /// Creates a new instance of Category.
        /// </summary>
        /// <param name="name"></param>
        public Category(Guid guid, string name)
        {
            SystemId = guid;
            Name = name;
        }
    }
}
