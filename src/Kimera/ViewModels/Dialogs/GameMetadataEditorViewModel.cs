using Caliburn.Micro;
using Kimera.Data.Entities;
using Kimera.Data.Enums;
using Kimera.Network;
using Kimera.Network.Entities;
using Kimera.Network.Services.Interfaces;
using Kimera.Utilities;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Kimera.ViewModels.Dialogs
{
    public class GameMetadataEditorViewModel : Screen
    {
        private bool _isRegistered = false;

        public bool IsRegistered
        {
            get => _isRegistered;
        }

        private GameMetadata _metadata = new GameMetadata();

        public GameMetadata Metadata
        {
            get => _metadata;
        }

        private bool _isWorking = false;

        public bool IsWorking
        {
            get => _isWorking;
            set => Set(ref _isWorking, value);
        }

        public List<ISearchService> SearchServices
        {
            get => SearchServiceProvider.Services;
        }

        private ISearchService _selectedSearchService = null;

        public ISearchService SelectedSearchService
        {
            get => _selectedSearchService;
            set => Set(ref _selectedSearchService, value);
        }

        private string _searchKeywords = string.Empty;

        public string SearchKeywords
        {
            get => _searchKeywords;
            set
            {
                Set(ref _searchKeywords, value);
            }
        }

        private BindableCollection<SearchResult> _suggestions = new BindableCollection<SearchResult>();

        public BindableCollection<SearchResult> Suggestions
        {
            get => _suggestions;
            set => Set(ref _suggestions, value);
        }

        private SearchResult _selectedSuggestion = null;

        public SearchResult SelectedSuggestion
        {
            get => _selectedSuggestion;
            set
            {
                Set(ref _selectedSuggestion, value);

                if (value != null)
                {
                    LoadMetadata(value.ProductCode);
                }
            }
        }

        public string Name
        {
            get => _metadata.Name;
            set
            {
                _metadata.Name = value;
                NotifyOfPropertyChange("Name");
            }
        }

        public string Description
        {
            get => _metadata.Description;
            set
            {
                _metadata.Description = value;
                NotifyOfPropertyChange("Description");
            }
        }

        public string Creator
        {
            get => _metadata.Creator;
            set
            {
                _metadata.Creator = value;
                NotifyOfPropertyChange("Creator");
            }
        }

        public Age AdmittedAge
        {
            get => _metadata.AdmittedAge;
            set
            {
                _metadata.AdmittedAge = value;
                NotifyOfPropertyChange("AdmittedAge");
            }
        }

        public string Genres
        {
            get => _metadata.Genres;
            set
            {
                _metadata.Genres = value;
                NotifyOfPropertyChange("Genres");
            }
        }

        public string Tags
        {
            get => _metadata.Tags;
            set
            {
                _metadata.Tags = value;
                NotifyOfPropertyChange("Tags");
            }
        }

        public string SupportedLanguages
        {
            get => _metadata.SupportedLanguages;
            set
            {
                _metadata.SupportedLanguages = value;
                NotifyOfPropertyChange("SupportedLanguages");
            }
        }

        public double Score
        {
            get => _metadata.Score;
            set
            {
                _metadata.Score = value;
                NotifyOfPropertyChange("Score");
            }
        }

        public string IconUri
        {
            get => _metadata.IconUri;
            set
            {
                _metadata.IconUri = value;
                NotifyOfPropertyChange("IconUri");
            }
        }

        public string ThumbnailUri
        {
            get => _metadata.ThumbnailUri;
            set
            {
                _metadata.ThumbnailUri = value;
                NotifyOfPropertyChange("ThumbnailUri");
            }
        }

        public string HomepageUrl
        {
            get => _metadata.HomepageUrl;
            set
            {
                _metadata.HomepageUrl = value;
                NotifyOfPropertyChange("HomepageUrl");
            }
        }

        public GameMetadataEditorViewModel(GameMetadata metadata)
        {
            _selectedSearchService = SearchServices.FirstOrDefault();

            InitializeGameMetadata(metadata);
        }

        private void InitializeGameMetadata(GameMetadata metadata)
        {
            try
            {
                var temp = App.DatabaseContext.GameMetadatas.Where(p => p.SystemId == metadata.SystemId).FirstOrDefault();

                if (temp == null)
                {
                    _isRegistered = false;
                    _metadata = metadata;
                }
                else
                {
                    _isRegistered = true;
                    _metadata = metadata.Copy();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while loading the components data table.");
            }
        }

        public async void Search()
        {
            IsWorking = true;

            if (_selectedSearchService != null)
            {
                List<SearchResult> results = await _selectedSearchService.GetSearchResultsAsync(_searchKeywords);

                Suggestions.Clear();
                Suggestions.AddRange(results);
            }

            IsWorking = false;
        }

        public async void LoadMetadata(string productCode)
        {
            IsWorking = true;

            Type type;
            GameMetadata metadata;

            if (MetadataServiceProvider.TryValidProductCode(productCode, out type))
            {
                metadata = await MetadataServiceProvider.GetMetadataAsync(type, productCode);

                if (metadata != null)
                {
                    Name = metadata.Name;
                    Description = metadata.Description;
                    Creator = metadata.Creator;
                    Genres = metadata.Genres;
                    ThumbnailUri = metadata.ThumbnailUri;
                    IconUri = metadata.IconUri;
                    HomepageUrl = metadata.HomepageUrl;
                    AdmittedAge = metadata.AdmittedAge;
                    Tags = metadata.Tags;
                    SupportedLanguages = metadata.SupportedLanguages;
                    Score = metadata.Score;
                }
            }

            IsWorking = false;
        }

        public async void Cancel()
        {
            await TryCloseAsync(false);
        }

        public async void Confirm(Window window)
        {
            if (window == null)
            {
                Log.Warning("The game metadata values shouldn't be null.");
                return;
            }

            if (!ValidationHelper.IsValid(window))
            {
                MessageBox.Show((string)App.Current.Resources["VM_GAMEMETADATAEDITOR_INVALID_MD_MSG"], "Kimera", MessageBoxButton.OK, MessageBoxImage.Error);
                Log.Warning("The game metadata values aren't valid.");
                return;
            }

            await TryCloseAsync(true);
        }
    }
}
