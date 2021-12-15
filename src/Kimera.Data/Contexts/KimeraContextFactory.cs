using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kimera.Data.Contexts
{
    public class KimeraContextFactory : IDesignTimeDbContextFactory<KimeraContext>
    {
        public KimeraContext CreateDbContext(string[] args)
        {
            return new KimeraContext("Kimera.db");
        }
    }
}
