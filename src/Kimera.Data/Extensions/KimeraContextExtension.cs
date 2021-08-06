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
            Category category = await context.Categories.Where(c => c.SystemId == guid).FirstOrDefaultAsync();

            if (category == null)
            {
                using (var transaction = await context.Database.BeginTransactionAsync())
                {
                    Category newCategory = new Category(guid, name);

                    await context.Categories.AddAsync(newCategory);
                    await context.SaveChangesAsync();

                    await transaction.CommitAsync();
                }
            }
        }

        /// <summary>
        /// Sets the game metadata.
        /// </summary>
        /// <param name="gameMetadata">The game metadata.</param>
        public static void SetGameMetadata(this Game game, GameMetadata gameMetadata)
        {
            if (game.SystemId == Guid.Empty)
            {
                game.SystemId = Guid.NewGuid();
            }

            if (gameMetadata.SystemId == Guid.Empty)
            {
                gameMetadata.SystemId = Guid.NewGuid();
            }

            game.GameMetadata = gameMetadata.SystemId;
            gameMetadata.Game = game.SystemId;

            // To add the game metadata to database indirectly.
            game.GameMetadataNavigation = gameMetadata;
        }

        /// <summary>
        /// Sets the package metadata.
        /// </summary>
        /// <param name="packageMetadata">The package metadata.</param>
        public static void SetPackageMetadata(this Game game, PackageMetadata packageMetadata)
        {
            if (game.SystemId == Guid.Empty)
            {
                game.SystemId = Guid.NewGuid();
            }

            if (packageMetadata.SystemId == Guid.Empty)
            {
                packageMetadata.SystemId = Guid.NewGuid();
            }

            game.PackageMetadata = packageMetadata.SystemId;
            packageMetadata.Game = game.SystemId;

            game.PackageMetadataNavigation = packageMetadata;
        }

        /// <summary>
        /// Adds a component to package metadata.
        /// </summary>
        /// <param name="packageMetadata">The package metadata.</param>
        /// <param name="component">The component.</param>
        public static void AddComponent(this PackageMetadata packageMetadata, Component component)
        {
            if (packageMetadata.SystemId == Guid.Empty)
            {
                packageMetadata.SystemId = Guid.NewGuid();
            }

            component.PackageMetadata = packageMetadata.SystemId;
            packageMetadata.Components.Add(component);
        }
    }
}
