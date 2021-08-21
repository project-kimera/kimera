using Caliburn.Micro;
using Kimera.Data.Entities;
using Kimera.Services;
using Kimera.ViewModels.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Kimera.ViewModels.Pages
{
    public class LibraryViewModel : Screen
    {
        #region ::Variables & Properties::

        private LibraryService _libraryService = IoC.Get<LibraryService>();

        public LibraryService LibraryService
        {
            get => _libraryService;
            set => Set(ref _libraryService, value);
        }

        private GameService _gameService = IoC.Get<GameService>();

        public GameService GameService
        {
            get => _gameService;
            set => Set(ref _gameService, value);
        }

        private BindableCollection<Game> _filteredGames = new BindableCollection<Game>();

        public BindableCollection<Game> FilteredGames
        {
            get => _filteredGames;
            set => Set(ref _filteredGames, value);
        }

        private DataTemplate _viewTemplate = App.Current.FindResource("TileViewItemTemplate") as DataTemplate;

        public DataTemplate ViewTemplate
        {
            get => _viewTemplate;
            set => Set(ref _viewTemplate, value);
        }

        #endregion

        #region ::Constructors::

        public LibraryViewModel()
        {

        }

        #endregion

        #region ::Actions::

        public async void RefreshGames()
        {
            await _libraryService.UpdateGamesAsync(_libraryService.SelectedCategory);
        }

        public async void ChangeToAllCategory()
        {
            await _libraryService.UpdateSelectedCategoryAsync(Settings.GUID_ALL_CATEGORY);
            await _libraryService.UpdateGamesAsync(Settings.GUID_ALL_CATEGORY);
        }

        public async void ChangeToFavoriteCategory()
        {
            await _libraryService.UpdateSelectedCategoryAsync(Settings.GUID_FAVORITE_CATEGORY);
            await _libraryService.UpdateGamesAsync(Settings.GUID_FAVORITE_CATEGORY);
        }

        public async void AddCategory()
        {
            StringEditorViewModel viewModel = new StringEditorViewModel();
            viewModel.Title = (string)App.Current.Resources["VIEW_STRINGEDITOR_ADD_CATEGORY_TITLE"];
            viewModel.Caption = (string)App.Current.Resources["VIEW_STRINGEDITOR_ADD_CATEGORY_CAPTION"];
            viewModel.Text = string.Empty;

            bool? dialogResult = await IoC.Get<IWindowManager>().ShowDialogAsync(viewModel).ConfigureAwait(false);

            if (dialogResult == true)
            {
                if (!string.IsNullOrEmpty(viewModel.Text))
                {
                    Category temp = App.DatabaseContext.Categories.Where(c => c.Name == viewModel.Text).FirstOrDefault();

                    if (temp == null)
                    {
                        using (var transaction = await App.DatabaseContext.Database.BeginTransactionAsync().ConfigureAwait(false))
                        {
                            Category category = new Category(viewModel.Text);

                            await App.DatabaseContext.Categories.AddAsync(category).ConfigureAwait(false);
                            await App.DatabaseContext.SaveChangesAsync().ConfigureAwait(false);

                            await _libraryService.UpdateSelectedCategoryAsync(category.SystemId).ConfigureAwait(false);
                            await _libraryService.UpdateGamesAsync(category.SystemId).ConfigureAwait(false);

                            await transaction.CommitAsync().ConfigureAwait(false);
                        }
                    }
                    else
                    {
                        MessageBox.Show((string)App.Current.Resources["VM_LIBRARY_CATEGORY_EXISTS_MSG"], "Kimera", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                    }
                }
                else
                {
                    MessageBox.Show((string)App.Current.Resources["VM_LIBRARY_CATEGORY_EMPTY_NAME_MSG"], "Kimera", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }
            }
        }

        public async void RenameCategory()
        {
            CategoryNameEditorViewModel viewModel = new CategoryNameEditorViewModel();
            viewModel.Title = (string)App.Current.Resources["VIEW_CATEGORYNAMEEDITOR_TITLE"];
            viewModel.Caption = (string)App.Current.Resources["VIEW_CATEGORYENAMEEDITOR_CAPTION"];
            viewModel.Categories = _libraryService.Categories;
            viewModel.SelectedCategory = _libraryService.SelectedCategory == Settings.GUID_ALL_CATEGORY || _libraryService.SelectedCategory == Settings.GUID_FAVORITE_CATEGORY
                ? _libraryService.Categories.FirstOrDefault() : _libraryService.Categories.Where(c => c.SystemId == _libraryService.SelectedCategory).FirstOrDefault();

            bool? dialogResult = await IoC.Get<IWindowManager>().ShowDialogAsync(viewModel).ConfigureAwait(false);

            if (dialogResult == true)
            {
                if (!string.IsNullOrEmpty(viewModel.Text))
                {
                    if (viewModel.SelectedCategory != null)
                    {
                        using (var transaction = await App.DatabaseContext.Database.BeginTransactionAsync().ConfigureAwait(false))
                        {
                            viewModel.SelectedCategory.Name = viewModel.Text;
                            await App.DatabaseContext.SaveChangesAsync().ConfigureAwait(false);

                            await _libraryService.UpdateCategoriesAsync().ConfigureAwait(false);
                            await _libraryService.UpdateSelectedCategoryAsync(_libraryService.SelectedCategory).ConfigureAwait(false);

                            await transaction.CommitAsync().ConfigureAwait(false);
                        }
                    }
                    else
                    {
                        MessageBox.Show((string)App.Current.Resources["VM_LIBRARY_CATEGORY_NOT_FOUND_MSG"], "Kimera", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                    }
                }
                else
                {
                    MessageBox.Show((string)App.Current.Resources["VM_LIBRARY_CATEGORY_EMPTY_NAME_MSG"], "Kimera", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }
            }
        }

        public async void RemoveCategory()
        {
            CategorySelectorViewModel viewModel = new CategorySelectorViewModel();
            viewModel.Title = (string)App.Current.Resources["VIEW_CATEGORYSELECTOR_REMOVE_CATEGORY_TITLE"];
            viewModel.Caption = (string)App.Current.Resources["VIEW_CATEGORYSELECTOR_REMOVE_CATEGORY_CAPTION"];
            viewModel.Categories = _libraryService.Categories;
            viewModel.SelectedCategory = _libraryService.SelectedCategory == Settings.GUID_ALL_CATEGORY || _libraryService.SelectedCategory == Settings.GUID_FAVORITE_CATEGORY
                ? _libraryService.Categories.FirstOrDefault() : _libraryService.Categories.Where(c => c.SystemId == _libraryService.SelectedCategory).FirstOrDefault();

            bool? dialogResult = await IoC.Get<IWindowManager>().ShowDialogAsync(viewModel).ConfigureAwait(false);

            if (dialogResult == true)
            {
                if (viewModel.SelectedCategory != null)
                {
                    MessageBoxResult result = MessageBox.Show((string)App.Current.Resources["VM_LIBRARY_CATEGORY_REMOVE_CHECKER_MSG"], "Kimera", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        using (var transaction = await App.DatabaseContext.Database.BeginTransactionAsync().ConfigureAwait(false))
                        {
                            App.DatabaseContext.Categories.Remove(viewModel.SelectedCategory);
                            await App.DatabaseContext.SaveChangesAsync().ConfigureAwait(false);

                            await transaction.CommitAsync().ConfigureAwait(false);
                        }
                    }
                }
                else
                {
                    MessageBox.Show((string)App.Current.Resources["VM_LIBRARY_CATEGORY_NOT_FOUND_MSG"], "Kimera", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }
            }
        }

        public void ChangeToTileView()
        {
            ViewTemplate = App.Current.FindResource("TileViewItemTemplate") as DataTemplate;
        }

        public void ChangeToIconView()
        {
            ViewTemplate = App.Current.FindResource("IconViewItemTemplate") as DataTemplate;
        }

        public void NavigateToGameRegister()
        {
            IoC.Get<INavigationService>().NavigateToViewModel(typeof(GameRegisterViewModel));
        }

        #endregion
    }
}
