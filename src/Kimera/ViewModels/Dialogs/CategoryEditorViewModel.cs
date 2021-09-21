using Caliburn.Micro;
using Kimera.Data.Entities;
using Kimera.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Kimera.ViewModels.Dialogs
{
    public class CategoryEditorViewModel : Screen
    {
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

        public async void ShowAllGames()
        {
            await _libraryService.ShowAllGamesAsync();
        }

        public async void ShowFavoriteGames()
        {
            await _libraryService.ShowFavoriteGamesAsync();
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
            viewModel.SelectedCategory = _libraryService.Categories.Where(c => c.SystemId == _libraryService.SelectedCategoryGuid).FirstOrDefault();

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

                            if (_libraryService.SelectedCategoryGuid != Guid.Empty)
                            {
                                await _libraryService.UpdateGamesAsync(_libraryService.SelectedCategoryGuid).ConfigureAwait(false);
                            }
                            else
                            {
                                await _libraryService.ShowAllGamesAsync();
                            }

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
            viewModel.SelectedCategory = _libraryService.Categories.Where(c => c.SystemId == _libraryService.SelectedCategoryGuid).FirstOrDefault();

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
                            List<CategorySubscription> subscriptions = App.DatabaseContext.CategorySubscriptions.Where(c => c.Category == viewModel.SelectedCategory.SystemId).ToList();

                            App.DatabaseContext.CategorySubscriptions.RemoveRange(subscriptions);

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

        public async void Confirm(Window window)
        {
            await TryCloseAsync(true);
        }
    }
}
