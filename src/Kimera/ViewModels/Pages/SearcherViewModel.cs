using Caliburn.Micro;
using Kimera.Data.Entities;
using Kimera.Entities;
using Kimera.Messages;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;

namespace Kimera.ViewModels.Pages
{
    public class SearcherViewModel : Screen
    {
        private string _textToSearch = string.Empty;

        public string TextToSearch
        {
            get => _textToSearch;
            set => Set(ref _textToSearch, value);
        }

        private List<SearchCategory> _searchCategories = new List<SearchCategory>();

        public List<SearchCategory> SearchCategories
        {
            get => _searchCategories;
            set => Set(ref _searchCategories, value);
        }

        private SearchCategory _searchCategory = SearchCategory.All;

        public SearchCategory SearchCategory
        {
            get => _searchCategory;
            set => Set(ref _searchCategory, value);
        }

        private BindableCollection<GameMetadata> _searchResults = new BindableCollection<GameMetadata>();

        public BindableCollection<GameMetadata> SearchResults
        {
            get => _searchResults;
            set => Set(ref _searchResults, value);
        }

        public SearcherViewModel()
        {
            SearchCategories = Enum.GetValues(typeof(SearchCategory)).Cast<SearchCategory>().ToList();
            Search();
        }

        private async Task SearchInternalAsync(SearchCategory searchCategory, string searchText)
        {
            try
            {
                string[] parameters = searchText.Split(' ', '　');

                var predicate = PredicateBuilder.New<GameMetadata>();

                if (searchCategory == SearchCategory.All)
                {

                    foreach (string param in parameters)
                    {
                        predicate.And(
                            g =>
                            g.Name.Contains(param) ||
                            g.Description.Contains(param) ||
                            g.Creator.Contains(param) ||
                            g.Genres.Contains(param) ||
                            g.Tags.Contains(param) ||
                            g.Memo.Contains(param));
                    }
                }
                else if (searchCategory == SearchCategory.Name)
                {
                    foreach (string param in parameters)
                    {
                        predicate.And(g => g.Name.Contains(param));
                    }
                }
                else if (searchCategory == SearchCategory.Description)
                {
                    foreach (string param in parameters)
                    {
                        predicate.And(g => g.Description.Contains(param));
                    }
                }
                else if (searchCategory == SearchCategory.Creator)
                {
                    foreach (string param in parameters)
                    {
                        predicate.And(g => g.Creator.Contains(param));
                    }
                }
                else if (searchCategory == SearchCategory.Genres)
                {
                    foreach (string param in parameters)
                    {
                        predicate.And(g => g.Genres.Contains(param));
                    }
                }
                else if (searchCategory == SearchCategory.Tags)
                {
                    foreach (string param in parameters)
                    {
                        predicate.And(g => g.Tags.Contains(param));
                    }
                }
                else if (searchCategory == SearchCategory.Memo)
                {
                    foreach (string param in parameters)
                    {
                        predicate.And(g => g.Memo.Contains(param));
                    }
                }

                List<GameMetadata> game = await App.DatabaseContext.GameMetadatas
                    .AsNoTracking()
                    .AsExpandable()
                    .Where(predicate)
                    .ToListAsync()
                    .ConfigureAwait(false);

                SearchResults = new BindableCollection<GameMetadata>(game);
            }
            catch
            {
                throw;
            }
        }

        public async void Search()
        {
            await SearchInternalAsync(_searchCategory, _textToSearch);
        }

        public void NavigateToGameView(Guid gameGuid)
        {
            Game game = App.DatabaseContext.Games.Where(g => g.SystemId == gameGuid).FirstOrDefault();

            if (game != null)
            {
                INavigationService navigationService = IoC.Get<INavigationService>();
                navigationService.For<GameViewModel>()
                    .WithParam(g => g.Game, game)
                    .Navigate();
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
