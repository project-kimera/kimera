using Caliburn.Micro;
using Kimera.Data.Entities;
using Kimera.ViewModels.Pages;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kimera.Services
{
    public class GameService : PropertyChangedBase
    {
        #region ::Variables & Properties::

        private bool _isRunning = false;

        public bool IsRunning
        {
            get => _isRunning;
            set => Set(ref _isRunning, value);
        }

        #endregion

        #region ::Methods::



        #endregion

        #region ::Actions::

        public void StartGame(Guid gameGuid)
        {

        }

        public void MoveGame(Guid gameGuid)
        {

        }

        public void RemoveGameResources(Guid gameGuid)
        {

        }

        public void RemoveGame(Guid gameGuid)
        {

        }

        public async void ViewGame(Guid gameGuid)
        {
            Game game = await App.DatabaseContext.Games.Where(g => g.SystemId == gameGuid).FirstOrDefaultAsync();

            if (game != null)
            {
                INavigationService navigationService = IoC.Get<INavigationService>();
                navigationService.For<GameViewModel>()
                    .WithParam(g => g.Game, game)
                    .Navigate();
            }
        }

        public void CallMetadataEditor(Guid gameGuid)
        {

        }

        public void CallSettingsEditor(Guid gameGuid)
        {

        }

        #endregion
    }
}
