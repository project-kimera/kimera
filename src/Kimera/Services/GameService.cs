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

        private Guid _runningGameGuid = Guid.Empty;

        public Guid RunningGameGuid
        {
            get => _runningGameGuid;
            set => Set(ref _runningGameGuid, value);
        }

        private bool _isRunning = false;

        public bool IsRunning
        {
            get => _isRunning;
            set => Set(ref _isRunning, value);
        }

        #endregion

        #region ::Methods::

        /// <summary>
        /// Add a game with metadatas to database. (Non-observable, Non-cancellable)
        /// </summary>
        /// <param name="gameMetadata">GameMetadata of the game.</param>
        /// <param name="packageMetadata">PackageMetadata of the game.</param>
        /// <param name="targetCategory">A category where the game is.</param>
        /// <returns>A task record</returns>
        public async Task<TaskRecord> AddGameInternalAsync(GameMetadata gameMetadata, PackageMetadata packageMetadata, Category targetCategory)
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
                if (targetCategory != null)
                {
                    await App.DatabaseContext.AddCategorySubscriptionAsync(targetCategory, game);
                }

                return new TaskRecord(Entities.TaskStatus.Success, "The game has registered successfully.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An exception occurred while registering the game.");
                return new TaskRecord(Entities.TaskStatus.Exception, $"An exception occurred while registering the game.\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }

        /// <summary>
        /// Remove game from database. (Non-observable, Non-cancellable)
        /// </summary>
        /// <param name="game">A game to remove.</param>
        /// <returns>A task record</returns>
        public async Task<TaskRecord> RemoveGameInternalAsync(Game game)
        {
            try
            {
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
                    App.DatabaseContext.GameMetadatas.Remove(game.GameMetadataNavigation);
                    App.DatabaseContext.PackageMetadatas.Remove(game.PackageMetadataNavigation);
                    App.DatabaseContext.Games.Remove(game);

                    await App.DatabaseContext.SaveChangesAsync();

                    await transaction.CommitAsync();
                }

                return new TaskRecord(Entities.TaskStatus.Success, "The game has removed successfully.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An exception occurred while removing the game.");
                return new TaskRecord(Entities.TaskStatus.Exception, $"An exception occurred while removing the game.\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }

        /// <summary>
        /// Remove game resources from file system. (Observable, Cancellable)
        /// </summary>
        /// <param name="game">A game to remove.</param>
        /// <returns>A task record</returns>
        public async Task<TaskRecord> RemoveGameResourcesInternalAsync(Game game)
        {
            try
            {
                VariableBuilder builder = new VariableBuilder(game);
                string gameDirectory = builder.GetGameDirectory();

                if (Directory.Exists(gameDirectory))
                {

                }

                return new TaskRecord(Entities.TaskStatus.Success, "The game resources have removed successfully.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An exception occurred while removing the game resources.");
                return new TaskRecord(Entities.TaskStatus.Exception, $"An exception occurred while removing the game resources.\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
            finally
            {
                if (_libraryService.SelectedCategoryGuid != Guid.Empty)
                {
                    await _libraryService.UpdateGamesAsync(_libraryService.SelectedCategoryGuid).ConfigureAwait(false);
                }
                else
                {
                    await _libraryService.ShowAllGamesAsync();
                }
            }
        }

        /// <summary>
        /// Validate metadatas of a game. (Non-observable, Non-cancellable)
        /// </summary>
        /// <param name="game">A game to validate.</param>
        /// <returns>A task record</returns>
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
                        return new TaskRecord(Entities.TaskStatus.Failure, $"The component does not found. ({component.FilePath})");
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

                    return new TaskRecord(Entities.TaskStatus.Success, "The metadata is valid.");
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
                    return new TaskRecord(Entities.TaskStatus.Exception, "An exception has occurred while validating the metadata.");
                }
            }
            finally
            {
                if (_libraryService.SelectedCategoryGuid != Guid.Empty)
                {
                    await _libraryService.UpdateGamesAsync(_libraryService.SelectedCategoryGuid).ConfigureAwait(false);
                }
                else
                {
                    await _libraryService.ShowAllGamesAsync();
                }
            }
        }

        /// <summary>
        /// Validate game resources. (Non-observable, Non-cancellable)
        /// </summary>
        /// <param name="game">A game to validate.</param>
        /// <returns>A task record</returns>
        public async Task<TaskRecord> ValidateResourcesInternalAsync(Game game)
        {
            VariableBuilder builder = new VariableBuilder(game);
            string gameDirectory = builder.GetGameDirectory();

            // Checks the entry point.
            if (Path.IsPathRooted(game.PackageMetadataNavigation.EntryPointFilePath))
            {
                if (!File.Exists(game.PackageMetadataNavigation.EntryPointFilePath))
                {
                    return new TaskRecord(Entities.TaskStatus.Failure, "The entry point does not found.");
                }
            }
            else
            {
                string entryPoint = Path.Combine(gameDirectory, game.PackageMetadataNavigation.EntryPointFilePath);

                if (!File.Exists(entryPoint))
                {
                    return new TaskRecord(Entities.TaskStatus.Failure, "The entry point does not found.");
                }
            }         

            return new TaskRecord(Entities.TaskStatus.Success, "The game resources are valid.");
        }

        /// <summary>
        /// Decompress components of a game. (Observable, Cancellable)
        /// </summary>
        /// <param name="game">A game to decompress.</param>
        /// <param name="ignorePackageStatus">If it is true, task will ignore package status during work.</param>
        /// <returns>A task record</returns>
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
                        return new TaskRecord(Entities.TaskStatus.Failure, $"The main component does not found.");
                    }

                    ArchiveFileManager archive = new ArchiveFileManager(mainComponent.FilePath);

                    try
                    {
                        archive.GetArchiveType();
                    }
                    catch
                    {
                        return new TaskRecord(Entities.TaskStatus.Failure, $"The main component is not a valid archive file.");
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

            return new TaskRecord(Entities.TaskStatus.Success, "The components have decompressed successfully.");
        }

        /// <summary>
        /// Run game process. (Non-observable, Cancellable)
        /// </summary>
        /// <param name="game">A game to run.</param>
        /// <returns>A task record</returns>
        public async Task<TaskRecord> RunProcessInternalAsync(Game game)
        {

            return new TaskRecord(Entities.TaskStatus.Success, "The game resources have removed successfully.");
        }

        /// <summary>
        /// Process some necessary tasks and Run a game. (Observable, Cancellable)
        /// </summary>
        /// <param name="game">A game to start.</param>
        /// <returns>A task record</returns>
        public async Task<TaskRecord> StartGameInternalAsync(Game game)
        {
            TaskRecord metadataValidationResult = await ValidateMetadataInternalAsync(game);

            if (metadataValidationResult.Type != Entities.TaskStatus.Success)
            {
                return metadataValidationResult;
            }

            TaskRecord resourcesValidationResult = await ValidateResourcesInternalAsync(game);

            if (resourcesValidationResult.Type != Entities.TaskStatus.Success)
            {
                return resourcesValidationResult;
            }

            TaskRecord decompressionResult = await DecompressComponentsInternalAsync(game);

            if (decompressionResult.Type != Entities.TaskStatus.Success)
            {
                return decompressionResult;
            }

            return new TaskRecord(Entities.TaskStatus.Success, "The game has started successfully.");
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

            if (result.Type != Entities.TaskStatus.Success)
            {
                Log.Warning($"Failed to start the game. ({gameGuid})");
                MessageBox.Show(string.Format((string)App.Current.Resources["SVC_GAME_FAILED_TO_START_MSG"], result.Message), "Kimera", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (result.Type == Entities.TaskStatus.Success)
            {
                RunningGameGuid = gameGuid;
                IsRunning = true;
            }
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

            if (!game.IsFavorite)
            {
                using (var transaction = await App.DatabaseContext.Database.BeginTransactionAsync().ConfigureAwait(false))
                {
                    game.IsFavorite = true;

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

                        game.IsFavorite = false;

                        await App.DatabaseContext.SaveChangesAsync().ConfigureAwait(false);

                        await transaction.CommitAsync().ConfigureAwait(false);
                    }
                }
            }

            if (_libraryService.SelectedCategoryGuid != Guid.Empty)
            {
                await _libraryService.UpdateGamesAsync(_libraryService.SelectedCategoryGuid).ConfigureAwait(false);
            }
            else
            {
                await _libraryService.ShowAllGamesAsync();
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

            CategorySubscription subscription = await App.DatabaseContext.CategorySubscriptions.Where(c => c.Game == gameGuid).FirstOrDefaultAsync();

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

                    if (_libraryService.SelectedCategoryGuid != Guid.Empty)
                    {
                        await _libraryService.UpdateGamesAsync(_libraryService.SelectedCategoryGuid).ConfigureAwait(false);
                    }
                    else
                    {
                        await _libraryService.ShowAllGamesAsync();
                    }
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

                    if (_libraryService.SelectedCategoryGuid != Guid.Empty)
                    {
                        await _libraryService.UpdateGamesAsync(_libraryService.SelectedCategoryGuid).ConfigureAwait(false);
                    }
                    else
                    {
                        await _libraryService.ShowAllGamesAsync();
                    }
                }
            }
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

            if (MessageBox.Show((string)App.Current.Resources["SVC_GAME_REMOVE_DATA_CHECK_MSG"], "Kimera", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
            {
                await RemoveGameInternalAsync(game);
            }
            else
            {
                return;
            }

            if (MessageBox.Show((string)App.Current.Resources["SVC_GAME_REMOVE_RESOURCES_CHECK_MSG"], "Kimera", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
            {
                await RemoveGameResourcesInternalAsync(game);
            }
            else
            {
                return;
            }

            MessageBox.Show((string)App.Current.Resources["SVC_GAME_REMOVE_SUCCESS_MSG"], "Kimera", MessageBoxButton.OK, MessageBoxImage.Information);
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

                if (_libraryService.SelectedCategoryGuid != Guid.Empty)
                {
                    await _libraryService.UpdateGamesAsync(_libraryService.SelectedCategoryGuid).ConfigureAwait(false);
                }
                else
                {
                    await _libraryService.ShowAllGamesAsync();
                }
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

                if (_libraryService.SelectedCategoryGuid != Guid.Empty)
                {
                    await _libraryService.UpdateGamesAsync(_libraryService.SelectedCategoryGuid).ConfigureAwait(false);
                }
                else
                {
                    await _libraryService.ShowAllGamesAsync();
                }
            }
        }

        #endregion
    }
}
