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
            if (_libraryService.SelectedCategoryGuid != Guid.Empty)
            {
                await _libraryService.UpdateGamesAsync(_libraryService.SelectedCategoryGuid).ConfigureAwait(false);
            }
            else
            {
                await _libraryService.ShowAllGamesAsync();
            }
        }

        public async void EditCategory()
        {
            CategoryEditorViewModel viewModel = new CategoryEditorViewModel();

            await IoC.Get<IWindowManager>().ShowDialogAsync(viewModel).ConfigureAwait(false);
        }

        public void EditSorting()
        {

        }

        public void EditFilter()
        {

        }

        public void ChangeToTileView()
        {
            ViewTemplate = App.Current.FindResource("TileViewItemTemplate") as DataTemplate;
        }

        public void ChangeToIconView()
        {
            ViewTemplate = App.Current.FindResource("IconViewItemTemplate") as DataTemplate;
        }

        public async void NavigateToGameRegister()
        {
            GameRegisterViewModel viewModel = new GameRegisterViewModel();

            await IoC.Get<IWindowManager>().ShowDialogAsync(viewModel).ConfigureAwait(false);
        }

        #endregion
    }
}
