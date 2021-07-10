using Kimera.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kimera.Data.Contexts
{
    public partial class KimeraContext : DbContext
    {
        public virtual DbSet<Category> Category { get; set; }

        public virtual DbSet<Game> Game { get; set; }

        public virtual DbSet<GameMetadata> GameMetadata { get; set; }

        public virtual DbSet<PackageMetadata> PackageMetadata { get; set; }

        public virtual DbSet<Plugin> Plugin { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }
    }
}
