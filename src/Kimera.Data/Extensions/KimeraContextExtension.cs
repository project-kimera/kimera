using Kimera.Data.Contexts;
using Kimera.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kimera.Data.Extensions
{
    public static class KimeraContextExtension
    {
        public static async Task EnsureCategoryCreated(this KimeraContext context, Guid guid, string name)
        {
            Category category = await context.Categories.Where(c => c.SystemId == guid).FirstOrDefaultAsync().ConfigureAwait(false);

            if (category == null)
            {
                Category newCategory = new Category();
                newCategory.SystemId = guid;
                newCategory.Name = name;

                await context.Categories.AddAsync(newCategory);
                await context.SaveChangesAsync();
            }
        }
    }
}
