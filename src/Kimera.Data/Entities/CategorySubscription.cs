using System;

namespace Kimera.Data.Entities
{
    public class CategorySubscription
    {
        public int Id { get; set; }

        public Guid Category { get; set; }

        public Guid Game { get; set; }

        public virtual Category CategoryNavigation { get; set; }

        public virtual Game GameNavigation { get; set; }

    }
}
