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
    /// <summary>
    /// Extends the feature of <see cref="KimeraContext"/>.
    /// </summary>
    public static class KimeraContextExtension
    {
        /// <summary>
        /// Ensure that the category is created.
        /// </summary>
        /// <param name="context">The <see cref="KimeraContext"/> instance to be used.</param>
        /// <param name="guid">The GUID of a category to create.</param>
        /// <param name="name">The name of a category to create.</param>
        /// <returns></returns>
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
