using Caliburn.Micro;
using Kimera.Data.Entities;
using Kimera.Data.Enums;
using Kimera.Messages;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;

namespace Kimera.ViewModels.Pages
{
    public class GameViewModel : Screen
    {
        #region ::Aasfeqwt::

        private Game _game = new Game();

        public Game Game
        {
            get => _game;
            set => Set(ref _game, value);
        }

        public string Name
        {
            get => _game?.GameMetadataNavigation.Name;
        }

        public string Description
        {
            get => _game?.GameMetadataNavigation.Description;
        }

        public string Creator
        {
            get => _game?.GameMetadataNavigation.Creator;
        }

        public Age AdmittedAge
        {
            get => _game.GameMetadataNavigation.AdmittedAge;
        }

        public List<string> Genres
        {
            get
            {
                List<string> temp = _game?.GameMetadataNavigation.Genres.Split(',').ToList();
                return temp;
            }
        }

        public List<string> Tags
        {
            get
            {
                List<string> temp = _game?.GameMetadataNavigation.Tags.Split(',').ToList();
                return temp;
            }
        }

        public string SupportedLanguages
        {
            get => _game?.GameMetadataNavigation.SupportedLanguages;
        }

        public double Score
        {
            get => _game.GameMetadataNavigation.Score;
        }

        public string Memo
        {
            get => _game?.GameMetadataNavigation.Memo;
        }

        public int PlayTime
        {
            get => _game.GameMetadataNavigation.PlayTime;
        }

        public DateTime FirstTime
        {
            get => _game.GameMetadataNavigation.FirstTime;
        }

        public DateTime LastTime
        {
            get => _game.GameMetadataNavigation.LastTime;
        }

        public string ThumbnailUri
        {
            get => _game?.GameMetadataNavigation.ThumbnailUri;
        }

        public string HomepageUrl
        {
            get => _game?.GameMetadataNavigation.HomepageUrl;
        }

        public PackageStatus PackageStatus
        {
            get => _game.PackageStatus;
        }

        #endregion

        public GameViewModel(Guid gameGuid)
        {
            InitializeGame(gameGuid);
        }

        public void InitializeGame(Guid gameGuid)
        {
            Game game = App.DatabaseContext.Games.Where(g => g.SystemId == gameGuid).FirstOrDefault();

            if (game != null)
            {
                Game = game;
            }
            else
            {
                Game = new Game();
            }
        }

        public void GoBack()
        {
            INavigationService navigationService = IoC.Get<INavigationService>();

            if (navigationService.CanGoBack)
            {
                navigationService.GoBack();
            }
        }
    }
}
