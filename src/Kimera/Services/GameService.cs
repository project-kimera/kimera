using Caliburn.Micro;
using Kimera.Data.Entities;
using Kimera.Data.Enums;
using Kimera.Data.Extensions;
using Kimera.Entities;
using Kimera.ViewModels.Dialogs;
using Kimera.ViewModels.Pages;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Kimera.Services
{
    public class GameService : PropertyChangedBase
    {
        #region ::Variables & Properties::

        private bool _isRunning = false;

        public bool IsRunning
        {
            get => _isRunning;
            set => Set(ref _isRunning, value);
        }

        #endregion

        #region ::Methods::

        public async Task<TaskRecord> AddGameAsync(GameMetadata gameMetadata, PackageMetadata packageMetadata, List<Component> components, List<Category> targetCategories, CancellationToken cancellationToken)
        {
            try
            {
                Game game = new Game();
                game.PackageStatus = PackageStatus.NeedProcessing;

                if (components == null || components.Count == 0)
                {
                    return new TaskRecord(TaskRecordType.Failure, "At least one component is required.");
                }

                foreach (Component component in components)
                {
                    packageMetadata.AddComponent(component);
                }

                game.SetGameMetadata(gameMetadata);
                game.SetPackageMetadata(packageMetadata);

                using (var transaction = await App.DatabaseContext.Database.BeginTransactionAsync())
                {
                    await App.DatabaseContext.Games.AddAsync(game);
                    await App.DatabaseContext.SaveChangesAsync();

                    await transaction.CommitAsync();
                }

                // Add categories.
                Category allCategory = await App.DatabaseContext.GetCategoryAsync(Settings.GUID_ALL_CATEGORY);
                await App.DatabaseContext.AddCategorySubscriptionAsync(allCategory, game);

                if (targetCategories == null || targetCategories.Count == 0)
                {
                    targetCategories = new List<Category>();
                }

                foreach (Category category in targetCategories)
                {
                    if (category.SystemId != Settings.GUID_ALL_CATEGORY && category.SystemId != Settings.GUID_FAVORITE_CATEGORY)
                    {
                        await App.DatabaseContext.AddCategorySubscriptionAsync(category, game);
                    }
                }

                if (cancellationToken.IsCancellationRequested)
                {
                    await RemoveGameAsync(game);
                    return new TaskRecord(TaskRecordType.Canceled, "The game registration has canceled.");
                }

                return new TaskRecord(TaskRecordType.Success, "The game has registered successfully.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An exception occurred while registering the game.");
                return new TaskRecord(TaskRecordType.Exception, $"An exception occurred while registering the game.\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }

        public async Task<TaskRecord> RemoveGameAsync(Game game)
        {
            try
            {
                await RemoveGameResourcesAsync(game.SystemId);

                using (var transaction = await App.DatabaseContext.Database.BeginTransactionAsync())
                {
                    // Remove category subscriptions.
                    List<CategorySubscription> subscriptions = App.DatabaseContext.CategorySubscriptions.Where(c => c.Game == game.SystemId).ToList();

                    foreach (CategorySubscription subscription in subscriptions)
                    {
                        App.DatabaseContext.CategorySubscriptions.Remove(subscription);
                    }

                    // Remove components.
                    foreach (Component component in game.PackageMetadataNavigation.Components)
                    {
                        App.DatabaseContext.Components.Remove(component);
                    }

                    // Remove datas.
                    GameMetadata gm = game.GameMetadataNavigation;
                    PackageMetadata pm = game.PackageMetadataNavigation;

                    App.DatabaseContext.Games.Remove(game);
                    App.DatabaseContext.GameMetadatas.Remove(gm);
                    App.DatabaseContext.PackageMetadatas.Remove(pm);

                    await App.DatabaseContext.SaveChangesAsync();

                    await transaction.CommitAsync();
                }

                return new TaskRecord(TaskRecordType.Success, "The game has removed successfully.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An exception occurred while removing the game.");
                return new TaskRecord(TaskRecordType.Exception, $"An exception occurred while removing the game.\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }

        public async Task<TaskRecord> RemoveGameResourcesAsync(Guid gameGuid)
        {
            try
            {
                string gameDirectory = Path.Combine(Settings.Instance.WorkDirectory, gameGuid.ToString());

                

                return new TaskRecord(TaskRecordType.Success, "The game resources have removed successfully.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An exception occurred while removing the game resources.");
                return new TaskRecord(TaskRecordType.Exception, $"An exception occurred while removing the game resources.\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }

        public async Task<TaskRecord> ValidateGameAsync(Game game)
        {
            return new TaskRecord(TaskRecordType.Success, "The game resources have removed successfully.");
        }

        public async Task<TaskRecord> ValidateGameResourcesAsync(Game game)
        {
            return new TaskRecord(TaskRecordType.Success, "The game resources have removed successfully.");
        }

        public async Task<TaskRecord> StartGameAsync(Game game)
        {
            return new TaskRecord(TaskRecordType.Success, "The game resources have removed successfully.");
        }

        public async Task<TaskRecord> ProcessGameAsync(Game game)
        {
            return new TaskRecord(TaskRecordType.Success, "The game resources have removed successfully.");
        }

        #endregion

        #region ::Actions::

        public void StartGame(Guid gameGuid)
        {

        }

        public void MoveGame(Guid gameGuid)
        {

        }

        public void RemoveGameResources(Guid gameGuid)
        {

        }

        public void RemoveGame(Guid gameGuid)
        {

        }

        public async void ViewGame(Guid gameGuid)
        {
            Game game = await App.DatabaseContext.Games.Where(g => g.SystemId == gameGuid).FirstOrDefaultAsync();

            if (game != null)
            {
                INavigationService navigationService = IoC.Get<INavigationService>();
                navigationService.For<GameViewModel>()
                    .WithParam(g => g.Game, game)
                    .Navigate();
            }
        }

        public async void CallGameMetadataEditor(Guid gameGuid)
        {
            Game game = await App.DatabaseContext.GetGameAsync(gameGuid).ConfigureAwait(false);

            if (game == null)
            {
                MessageBox.Show((string)App.Current.Resources["SVC_GAME_NOT_FOUND_MSG"], "Kimera", MessageBoxButton.OK, MessageBoxImage.Error);
                Log.Warning($"The game does not found. ({gameGuid})");
                return;
            }

            GameMetadataEditorViewModel viewModel = new GameMetadataEditorViewModel();
            viewModel.Metadata = game.GameMetadataNavigation.Copy();

            bool? dialogResult = await IoC.Get<IWindowManager>().ShowDialogAsync(viewModel).ConfigureAwait(false);

            if (dialogResult == true)
            {
                game.GameMetadataNavigation = viewModel.Metadata;

                LibraryService library = IoC.Get<LibraryService>();
                await library.UpdateGamesAsync(library.SelectedCategory).ConfigureAwait(false);
            }
        }

        public void CallPackageMetadataEditor(Guid gameGuid)
        {

        }

        #endregion
    }
}
