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
        private LibraryService _service = LibraryService.Instance;

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
                RaisePropertyChanged();
            }
        }

        private Guid _currentCategory;

        public Guid CurrentCategory
        {
            get
            {
                return _currentCategory;
            }
            set
            {
                _currentCategory = value;
                RaisePropertyChanged();
            }
        }

        private ObservableCollection<GameMetadata> _games = new ObservableCollection<GameMetadata>();

        public ObservableCollection<GameMetadata> Games
        {
            get
            {
                return _games;
            }
            set
            {
                _games = value;
                RaisePropertyChanged();
            }
        }

        public DelegateCommand AddExecutableFileCommand { get; }

        public DelegateCommand AddArchiveFileCommand { get; }

        public DelegateCommand AddMultipleFileCommand { get; }

        public DelegateCommand AddFolderCommand { get; }

        public LibraryViewModel()
        {
            foreach (Category category in App.DatabaseContext.Categories)
            {
                Categories.Add(category);
            }

            _service.GamesChangedEvent += OnGamesChangedEvent;
            _service.UpdateGames(Categories.FirstOrDefault().SystemId);

            AddExecutableFileCommand = new DelegateCommand(AddExecutableFileDialog);
            AddArchiveFileCommand = new DelegateCommand(AddArchiveFileDialog);
            AddMultipleFileCommand = new DelegateCommand(AddMultipleFileDialog);
            AddFolderCommand = new DelegateCommand(AddFolderDialog);
        }

        private async void OnGamesChangedEvent(object sender, EventArgs e)
        {
            var task = Task.Factory.StartNew(() =>
            {
                _games.Clear();

                foreach (Game game in _service.Games)
                {
                    _games.Add(game.GameMetadataNavigation);
                }
            });

            await task.ConfigureAwait(false);
        }

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
    }
}
