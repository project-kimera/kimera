using System;
using System.Collections.Generic;

namespace Kimera.Data.Entities
{
    /// <summary>
    /// Category entity to store games. It has 1:N relationship.
    /// </summary>
    public class Category
    {
        public int Id { get; set; }

        public Guid SystemId { get; set; }

        public string Name { get; set; }

        public virtual ICollection<CategorySubscription> CategorySubscription { get; set; } = new HashSet<CategorySubscription>();
    }
}
