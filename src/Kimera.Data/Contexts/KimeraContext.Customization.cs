using Microsoft.EntityFrameworkCore;

namespace Kimera.Data.Contexts
{
    public partial class KimeraContext
    {
        private readonly DbContextOptions _options;

        public KimeraContext()
        {

        }

        public KimeraContext(DbContextOptions options) : base(options)
        {
            _options = options;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (_options == null)
            {
                optionsBuilder.UseSqlite("FileName=Kimera.db");
            }
        }
    }
}
