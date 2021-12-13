using Caliburn.Micro;
using Kimera.Entities;
using Kimera.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Kimera.ViewModels.Dialogs
{
    public class FilterEditorViewModel : Screen
    {
        private LibraryService _libraryService = IoC.Get<LibraryService>();

        private FilteringService _filteringService = IoC.Get<FilteringService>();

        private string _searchKeyword = string.Empty;

        public string SearchKeyword
        {
            get => _searchKeyword;
            set => Set(ref _searchKeyword, value);
        }

        private List<string> _suggestions = new List<string>();

        public List<string> Suggestions
        {
            get => _suggestions;
            set => Set(ref _suggestions, value);
        }

        private bool _isSuggestionsOpen = false;

        public bool IsSuggestionsOpen
        {
            get => _isSuggestionsOpen;
            set => Set(ref _isSuggestionsOpen, value);
        }

        private BindableCollection<string> _selectedFilters = new BindableCollection<string>();

        public BindableCollection<string> SelectedFilters
        {
            get => _selectedFilters;
            set => Set(ref _selectedFilters, value);
        }

        private FilteringMethod _method = FilteringMethod.And;

        public FilteringMethod Method
        {
            get => _method;
            set => Set(ref _method, value);
        }

        public FilterEditorViewModel()
        {
            InitializeSettings();
        }

        public void InitializeSettings()
        {
            SelectedFilters = _filteringService.SelectedFilters;
            Method = _filteringService.Method;
        }

        public async void AddFilter()
        {
            if (!string.IsNullOrEmpty(_searchKeyword))
            {
                bool isAvailable = _filteringService.Filters.Contains(_searchKeyword, StringComparer.OrdinalIgnoreCase);

                if (isAvailable)
                {
                    if (_selectedFilters.Contains(_searchKeyword, StringComparer.OrdinalIgnoreCase))
                    {
                        MessageBox.Show((string)App.Current.Resources["VM_FILTEREDITOR_ALREADY_USING_MSG"], "Kimera", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                    }

                    SelectedFilters.Add(_searchKeyword);
                    SearchKeyword = string.Empty;
                }
                else
                {
                    MessageBox.Show((string)App.Current.Resources["VM_FILTEREDITOR_INVALID_FILTER_MSG"], "Kimera", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            }
        }

        public async void RemoveFilter(string filter)
        {
            if (!string.IsNullOrEmpty(filter))
            {
                if (_selectedFilters.Contains(filter, StringComparer.OrdinalIgnoreCase))
                {
                    SelectedFilters.Remove(filter);
                }
                else
                {
                    MessageBox.Show((string)App.Current.Resources["VM_FILTEREDITOR_FILTER_NOT_EXIST_MSG"] + $"({filter})", "Kimera", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }
            }
        }

        public async void OnTextChanged()
        {
            IsSuggestionsOpen = true;

            if (string.IsNullOrEmpty(_searchKeyword))
            {
                Suggestions = _filteringService.Filters.ToList();
            }
            else
            {
                Suggestions = _filteringService.Filters.Where(f => f.ToLower().StartsWith(_searchKeyword.ToLower())).ToList();
            }
        }

        public async void Confirm()
        {
            _filteringService.SelectedFilters = _selectedFilters;
            _filteringService.Method = _method;

            // Refresh
            if (_libraryService.SelectedCategoryGuid != Guid.Empty)
            {
                await _libraryService.UpdateGamesAsync(_libraryService.SelectedCategoryGuid).ConfigureAwait(false);
            }
            else
            {
                await _libraryService.ShowAllGamesAsync();
            }

            await TryCloseAsync(true);
        }
    }
}
