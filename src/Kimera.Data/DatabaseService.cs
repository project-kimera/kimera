using Kimera.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kimera.Data
{
    public class DatabaseService : IDisposable
    {
        private KimeraContext _data;
        private DbContextOptions _lastOptions;

        /// <summary>
        /// Gets an instance of the data service.
        /// </summary>
        public KimeraContext Service
        {
            get
            {
                if (_data != null)
                {
                    return _data;
                }

                _lastOptions = _lastOptions ?? new DatabaseOptionsBuilder().Build();
                _data = new KimeraContext(_lastOptions);

                return _data;
            }
        }

        public DatabaseService() { }

        public DatabaseService(DatabaseOptionsBuilder builder)
        {
            _lastOptions = builder.Build();
        }

        public DatabaseOptionsBuilder Configure()
        {
            return new DatabaseOptionsBuilder();
        }

        public void UseConfiguration(DatabaseOptionsBuilder builder)
        {
            _data?.Dispose();

            _data = new KimeraContext(builder.Build());
        }

        public DatabaseService WithScopedService()
        {
            var scopedService = new DatabaseService();
            scopedService.UseConfiguration(new DatabaseOptionsBuilder(_lastOptions));
            return scopedService;
        }

        public DatabaseService WithScopedService(DatabaseOptionsBuilder builder)
        {
            var scopedService = new DatabaseService();
            scopedService.UseConfiguration(builder);
            return scopedService;
        }

        public void Dispose()
        {
            _data?.Dispose();
        }
    }
}
