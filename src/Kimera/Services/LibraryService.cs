using Caliburn.Micro;
using Kimera.Data.Entities;
using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;

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

        private Guid _selectedCategory;

        public Guid SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                Set(ref _selectedCategory, value);
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
            await UpdateSelectedCategoryAsync(Settings.GUID_ALL_CATEGORY);
            await UpdateGamesAsync(Settings.GUID_ALL_CATEGORY);

            App.DatabaseContext.Categories.Local.CollectionChanged += OnCategoriesLocalCollectionChanged;
            App.DatabaseContext.CategorySubscriptions.Local.CollectionChanged += OnCategorySubscriptionsLocalCollectionChanged;

            Log.Information("The library service has been initialized.");
        }

        #endregion

        #region ::Methods::

        public void UpdateCategories()
        {
            try
            {
                List<Category> result = App.DatabaseContext.Categories.ToList();

                _categories.Clear();

                foreach (Category category in result)
                {
                    if (category.SystemId != Settings.GUID_ALL_CATEGORY && category.SystemId != Settings.GUID_FAVORITE_CATEGORY)
                    {
                        _categories.Add(category);
                    }
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
                Category category = App.DatabaseContext.Categories.Where(c => c.SystemId == categoryGuid).First();

                if (category != null)
                {
                    SelectedCategory = categoryGuid;

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

                            if (targetCategory.SystemId == _selectedCategory)
                            {
                                await UpdateSelectedCategoryAsync(Settings.GUID_ALL_CATEGORY);
                                await UpdateGamesAsync(Settings.GUID_ALL_CATEGORY);
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
                            if (subscription.Category == _selectedCategory)
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
                            if (subscription.Category == _selectedCategory)
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