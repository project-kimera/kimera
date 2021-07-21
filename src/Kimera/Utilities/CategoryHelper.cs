using Kimera.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kimera.Utilities
{
    public static class CategoryHelper
    {
        public static async Task EnsureCategoryCreated(Guid guid, string name)
        {
            Category category = await App.DatabaseContext.Categories.Where(c => c.SystemId == guid).FirstOrDefaultAsync().ConfigureAwait(false);

            if (category == null)
            {
                Category newCategory = new Category();
                newCategory.SystemId = guid;
                newCategory.Name = name;

                await App.DatabaseContext.Categories.AddAsync(newCategory);
                await App.DatabaseContext.SaveChangesAsync();
            }
        }
    }
}
