using Kimera.Common.Commands;
using Kimera.Data.Entities;
using Kimera.Data.Enums;
using Kimera.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Kimera.ViewModels
{
    public class GameViewModel : ViewModelBase
    {
        private Game _game;

        public string Name
        {
            get
            {
                return _game.GameMetadataNavigation.Name;
            }
        }

        public string Description
        {
            get
            {
                return _game.GameMetadataNavigation.Description;
            }
        }

        public string Creator
        {
            get
            {
                return _game.GameMetadataNavigation.Creator;
            }
        }

        public Age AdmittedAge
        {
            get
            {
                return _game.GameMetadataNavigation.AdmittedAge;
            }
        }

        public List<string> Genres
        {
            get
            {
                List<string> temp = _game.GameMetadataNavigation.Genres.Split(',').ToList();
                return temp;
            }
        }

        public List<string> Tags
        {
            get
            {
                List<string> temp = _game.GameMetadataNavigation.Tags.Split(',').ToList();
                return temp;
            }
        }

        public string SupportedLanguages
        {
            get
            {
                return _game.GameMetadataNavigation.SupportedLanguages;
            }
        }

        public double Score
        {
            get
            {
                return _game.GameMetadataNavigation.Score;
            }
        }

        public string Memo
        {
            get
            {
                return _game.GameMetadataNavigation.Memo;
            }
        }

        public int PlayTime
        {
            get
            {
                return _game.GameMetadataNavigation.PlayTime;
            }
        }

        public DateTime FirstTime
        {
            get
            {
                return _game.GameMetadataNavigation.FirstTime;
            }
        }

        public DateTime LastTime
        {
            get
            {
                return _game.GameMetadataNavigation.LastTime;
            }
        }

        public string ThumbnailUri
        {
            get
            {
                return _game.GameMetadataNavigation.ThumbnailUri;
            }
        }

        public string HomepageUrl
        {
            get
            {
                return _game.GameMetadataNavigation.HomepageUrl;
            }
        }

        public PackageStatus PackageStatus
        {
            get
            {
                return _game.PackageStatus;
            }
        }

        public ICommand StartGameCommand
        {
            get
            {
                return _game.StartGameCommand;
            }
        }

        public DelegateCommand GoBackCommand { get; }

        public GameViewModel(Guid guid)
        {
            InitializeGame(guid);

            GoBackCommand = new DelegateCommand(GoBack);
        }

        private void InitializeGame(Guid guid)
        {
            Game game = App.DatabaseContext.Games.Where(g => g.SystemId == guid).FirstOrDefault();

            if (game != null)
            {
                _game = game;
            }
            else
            {
                _game = new Game();
            }
        }

        private void GoBack()
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                if (NavigationService.Instance.CanGoBack())
                {
                    NavigationService.Instance.GoBack();
                }
            });
        }
    }
}
