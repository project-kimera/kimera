using System;
using System.Collections.Generic;

namespace Kimera.Data.Entities
{
    /// <summary>
    /// Category entity to store games. It has 1:N relationship.
    /// </summary>
    public class Category
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public virtual ICollection<Guid> Games { get; private set; } = new List<Guid>();

        public Category(Guid id, string name)
        {
            Id = id;
            Name = name;
        }

        public Category(Guid id, Guid parentId, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
