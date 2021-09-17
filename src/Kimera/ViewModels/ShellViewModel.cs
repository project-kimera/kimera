using Caliburn.Micro;
using Kimera.Services;
using Kimera.ViewModels.Dialogs;
using Kimera.ViewModels.Pages;
using Kimera.ViewModels.Specials;
using System.Windows.Controls;

namespace Kimera.ViewModels
{
    public class ShellViewModel : Conductor<IScreen>.Collection.OneActive
    {
        private readonly SimpleContainer _container;
        private INavigationService _navigationService;

        private bool _clearTitleBar = true;

        public bool ClearTitleBar
        {
            get => _clearTitleBar;
            set => Set(ref _clearTitleBar, value);
        }

        public ShellViewModel(SimpleContainer container)
        {
            _container = container;
        }

        public void RegisterFrame(Frame frame)
        {
            _navigationService = new FrameAdapter(frame);

            _container.Instance(_navigationService);

            _navigationService.NavigateToViewModel(typeof(GuardViewModel));
        }

        public void NavigateToSearcher()
        {
            _navigationService.NavigateToViewModel(typeof(SearcherViewModel));
        }

        public void NavigateToLibrary()
        {
            _navigationService.NavigateToViewModel(typeof(LibraryViewModel));
        }

        public void NavigateToStatistics()
        {
            _navigationService.NavigateToViewModel(typeof(StatisticsViewModel));
        }

        public async void NavigateToSettings()
        {
            SettingsViewModel viewModel = new SettingsViewModel();

            bool? dialogResult = await IoC.Get<IWindowManager>().ShowDialogAsync(viewModel).ConfigureAwait(false);
        }
    }
}
