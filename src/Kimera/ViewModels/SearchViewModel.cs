using Kimera.Common.Commands;
using Kimera.Data.Entities;
using Kimera.Entities;
using Kimera.Pages;
using Kimera.Services;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Kimera.ViewModels
{
    public class SearchViewModel : ViewModelBase
    {

        #region ::Variables & Properties::

        private string _text = string.Empty;

        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;
                RaisePropertyChanged();
            }
        }

        public List<SearchCategory> SearchCategories { get; set; }

        public SearchCategory SearchCategory
        {
            get
            {
                return Settings.Instance.LatestSearchCategory;
            }
            set
            {
                Settings.Instance.LatestSearchCategory = value;
                RaisePropertyChanged();
                Search();
            }
        }

        private bool _useAscendingSort = true;

        public bool UseAscendingSort
        {
            get
            {
                return _useAscendingSort;
            }
            set
            {
                _useAscendingSort = value;
                RaisePropertyChanged();
            }
        }

        private ObservableCollection<GameMetadata> _searchResults = new ObservableCollection<GameMetadata>();

        public ObservableCollection<GameMetadata> SearchResults
        {
            get
            {
                return _searchResults;
            }
            set
            {
                _searchResults = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region ::Commands::

        public DelegateCommand GoBackCommand { get; }

        public DelegateCommand SearchCommand { get; }

        public RelayCommand<Guid> ShowGameInformationCommand { get; }

        #endregion

        #region ::Constructors::

        public SearchViewModel(string text)
        {
            Text = text;
            SearchCategories = Enum.GetValues(typeof(SearchCategory)).Cast<SearchCategory>().ToList();

            GoBackCommand = new DelegateCommand(GoBack);
            SearchCommand = new DelegateCommand(Search);
            ShowGameInformationCommand = new RelayCommand<Guid>(ShowGameInformation);

            Search();
        }

        #endregion

        #region ::Methods::

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

                SearchResults = new ObservableCollection<GameMetadata>(game);
            }
            catch
            {
                throw;
            }
        }

        #endregion

        #region ::Command Actions::

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

        private async void Search()
        {
            await SearchInternalAsync(Settings.Instance.LatestSearchCategory, _text);
        }

        private void ShowGameInformation(Guid gameGuid)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                NavigationService.Instance.NavigateTo(new GamePage(gameGuid));
            });
        }

        #endregion
    }
}
