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
        public static async Task EnsureCategoryCreatedAsync(this KimeraContext context, Guid guid, string name)
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
        /// Returns the category from GUID.
        /// </summary>
        /// <param name="context">The <see cref="KimeraContext"/> instance to be used.</param>
        /// <param name="categoryGuid">The GUID of the category.</param>
        /// <returns>The category.</returns>
        public static async Task<Category> GetCategory(this KimeraContext context, Guid categoryGuid)
        {
            Category category = await context.Categories.Where(c => c.SystemId == categoryGuid).FirstOrDefaultAsync();
            return category;
        }

        /// <summary>
        /// Returns the game from GUID.
        /// </summary>
        /// <param name="context">The <see cref="KimeraContext"/> instance to be used.</param>
        /// <param name="gameGuid">The GUID of the game.</param>
        /// <returns>The game.</returns>
        public static async Task<Game> GetGame(this KimeraContext context, Guid gameGuid)
        {
            Game game = await context.Games.Where(c => c.SystemId == gameGuid).FirstOrDefaultAsync();
            return game;
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

        /// <summary>
        /// Adds a category subscription to kimera context.
        /// </summary>
        /// <param name="context">The <see cref="KimeraContext"/> instance to be used.</param>
        /// <param name="category">The category to add.</param>
        /// <param name="game">The game to add.</param>
        /// <returns>Task result.</returns>
        public static async Task AddCategorySubscriptionAsync(this KimeraContext context, Category category, Game game)
        {
            if (category.SystemId == Guid.Empty)
            {
                category.SystemId = Guid.NewGuid();
            }

            if (game.SystemId == Guid.Empty)
            {
                game.SystemId = Guid.NewGuid();
            }

            using (var transaction = await context.Database.BeginTransactionAsync())
            {
                CategorySubscription subscription = new CategorySubscription();
                subscription.Category = category.SystemId;
                subscription.Game = game.SystemId;
                subscription.CategoryNavigation = category;
                subscription.GameNavigation = game;

                await context.CategorySubscriptions.AddAsync(subscription);
                await context.SaveChangesAsync();

                await transaction.CommitAsync();
            }
        }
    }
}
