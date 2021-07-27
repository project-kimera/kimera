using Kimera.Commands;
using Kimera.Data.Entities;
using Kimera.Dialogs;
using Kimera.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public DelegateCommand AddExecutableFileCommand { get; }

        public DelegateCommand AddArchiveFileCommand { get; }

        public DelegateCommand AddMultipleFileCommand { get; }

        public DelegateCommand AddFolderCommand { get; }

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

        private void AddExecutableFileDialog()
        {
            AddExecutableFileDialog dialog = new AddExecutableFileDialog();
            dialog.ShowDialog();
        }

        private void AddArchiveFileDialog()
        {
            AddArchiveFileDialog dialog = new AddArchiveFileDialog();
            dialog.ShowDialog();
        }

        private void AddMultipleFileDialog()
        {
            AddMultipleFileDialog dialog = new AddMultipleFileDialog();
            dialog.ShowDialog();
        }

        private void AddFolderDialog()
        {
            AddFolderDialog dialog = new AddFolderDialog();
            dialog.ShowDialog();
        }

        #endregion

        #region ::Constructors::

        public LibraryViewModel()
        {
            _service.CategoriesChangedEvent += OnCategoriesChanged;
            _service.SelectedCategoryChangedEvent += OnSelectedCategoryChanged;
            _service.GamesChangedEvent += OnGamesChanged;

            AddExecutableFileCommand = new DelegateCommand(AddExecutableFileDialog);
            AddArchiveFileCommand = new DelegateCommand(AddArchiveFileDialog);
            AddMultipleFileCommand = new DelegateCommand(AddMultipleFileDialog);
            AddFolderCommand = new DelegateCommand(AddFolderDialog);
        }

        #endregion
    }
}
