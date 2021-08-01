using Kimera.Data.Entities;
using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Kimera.Services
{
    public class LibraryService
    {
        #region ::Singleton Members::

        private static LibraryService _instance;

        public static LibraryService Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new LibraryService();
                }

                return _instance;
            }
            set
            {
                _instance = value;
            }
        }

        #endregion

        #region ::Variables & Properties::

        public delegate void CategoriesChangedEventHandler(object sender, EventArgs e);

        public event CategoriesChangedEventHandler CategoriesChangedEvent;

        public delegate void SelectedCategoryChangedEventHandler(object sender, EventArgs e);

        public event SelectedCategoryChangedEventHandler SelectedCategoryChangedEvent;

        public delegate void GamesChangedEventHandler(object sender, EventArgs e);

        public event GamesChangedEventHandler GamesChangedEvent;

        private ObservableCollection<Category> _categories = new ObservableCollection<Category>();

        public ObservableCollection<Category> Categories
        {
            get
            {
                return _categories;
            }
            set
            {
                _categories = value;
                CategoriesChangedEvent?.Invoke(this, new EventArgs());
            }
        }

        private Guid _selectedCategory;

        public Guid SelectedCategory
        {
            get
            {
                return _selectedCategory;
            }
            set
            {
                _selectedCategory = value;
                SelectedCategoryChangedEvent?.Invoke(this, new EventArgs());
            }
        }

        private ObservableCollection<Game> _games = new ObservableCollection<Game>();

        public ObservableCollection<Game> Games
        {
            get
            {
                return _games;
            }
            set
            {
                _games = value;
                GamesChangedEvent?.Invoke(this, new EventArgs());
            }
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
                    _categories.Add(category);
                }

                CategoriesChangedEvent?.Invoke(this, new EventArgs());
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "An exception occurred while updating games.");
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

                    SelectedCategoryChangedEvent?.Invoke(this, new EventArgs());

                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "An exception occurred while updating games.");
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

                GamesChangedEvent?.Invoke(this, new EventArgs());
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

                CategoriesChangedEvent.Invoke(this, new EventArgs());
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                bool suicideFlag = false;

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
                            suicideFlag = true;
                            await UpdateSelectedCategoryAsync(Settings.GUID_ALL_CATEGORY);
                        }
                    }
                }

                CategoriesChangedEvent.Invoke(this, new EventArgs());
                
                if (suicideFlag)
                {
                    SelectedCategoryChangedEvent.Invoke(this, new EventArgs());
                }
            }

            Log.Information("The categories table has been changed.");
        }

        private void OnCategorySubscriptionsLocalCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
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

                GamesChangedEvent.Invoke(this, new EventArgs());
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

                GamesChangedEvent.Invoke(this, new EventArgs());
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                Games.Clear();
            }

            Log.Information("The category subscriptions table has been changed.");
        }

        #endregion

        #region ::Constructors::

        public LibraryService()
        {
            InitializeService();
        }

        private async void InitializeService()
        {
            await UpdateCategoriesAsync();
            await UpdateSelectedCategoryAsync(Settings.GUID_ALL_CATEGORY);
            await UpdateGamesAsync(Settings.GUID_ALL_CATEGORY);

            App.DatabaseContext.Categories.Local.CollectionChanged += OnCategoriesLocalCollectionChanged;
            App.DatabaseContext.CategorySubscriptions.Local.CollectionChanged += OnCategorySubscriptionsLocalCollectionChanged;

            Log.Information("The library service has been initialized.");
        }

        #endregion
    }
}
