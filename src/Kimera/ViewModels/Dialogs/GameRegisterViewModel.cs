using Caliburn.Micro;
using Kimera.Data.Entities;
using Kimera.Entities;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kimera.ViewModels.Dialogs
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

        public async void EditPackageMetadata(PackageMetadata packageMetadata)
        {
            PackageMetadataEditorViewModel viewModel = new PackageMetadataEditorViewModel(packageMetadata, true);

            bool? dialogResult = await IoC.Get<IWindowManager>().ShowDialogAsync(viewModel).ConfigureAwait(false);

            if (dialogResult == true)
            {
                packageMetadata = viewModel.Metadata;
            }
        }

        public async void EditGameMetadata(GameMetadata gameMetadata)
        {
            GameMetadataEditorViewModel viewModel = new GameMetadataEditorViewModel(gameMetadata, true);

            bool? dialogResult = await IoC.Get<IWindowManager>().ShowDialogAsync(viewModel).ConfigureAwait(false);

            if (dialogResult == true)
            {
                gameMetadata = viewModel.Metadata;
            }
        }

        public async void AddFiles()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "Open";
            dialog.Filter = "Supported Files|*.zip;*.7z;*.rar";
            dialog.Multiselect = false;

            if (dialog.ShowDialog() == true)
            {
                //FilePath = dialog.FileName;

                //string productCode = MetadataServiceProvider.GetProductCodeFromPath(FilePath);

                //if (string.IsNullOrEmpty(productCode))
                {
                    //MessageBox.Show("상품 코드를 가져올 수 없습니다. 수동으로 게임 메타데이터를 입력해주세요.", "Kimera", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                //else
                {
                    //ProductCode = productCode;
                }
            }
        }

        public async void AddChunk()
        {
            GameRegistration registration = new GameRegistration();
            Registrations.Add(registration);
        }

        public async void Remove()
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
