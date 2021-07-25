using Kimera.Data.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kimera.Services
{
    public class LibraryService
    {
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

        public delegate void GamesChangedEventHandler(object sender, EventArgs e);

        public event GamesChangedEventHandler GamesChangedEvent;

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
            }
        }

        private List<Game> _games = new List<Game>();

        public List<Game> Games
        {
            get
            {
                return _games;
            }
        }

        public void UpdateGames(Guid categoryGuid)
        {
            List<CategorySubscription> subscriptions = App.DatabaseContext.CategorySubscriptions.Where(c => c.Category == categoryGuid).ToList();

            var result = subscriptions.Select(s => s.GameNavigation);
            _games =  result.ToList();

            GamesChangedEvent?.Invoke(this, new EventArgs());
        }

        public async Task UpdateGamesAsync(Guid categoryGuid)
        {
            var task = Task.Factory.StartNew(() =>
            {
                List<CategorySubscription> subscriptions = App.DatabaseContext.CategorySubscriptions.Where(c => c.Category == categoryGuid).ToList();

                var result = subscriptions.Select(s => s.GameNavigation);

                GamesChangedEvent?.Invoke(this, new EventArgs());
            });

            await task.ConfigureAwait(false);
        }
    }
}
