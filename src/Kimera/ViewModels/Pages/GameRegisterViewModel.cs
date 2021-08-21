using Caliburn.Micro;
using Kimera.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kimera.ViewModels.Pages
{
    public class GameRegisterViewModel : Screen
    {
        private BindableCollection<GameRegistration> _registrations = new BindableCollection<GameRegistration>();

        public BindableCollection<GameRegistration> Registrations
        {
            get => _registrations;
            set => Set(ref _registrations, value);
        }

        private GameRegistration _selectedRegistration;

        public GameRegistration SelectedRegistration
        {
            get => _selectedRegistration;
            set => Set(ref _selectedRegistration, value);
        }

        private double _progressValue = 0.0;

        public double ProgressValue
        {
            get => _progressValue;
            set => Set(ref _progressValue, value);
        }

        private string _progressCaption = string.Empty;

        public string ProgressCaption
        {
            get => _progressCaption;
            set => Set(ref _progressCaption, value);
        }

        public async void AddFiles()
        {

        }

        public async void AddChunks()
        {

        }

        public void GoBack()
        {
            INavigationService navigationService = IoC.Get<INavigationService>();

            if (navigationService.CanGoBack)
            {
                navigationService.GoBack();
            }
        }

        public void Confirm()
        {

        }
    }
}
