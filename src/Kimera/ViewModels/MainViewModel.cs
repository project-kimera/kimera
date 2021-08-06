using Kimera.Common.Commands;
using Kimera.Services;
using Kimera.Utilities;
using Kimera.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Kimera.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private LibraryPage _libraryPage = new LibraryPage();

        private StatisticsPage _statisticsPage = new StatisticsPage();

        private SettingsPage _settingsPage = new SettingsPage();

        private string _searchText = string.Empty;

        public string SearchText
        {
            get
            {
                return _searchText;
            }
            set
            {
                _searchText = value;
                RaisePropertyChanged();
            }
        }

        public DelegateCommand SearchCommand { get; }

        public DelegateCommand NavigateToLibraryCommand { get; }

        public DelegateCommand NavigateToStatisticsCommand { get; }

        public DelegateCommand NavigateToSettingsCommand { get; }

        public MainViewModel(Frame shellFrame)
        {
            NavigationService helper = NavigationService.Instance;
            helper.InitializeFrame(shellFrame);
            helper.NavigateTo(_libraryPage);

            SearchCommand = new DelegateCommand(Search);
            NavigateToLibraryCommand = new DelegateCommand(NavigateToLibrary);
            NavigateToStatisticsCommand = new DelegateCommand(NavigateToStatistics);
            NavigateToSettingsCommand = new DelegateCommand(NavigateToSettings);
        }

        private void Search()
        {
            if (!string.IsNullOrEmpty(_searchText))
            {
                NavigationService helper = NavigationService.Instance;
                helper.NavigateTo(new SearchPage(SearchText));

                SearchText = string.Empty;
            }
        }

        private void NavigateToLibrary()
        {
            NavigationService helper = NavigationService.Instance;
            helper.NavigateTo(_libraryPage);
        }

        private void NavigateToStatistics()
        {
            NavigationService helper = NavigationService.Instance;
            helper.NavigateTo(_statisticsPage);
        }

        private void NavigateToSettings()
        {
            NavigationService helper = NavigationService.Instance;
            helper.NavigateTo(_settingsPage);
        }
    }
}
