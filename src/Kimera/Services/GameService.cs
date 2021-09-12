using Caliburn.Micro;
using Kimera.Data.Entities;
using Kimera.Data.Enums;
using Kimera.Data.Extensions;
using Kimera.Entities;
using Kimera.IO;
using Kimera.Utilities;
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

        private LibraryService _libraryService = IoC.Get<LibraryService>();

        private bool _isRunning = false;

        public bool IsRunning
        {
            get => _isRunning;
            set => Set(ref _isRunning, value);
        }

        #endregion

        #region ::Methods::

        public async Task<TaskRecord> AddGameInternalAsync(GameMetadata gameMetadata, PackageMetadata packageMetadata, List<Category> targetCategories)
        {
            try
            {
                Game game = new Game();
                game.PackageStatus = PackageStatus.NeedProcessing;

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

                return new TaskRecord(TaskRecordType.Success, "The game has registered successfully.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An exception occurred while registering the game.");
                return new TaskRecord(TaskRecordType.Exception, $"An exception occurred while registering the game.\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }

        public async Task<TaskRecord> RemoveGameInternalAsync(Game game)
        {
            try
            {
                await RemoveGameResourcesInternalAsync(game);

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
                    App.DatabaseContext.Games.Remove(game);
                    App.DatabaseContext.GameMetadatas.Remove(game.GameMetadataNavigation);
                    App.DatabaseContext.PackageMetadatas.Remove(game.PackageMetadataNavigation);

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

        public async Task<TaskRecord> RemoveGameResourcesInternalAsync(Game game)
        {
            try
            {
                string gameDirectory = VariableBuilder.GetGameDirectory(game.SystemId);

                if (Directory.Exists(gameDirectory))
                {

                }

                return new TaskRecord(TaskRecordType.Success, "The game resources have removed successfully.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An exception occurred while removing the game resources.");
                return new TaskRecord(TaskRecordType.Exception, $"An exception occurred while removing the game resources.\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
            finally
            {
                await _libraryService.UpdateGamesAsync(_libraryService.SelectedCategory);
            }
        }

        public async Task<TaskRecord> ValidateMetadataInternalAsync(Game game)
        {
            try
            {
                foreach (Component component in game.PackageMetadataNavigation.Components)
                {
                    // Checks components.
                    if (!File.Exists(component.FilePath))
                    {
                        using (var transaction = await App.DatabaseContext.Database.BeginTransactionAsync())
                        {
                            game.PackageStatus = PackageStatus.FileNotFound;

                            await App.DatabaseContext.SaveChangesAsync();

                            await transaction.CommitAsync();
                        }

                        Log.Warning($"The component does not found. ({component.FilePath})");
                        return new TaskRecord(TaskRecordType.Failure, $"The component does not found. ({component.FilePath})");
                    }

                    // Checks component password.
                    ArchiveFileManager archive = new ArchiveFileManager(component.FilePath);
                    //await archive.IsValidPasswordAsync();
                }



                // Sets the package status to playable.
                using (var transaction = await App.DatabaseContext.Database.BeginTransactionAsync())
                {
                    game.PackageStatus = PackageStatus.Playable;

                    await App.DatabaseContext.SaveChangesAsync();

                    await transaction.CommitAsync();

                    return new TaskRecord(TaskRecordType.Success, "The metadata is valid.");
                }
            }
            catch (Exception ex)
            {
                using (var transaction = await App.DatabaseContext.Database.BeginTransactionAsync())
                {
                    game.PackageStatus = PackageStatus.Exception;

                    await App.DatabaseContext.SaveChangesAsync();

                    await transaction.CommitAsync();

                    Log.Error(ex, "An exception has occurred while validating the metadata.");
                    return new TaskRecord(TaskRecordType.Exception, "An exception has occurred while validating the metadata.");
                }
            }
            finally
            {
                await _libraryService.UpdateGamesAsync(_libraryService.SelectedCategory);
            }
        }

        public async Task<TaskRecord> ValidateResourcesInternalAsync(Game game)
        {
            string gameDirectory = VariableBuilder.GetGameDirectory(game.SystemId);
            
            // Checks the entry point.
            if (Path.IsPathRooted(game.PackageMetadataNavigation.EntryPointFilePath))
            {
                if (!File.Exists(game.PackageMetadataNavigation.EntryPointFilePath))
                {
                    return new TaskRecord(TaskRecordType.Failure, "The entry point does not found.");
                }
            }
            else
            {
                string entryPoint = Path.Combine(gameDirectory, game.PackageMetadataNavigation.EntryPointFilePath);

                if (!File.Exists(entryPoint))
                {
                    return new TaskRecord(TaskRecordType.Failure, "The entry point does not found.");
                }
            }         

            return new TaskRecord(TaskRecordType.Success, "The game resources are valid.");
        }

        public async Task<TaskRecord> DecompressComponentsInternalAsync(Game game, bool ignorePackageStatus = false)
        {
            if (game.PackageStatus == PackageStatus.Compressed || ignorePackageStatus)
            {
                // If the package type is single.
                if (game.PackageMetadataNavigation.Type == PackageType.Single)
                {
                    Component mainComponent = await App.DatabaseContext.Components.Where(c => c.PackageMetadata == game.PackageMetadata && c.Index == 0).FirstOrDefaultAsync();

                    if (mainComponent == null)
                    {
                        return new TaskRecord(TaskRecordType.Failure, $"The main component does not found.");
                    }

                    ArchiveFileManager archive = new ArchiveFileManager(mainComponent.FilePath);

                    try
                    {
                        archive.GetArchiveType();
                    }
                    catch
                    {
                        return new TaskRecord(TaskRecordType.Failure, $"The main component is not a valid archive file.");
                    }
                }


                // If the package type is chunk
                foreach (Component component in game.PackageMetadataNavigation.Components)
                {
                    if (File.Exists(component.FilePath))
                    {

                    }
                }
            }

            return new TaskRecord(TaskRecordType.Success, "The components have decompressed successfully.");
        }

        public async Task<TaskRecord> RunProcessInternalAsync(Game game)
        {

            return new TaskRecord(TaskRecordType.Success, "The game resources have removed successfully.");
        }

        public async Task<TaskRecord> StartGameInternalAsync(Game game)
        {
            TaskRecord metadataValidationResult = await ValidateMetadataInternalAsync(game);

            if (metadataValidationResult.Type != TaskRecordType.Success)
            {
                return metadataValidationResult;
            }

            TaskRecord resourcesValidationResult = await ValidateResourcesInternalAsync(game);

            if (resourcesValidationResult.Type != TaskRecordType.Success)
            {
                return resourcesValidationResult;
            }

            TaskRecord decompressionResult = await DecompressComponentsInternalAsync(game);

            if (decompressionResult.Type != TaskRecordType.Success)
            {
                return decompressionResult;
            }

            return new TaskRecord(TaskRecordType.Success, "The game has started successfully.");
        }

        #endregion

        #region ::Actions::

        public async void StartGame(Guid gameGuid)
        {
            Game game = await App.DatabaseContext.GetGameAsync(gameGuid).ConfigureAwait(false);

            if (game == null)
            {
                Log.Warning($"The game does not found. ({gameGuid})");
                MessageBox.Show((string)App.Current.Resources["SVC_GAME_GAME_NOT_FOUND_MSG"], "Kimera", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            TaskRecord result = await StartGameInternalAsync(game);

            if (result.Type != TaskRecordType.Success)
            {
                Log.Warning($"Failed to start the game. ({gameGuid})");
                MessageBox.Show($"Failed to start the game. ({gameGuid})\r\n{result.Message}", "Kimera", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }



            MessageBox.Show($"SUCCESS ({gameGuid})\r\n{result.Message}", "Kimera", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        public async void ChangeFavoriteState(Guid gameGuid)
        {
            Game game = await App.DatabaseContext.GetGameAsync(gameGuid).ConfigureAwait(false);

            if (game == null)
            {
                Log.Warning($"The game does not found. ({gameGuid})");
                MessageBox.Show((string)App.Current.Resources["SVC_GAME_GAME_NOT_FOUND_MSG"], "Kimera", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            CategorySubscription subscription = await App.DatabaseContext.CategorySubscriptions.Where(c => c.Game == gameGuid && c.Category == Settings.GUID_FAVORITE_CATEGORY).FirstOrDefaultAsync();

            if (subscription == null)
            {
                using (var transaction = await App.DatabaseContext.Database.BeginTransactionAsync().ConfigureAwait(false))
                {
                    CategorySubscription newSubscription = new CategorySubscription();
                    newSubscription.Category = Settings.GUID_FAVORITE_CATEGORY;
                    newSubscription.CategoryNavigation = await App.DatabaseContext.GetCategoryAsync(Settings.GUID_FAVORITE_CATEGORY).ConfigureAwait(false);
                    newSubscription.Game = game.SystemId;
                    newSubscription.GameNavigation = game;

                    await App.DatabaseContext.CategorySubscriptions.AddAsync(newSubscription).ConfigureAwait(false);

                    await App.DatabaseContext.SaveChangesAsync().ConfigureAwait(false);

                    await transaction.CommitAsync().ConfigureAwait(false);
                }

                MessageBox.Show((string)App.Current.Resources["SVC_GAME_ADD_FAVORITE_MSG"], "Kimera", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                if (MessageBox.Show((string)App.Current.Resources["SVC_GAME_REMOVE_FAVORITE_CHECKER_MSG"], "Kimera", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
                {
                    using (var transaction = await App.DatabaseContext.Database.BeginTransactionAsync().ConfigureAwait(false))
                    {

                        App.DatabaseContext.CategorySubscriptions.Remove(subscription);

                        await App.DatabaseContext.SaveChangesAsync().ConfigureAwait(false);

                        await transaction.CommitAsync().ConfigureAwait(false);
                    }
                }
            }
        }

        public async void MoveGame(Guid gameGuid)
        {
            Game game = await App.DatabaseContext.GetGameAsync(gameGuid).ConfigureAwait(false);

            if (game == null)
            {
                Log.Warning($"The game does not found. ({gameGuid})");
                MessageBox.Show((string)App.Current.Resources["SVC_GAME_GAME_NOT_FOUND_MSG"], "Kimera", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            CategorySubscription subscription = await App.DatabaseContext.CategorySubscriptions.Where(c => c.Game == gameGuid && c.Category != Settings.GUID_ALL_CATEGORY && c.Category != Settings.GUID_FAVORITE_CATEGORY).FirstOrDefaultAsync();

            if (subscription == null)
            {
                CategorySelectorViewModel viewModel = new CategorySelectorViewModel();
                viewModel.Title = (string)App.Current.Resources["VIEW_CATEGORYSELECTOR_MOVE_GAME_CATEGORY_TITLE"];
                viewModel.Caption = string.Format((string)App.Current.Resources["VIEW_CATEGORYSELECTOR_MOVE_GAME_CATEGORY_CAPTION"], (string)App.Current.Resources["SVC_GAME_CATEGORY_NONE"]);
                viewModel.Categories = _libraryService.Categories;

                bool? dialogResult = await IoC.Get<IWindowManager>().ShowDialogAsync(viewModel).ConfigureAwait(false);

                if (dialogResult == true)
                {
                    using (var transaction = await App.DatabaseContext.Database.BeginTransactionAsync().ConfigureAwait(false))
                    {
                        CategorySubscription newSubscription = new CategorySubscription();
                        newSubscription.Category = viewModel.SelectedCategory.SystemId;
                        newSubscription.CategoryNavigation = viewModel.SelectedCategory;
                        newSubscription.Game = game.SystemId;
                        newSubscription.GameNavigation = game;

                        await App.DatabaseContext.CategorySubscriptions.AddAsync(newSubscription).ConfigureAwait(false);

                        await App.DatabaseContext.SaveChangesAsync().ConfigureAwait(false);

                        await transaction.CommitAsync().ConfigureAwait(false);
                    }

                    await _libraryService.UpdateCategoriesAsync();
                    await _libraryService.UpdateGamesAsync(_libraryService.SelectedCategory).ConfigureAwait(false);
                }
            }
            else
            {
                CategorySelectorViewModel viewModel = new CategorySelectorViewModel();
                viewModel.Title = (string)App.Current.Resources["VIEW_CATEGORYSELECTOR_MOVE_GAME_CATEGORY_TITLE"];
                viewModel.Caption = string.Format((string)App.Current.Resources["VIEW_CATEGORYSELECTOR_MOVE_GAME_CATEGORY_CAPTION"], subscription.CategoryNavigation.Name);
                viewModel.Categories = _libraryService.Categories;
                viewModel.SelectedCategory = subscription.CategoryNavigation;

                bool? dialogResult = await IoC.Get<IWindowManager>().ShowDialogAsync(viewModel).ConfigureAwait(false);

                if (dialogResult == true)
                {
                    using (var transaction = await App.DatabaseContext.Database.BeginTransactionAsync().ConfigureAwait(false))
                    {
                        subscription.Category = viewModel.SelectedCategory.SystemId;
                        subscription.CategoryNavigation = viewModel.SelectedCategory;

                        await App.DatabaseContext.SaveChangesAsync().ConfigureAwait(false);

                        await transaction.CommitAsync().ConfigureAwait(false);
                    }

                    await _libraryService.UpdateCategoriesAsync();
                    await _libraryService.UpdateGamesAsync(_libraryService.SelectedCategory).ConfigureAwait(false);
                }
            }
        }

        public void RemoveGameResources(Guid gameGuid)
        {

        }

        public async void RemoveGame(Guid gameGuid)
        {
            Game game = await App.DatabaseContext.Games.Where(g => g.SystemId == gameGuid).FirstOrDefaultAsync();

            if (game == null)
            {
                Log.Warning($"The game does not found. ({gameGuid})");
                MessageBox.Show((string)App.Current.Resources["SVC_GAME_GAME_NOT_FOUND_MSG"], "Kimera", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            await RemoveGameInternalAsync(game);
        }

        public async void ViewGame(Guid gameGuid)
        {
            Game game = await App.DatabaseContext.Games.Where(g => g.SystemId == gameGuid).FirstOrDefaultAsync();

            if (game == null)
            {
                Log.Warning($"The game does not found. ({gameGuid})");
                MessageBox.Show((string)App.Current.Resources["SVC_GAME_GAME_NOT_FOUND_MSG"], "Kimera", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            INavigationService navigationService = IoC.Get<INavigationService>();
            navigationService.For<GameViewModel>()
                .WithParam(g => g.Game, game)
                .Navigate();
        }

        public async void CallGameMetadataEditor(Guid gameGuid)
        {
            Game game = await App.DatabaseContext.GetGameAsync(gameGuid).ConfigureAwait(false);

            if (game == null)
            {
                Log.Warning($"The game does not found. ({gameGuid})");
                MessageBox.Show((string)App.Current.Resources["SVC_GAME_NOT_FOUND_MSG"], "Kimera", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            GameMetadataEditorViewModel viewModel = new GameMetadataEditorViewModel(game.GameMetadataNavigation);

            bool? dialogResult = await IoC.Get<IWindowManager>().ShowDialogAsync(viewModel).ConfigureAwait(false);

            if (dialogResult == true)
            {
                using (var transaction = await App.DatabaseContext.Database.BeginTransactionAsync().ConfigureAwait(false))
                {
                    App.DatabaseContext.Entry(game.GameMetadataNavigation).CurrentValues.SetValues(viewModel.Metadata);
                    App.DatabaseContext.Entry(game.GameMetadataNavigation).State = EntityState.Modified;


                    await App.DatabaseContext.SaveChangesAsync().ConfigureAwait(false);

                    await transaction.CommitAsync().ConfigureAwait(false);
                }

                await _libraryService.UpdateGamesAsync(_libraryService.SelectedCategory).ConfigureAwait(false);
            }
        }

        public async void CallPackageMetadataEditor(Guid gameGuid)
        {
            Game game = await App.DatabaseContext.GetGameAsync(gameGuid).ConfigureAwait(false);

            if (game == null)
            {
                Log.Warning($"The game does not found. ({gameGuid})");
                MessageBox.Show((string)App.Current.Resources["SVC_GAME_NOT_FOUND_MSG"], "Kimera", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            PackageMetadataEditorViewModel viewModel = new PackageMetadataEditorViewModel(game.PackageMetadataNavigation);

            bool? dialogResult = await IoC.Get<IWindowManager>().ShowDialogAsync(viewModel).ConfigureAwait(false);

            if (dialogResult == true)
            {
                using (var transaction = await App.DatabaseContext.Database.BeginTransactionAsync().ConfigureAwait(false))
                {
                    App.DatabaseContext.Entry(game.PackageMetadataNavigation).CurrentValues.SetValues(viewModel.Metadata);
                    App.DatabaseContext.Entry(game.PackageMetadataNavigation).State = EntityState.Modified;

                    await App.DatabaseContext.SaveChangesAsync().ConfigureAwait(false);

                    await transaction.CommitAsync().ConfigureAwait(false);
                }

                await _libraryService.UpdateGamesAsync(_libraryService.SelectedCategory).ConfigureAwait(false);
            }
        }

        #endregion
    }
}
