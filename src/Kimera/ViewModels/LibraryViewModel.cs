using Kimera.Common.Commands;
using Kimera.Data.Entities;
using Kimera.Dialogs;
using Kimera.Pages;
using Kimera.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Kimera.ViewModels
{
    public class LibraryViewModel : ViewModelBase
    {
        #region ::Variables & Properties::

        private LibraryService _service = LibraryService.Instance;

        public ObservableCollection<Category> Categories
        {
            get
            {
                return _service.Categories;
            }
        }

        public Guid SelectedCategory
        {
            get
            {
                return _service.SelectedCategory;
            }
        }

        public ObservableCollection<Game> Games
        {
            get
            {
                return _service.Games;
            }
        }

        private ObservableCollection<Game> _filteredGames = new ObservableCollection<Game>();

        public ObservableCollection<Game> FilteredGames
        {
            get
            {
                return _filteredGames;
            }
            set
            {
                _filteredGames = value;
                RaisePropertyChanged();
            }
        }

        private DataTemplate _viewTemplate = App.Current.FindResource("TileViewItemTemplate") as DataTemplate;

        public DataTemplate ViewTemplate
        {
            get
            {
                return _viewTemplate;
            }
            set
            {
                _viewTemplate = value;
                RaisePropertyChanged();
            }
        }

        public DelegateCommand RefreshGamesCommand { get; }

        public DelegateCommand ChangeToAllCategoryCommand { get; }

        public DelegateCommand ChangeToFavoriteCategoryCommand { get; }

        public DelegateCommand AddCategoryCommand { get; }

        public DelegateCommand RenameCategoryCommand { get; }

        public DelegateCommand RemoveCategoryCommand { get; }

        public DelegateCommand ChangeToTileViewCommand { get; }

        public DelegateCommand ChangeToIconViewCommand { get; }

        public DelegateCommand ShowAddExecutableFilePageCommand { get; }

        public DelegateCommand ShowAddArchiveFilePageCommand { get; }

        public DelegateCommand ShowAddMultipleFilePageCommand { get; }

        public DelegateCommand ShowAddFolderPageCommand { get; }

        #endregion

        #region ::Event Subscribers::

        private void OnCategoriesChanged(object sender, EventArgs e)
        {
            RaisePropertyChanged("Categories");
        }

        private void OnSelectedCategoryChanged(object sender, EventArgs e)
        {
            RaisePropertyChanged("SelectedCategory");
        }

        private void OnGamesChanged(object sender, EventArgs e)
        {
            RaisePropertyChanged("Games");
        }

        #endregion

        #region ::Command Actions::

        private async void RefreshGames()
        {
            await LibraryService.Instance.UpdateGamesAsync(LibraryService.Instance.SelectedCategory);
        }

        private async void ChangeToAllCategory()
        {
            await LibraryService.Instance.UpdateSelectedCategoryAsync(Settings.GUID_ALL_CATEGORY);
            await LibraryService.Instance.UpdateGamesAsync(Settings.GUID_ALL_CATEGORY);
        }

        private async void ChangeToFavoriteCategory()
        {
            await LibraryService.Instance.UpdateSelectedCategoryAsync(Settings.GUID_FAVORITE_CATEGORY);
            await LibraryService.Instance.UpdateGamesAsync(Settings.GUID_FAVORITE_CATEGORY);
        }

        private async void AddCategory()
        {
            EditStringDialog dialog = new EditStringDialog();
            dialog.Title = "카테고리 추가";
            dialog.ViewModel.Caption = "추가할 카테고리의 이름을 입력해주세요.";
            
            if (dialog.ShowDialog() == true)
            {
                if (!string.IsNullOrEmpty(dialog.ViewModel.Text))
                {
                    Category temp = App.DatabaseContext.Categories.Where(c => c.Name == dialog.ViewModel.Text).FirstOrDefault();

                    if (temp == null)
                    {
                        using (var transaction = await App.DatabaseContext.Database.BeginTransactionAsync())
                        {
                            Category category = new Category(dialog.ViewModel.Text);

                            await App.DatabaseContext.Categories.AddAsync(category);
                            await App.DatabaseContext.SaveChangesAsync();

                            await LibraryService.Instance.UpdateSelectedCategoryAsync(category.SystemId);
                            await LibraryService.Instance.UpdateGamesAsync(category.SystemId);

                            await transaction.CommitAsync();
                        }
                    }
                    else
                    {
                        MessageBox.Show("이미 존재하는 카테고리입니다.", "Kimera", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("카테고리 이름을 입력해주세요.", "Kimera", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }
            }
        }

        private async void RenameCategory()
        {
            EditCategoryNameDialog dialog = new EditCategoryNameDialog();
            dialog.Title = "카테고리 이름 변경";
            dialog.ViewModel.Caption = "이름을 변경할 카테고리를 선택 후 이름을 수정해주세요.";
            dialog.ViewModel.Categories = LibraryService.Instance.Categories;
            dialog.ViewModel.SelectedCategory = LibraryService.Instance.SelectedCategory == Settings.GUID_ALL_CATEGORY || LibraryService.Instance.SelectedCategory == Settings.GUID_FAVORITE_CATEGORY
                ? LibraryService.Instance.Categories.FirstOrDefault() : LibraryService.Instance.Categories.Where(c => c.SystemId == LibraryService.Instance.SelectedCategory).FirstOrDefault();

            if (dialog.ShowDialog() == true)
            {
                if (!string.IsNullOrEmpty(dialog.ViewModel.Text))
                {
                    if (dialog.ViewModel.SelectedCategory != null)
                    {
                        using (var transaction = await App.DatabaseContext.Database.BeginTransactionAsync())
                        {
                            dialog.ViewModel.SelectedCategory.Name = dialog.ViewModel.Text;
                            await App.DatabaseContext.SaveChangesAsync();

                            await LibraryService.Instance.UpdateCategoriesAsync();
                            await LibraryService.Instance.UpdateSelectedCategoryAsync(LibraryService.Instance.SelectedCategory);

                            await transaction.CommitAsync();
                        }
                    }
                    else
                    {
                        MessageBox.Show("존재하지 않는 카테고리입니다.", "Kimera", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("카테고리 이름을 입력해주세요.", "Kimera", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }
            }
        }

        private async void RemoveCategory()
        {
            SelectCategoryDialog dialog = new SelectCategoryDialog();
            dialog.Title = "카테고리 제거";
            dialog.ViewModel.Caption = "제거할 카테고리를 선택해주세요.";
            dialog.ViewModel.Categories = LibraryService.Instance.Categories;
            dialog.ViewModel.SelectedCategory = LibraryService.Instance.SelectedCategory == Settings.GUID_ALL_CATEGORY || LibraryService.Instance.SelectedCategory == Settings.GUID_FAVORITE_CATEGORY
                ? LibraryService.Instance.Categories.FirstOrDefault() : LibraryService.Instance.Categories.Where(c => c.SystemId == LibraryService.Instance.SelectedCategory).FirstOrDefault();

            if (dialog.ShowDialog() == true)
            {
                if (dialog.ViewModel.SelectedCategory != null)
                {
                    MessageBoxResult result =  MessageBox.Show("정말 카테고리를 삭제하시겠습니까? 이 작업은 되돌릴 수 없습니다.", "Kimera", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        using (var transaction = await App.DatabaseContext.Database.BeginTransactionAsync())
                        {
                            App.DatabaseContext.Categories.Remove(dialog.ViewModel.SelectedCategory);
                            await App.DatabaseContext.SaveChangesAsync();

                            await transaction.CommitAsync();
                        }
                    }
                }
                else
                {
                    MessageBox.Show("존재하지 않는 카테고리입니다.", "Kimera", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }
            }
        }

        private void ChangeToTileView()
        {
            ViewTemplate = App.Current.FindResource("TileViewItemTemplate") as DataTemplate;
        }

        private void ChangeToIconView()
        {
            ViewTemplate = App.Current.FindResource("IconViewItemTemplate") as DataTemplate;
        }

        private void ShowAddExecutableFilePage()
        {
            AddExecutableFilePage page = new AddExecutableFilePage();
            NavigationService.Instance.NavigateTo(page);
        }

        private void ShowAddArchiveFilePage()
        {
            AddArchiveFilePage page = new AddArchiveFilePage();
            NavigationService.Instance.NavigateTo(page);
        }

        private void ShowAddMultipleFilePage()
        {
            AddMultipleFilesPage page = new AddMultipleFilesPage();
            NavigationService.Instance.NavigateTo(page);
        }

        private void ShowAddFolderPage()
        {
            AddGamesFromFolderPage page = new AddGamesFromFolderPage();
            NavigationService.Instance.NavigateTo(page);
        }

        #endregion

        #region ::Constructors::

        public LibraryViewModel()
        {
            _service.CategoriesChangedEvent += OnCategoriesChanged;
            _service.SelectedCategoryChangedEvent += OnSelectedCategoryChanged;
            _service.GamesChangedEvent += OnGamesChanged;

            RefreshGamesCommand = new DelegateCommand(RefreshGames);

            ChangeToAllCategoryCommand = new DelegateCommand(ChangeToAllCategory);
            ChangeToFavoriteCategoryCommand = new DelegateCommand(ChangeToFavoriteCategory);
            AddCategoryCommand = new DelegateCommand(AddCategory);
            RenameCategoryCommand = new DelegateCommand(RenameCategory);
            RemoveCategoryCommand = new DelegateCommand(RemoveCategory);

            ChangeToTileViewCommand = new DelegateCommand(ChangeToTileView);
            ChangeToIconViewCommand = new DelegateCommand(ChangeToIconView);

            ShowAddExecutableFilePageCommand = new DelegateCommand(ShowAddExecutableFilePage);
            ShowAddArchiveFilePageCommand = new DelegateCommand(ShowAddArchiveFilePage);
            ShowAddMultipleFilePageCommand = new DelegateCommand(ShowAddMultipleFilePage);
            ShowAddFolderPageCommand = new DelegateCommand(ShowAddFolderPage);
        }

        #endregion
    }
}
