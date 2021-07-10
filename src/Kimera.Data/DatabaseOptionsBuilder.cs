using Microsoft.EntityFrameworkCore;

namespace Kimera.Data
{
    internal class DatabaseOptionsBuilder
    {
        private static DbContextOptionsBuilder _builder;

        public DbContextOptionsBuilder Builder => _builder;

        public DatabaseOptionsBuilder()
        {
            _builder = new DbContextOptionsBuilder();
        }

        public DatabaseOptionsBuilder(DbContextOptions options)
        {
            _builder = new DbContextOptionsBuilder(options);
        }

        public DbContextOptions Build()
        {
            return _builder.IsConfigured ? _builder.Options : null;
        }

        public static implicit operator DbContextOptionsBuilder(DatabaseOptionsBuilder builder)
        {
            return builder.Builder;
        }

        public static implicit operator DatabaseOptionsBuilder(DbContextOptionsBuilder builder)
        {
            return new DatabaseOptionsBuilder(builder.Options);
        }
    }
}
