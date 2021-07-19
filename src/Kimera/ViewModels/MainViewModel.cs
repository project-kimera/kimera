using Kimera.Commands;
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
        public LibraryPage LibraryPage { get; set; } = new LibraryPage();

        public SettingsPage SettingsPage { get; set; } = new SettingsPage();

        public DelegateCommand NavigateToLibraryCommand { get; }

        public DelegateCommand NavigateToSettingsCommand { get; }

        public MainViewModel(Frame shellFrame)
        {
            NavigationService helper = NavigationService.Instance;
            helper.InitializeFrame(shellFrame);
            helper.NavigateTo(LibraryPage);

            NavigateToLibraryCommand = new DelegateCommand(NavigateToLibrary);
            NavigateToSettingsCommand = new DelegateCommand(NavigateToSettings);
        }

        private void NavigateToLibrary()
        {
            NavigationService helper = NavigationService.Instance;
            helper.NavigateTo(LibraryPage);
        }

        private void NavigateToSettings()
        {
            NavigationService helper = NavigationService.Instance;
            helper.NavigateTo(SettingsPage);
        }
    }
}
