using Caliburn.Micro;
using Kimera.Data.Entities;
using Kimera.Data.Extensions;
using Kimera.ViewModels.Dialogs;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Kimera.Services
{
    public class LibraryService : PropertyChangedBase
    {
        #region ::Variables & Properties::

        private BindableCollection<Category> _categories = new BindableCollection<Category>();

        public BindableCollection<Category> Categories
        {
            get => _categories;
            set
            { 
                Set(ref _categories, value);
            }
        }

        private Guid _selectedCategoryGuid;

        public Guid SelectedCategoryGuid
        {
            get => _selectedCategoryGuid;
            set
            {
                Set(ref _selectedCategoryGuid, value);
            }
        }

        private BindableCollection<Game> _games = new BindableCollection<Game>();

        public BindableCollection<Game> Games
        {
            get => _games;
            set
            {
                Set(ref _games, value);
            }
        }

        private BindableCollection<Game> _filteredGames = new BindableCollection<Game>();

        public BindableCollection<Game> FilteredGames
        {
            get => _filteredGames;
            set => Set(ref _filteredGames, value);
        }

        #endregion

        #region ::Constructors::

        public LibraryService()
        {
            InitializeService();
        }

        private async void InitializeService()
        {
            // Wait for database context.
            await UpdateCategoriesAsync();
            await UpdateSelectedCategoryAsync(Categories.FirstOrDefault().SystemId);
            await UpdateGamesAsync(Categories.FirstOrDefault().SystemId);

            App.DatabaseContext.Categories.Local.CollectionChanged += OnCategoriesLocalCollectionChanged;
            App.DatabaseContext.CategorySubscriptions.Local.CollectionChanged += OnCategorySubscriptionsLocalCollectionChanged;

            Log.Information("The library service has been initialized.");
        }

        #endregion

        #region ::Methods::

        public async Task ShowAllGamesAsync()
        {
            var task = Task.Factory.StartNew(() =>
            {
                _selectedCategoryGuid = Guid.Empty;

                List<Game> games = App.DatabaseContext.Games.ToList();

                Games = new BindableCollection<Game>(games);
            });

            await task;
        }

        public async Task ShowFavoriteGamesAsync()
        {
            var task = Task.Factory.StartNew(() =>
            {
                _selectedCategoryGuid = Guid.Empty;

                List<Game> games = App.DatabaseContext.Games.ToList();
                List<Game> result = new List<Game>();

                foreach (Game game in games)
                {
                    if (game.IsFavorite)
                    {
                        result.Add(game);
                    }
                }

                Games = new BindableCollection<Game>(result);
            });

            await task;
        }

        public async void ChangeCategory(Guid categoryGuid)
        {
            await UpdateSelectedCategoryAsync(categoryGuid).ConfigureAwait(false);
            await UpdateGamesAsync(categoryGuid).ConfigureAwait(false);
        }

        public void UpdateCategories()
        {
            try
            {
                List<Category> result = App.DatabaseContext.Categories.ToList();

                _categories.Clear();

                foreach (Category category in result)
                {
                    _categories.Add(category);
                }

                NotifyOfPropertyChange(() => Categories);
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "An exception occurred while updating categories.");
            }
        }

        public async Task UpdateCategoriesAsync()
        {
            var task = Task.Factory.StartNew(() =>
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    UpdateCategories();
                });
            });

            await task.ConfigureAwait(false);
        }

        public bool UpdateSelectedCategory(Guid categoryGuid)
        {
            try
            {
                Category category = App.DatabaseContext.Categories.Where(c => c.SystemId == categoryGuid).FirstOrDefault();

                if (category != null)
                {
                    SelectedCategoryGuid = category.SystemId;

                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "An exception occurred while updating selected category.");
                return false;
            }
        }

        public async Task<bool> UpdateSelectedCategoryAsync(Guid categoryGuid)
        {
            var task = Task.Factory.StartNew(() => UpdateSelectedCategory(categoryGuid));

            return await task.ConfigureAwait(false);
        }

        public void UpdateGames(Guid categoryGuid)
        {
            try
            {
                List<CategorySubscription> subscriptions = App.DatabaseContext.CategorySubscriptions.Where(c => c.Category == categoryGuid).ToList();

                var result = subscriptions.Select(s => s.GameNavigation);

                _games.Clear();

                foreach (Game game in result)
                {
                    _games.Add(game);
                }

                NotifyOfPropertyChange(() => Games);
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "An exception occurred while updating games.");
            }
        }

        public async Task UpdateGamesAsync(Guid categoryGuid)
        {
            var task = Task.Factory.StartNew(() =>
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    UpdateGames(categoryGuid);
                });
            });

            await task.ConfigureAwait(false);
        }

        #endregion

        #region ::Event Subscribers::

        private async void OnCategoriesLocalCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            try
            {
                if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    foreach (Category category in e.NewItems)
                    {

                        if (category != null)
                        {
                            App.Current.Dispatcher.Invoke(() =>
                            {
                                _categories.Add(category);
                            });
                        }
                    }

                    NotifyOfPropertyChange(() => Categories);
                }
                else if (e.Action == NotifyCollectionChangedAction.Remove)
                {
                    foreach (Category category in e.OldItems)
                    {
                        if (category != null)
                        {
                            Category targetCategory = _categories.Single(s => s.SystemId == category.SystemId);

                            if (targetCategory != null)
                            {
                                App.Current.Dispatcher.Invoke(() =>
                                {
                                    _categories.Remove(targetCategory);
                                });
                            }

                            if (targetCategory.SystemId == _selectedCategoryGuid)
                            {
                                await UpdateSelectedCategoryAsync(Categories.FirstOrDefault().SystemId).ConfigureAwait(false);
                                await UpdateGamesAsync(Categories.FirstOrDefault().SystemId).ConfigureAwait(false);
                            }
                        }
                    }

                    NotifyOfPropertyChange(() => Categories);
                }

                Log.Information("The categories table has been changed.");
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "An exception occurred while updating categories.");
            }
        }

        private void OnCategorySubscriptionsLocalCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            try
            {
                if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    foreach (CategorySubscription subscription in e.NewItems)
                    {
                        if (subscription != null)
                        {
                            if (subscription.Category == _selectedCategoryGuid)
                            {
                                App.Current.Dispatcher.Invoke(() =>
                                {
                                    _games.Add(subscription.GameNavigation);
                                });
                            }
                        }
                    }

                    NotifyOfPropertyChange(() => Games);
                }
                else if (e.Action == NotifyCollectionChangedAction.Remove)
                {
                    foreach (CategorySubscription subscription in e.OldItems)
                    {
                        if (subscription != null)
                        {
                            if (subscription.Category == _selectedCategoryGuid)
                            {
                                Game targetGame = _games.Single(s => s.SystemId == subscription.Game);

                                if (targetGame != null)
                                {
                                    App.Current.Dispatcher.Invoke(() =>
                                    {
                                        _games.Remove(targetGame);
                                    });
                                }
                            }
                        }
                    }

                    NotifyOfPropertyChange(() => Games);
                }
                else if (e.Action == NotifyCollectionChangedAction.Reset)
                {
                    Games.Clear();
                }

                Log.Information("The category subscriptions table has been changed.");
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "An exception occurred while updating games.");
            }
        }

        #endregion
    }
}