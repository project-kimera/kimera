using Caliburn.Micro;
using Kimera.Data.Entities;
using Kimera.Data.Enums;
using Kimera.Network;
using Kimera.Services;
using Kimera.Utilities;
using Kimera.Validators;
using Kimera.ViewModels.Dialogs;
using Microsoft.Win32;
using Serilog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Kimera.ViewModels.Pages
{
    public class SingleFileRegisterViewModel : Screen
    {
        #region ::Variables & Properties::

        private GameMetadata _gameMetadata = new GameMetadata();

        private string _filePath = string.Empty;

        public string FilePath
        {
            get => _filePath;
            set => Set(ref _filePath, value);
        }

        private string _password = string.Empty;

        public string Password
        {
            get => _password;
            set => Set(ref _password, value);
        }

        private string _entryPointFilePath = string.Empty;

        public string EntryPointFilePath
        {
            get => _entryPointFilePath;
            set => Set(ref _entryPointFilePath, value);
        }

        private string _commandlineArguments = string.Empty;

        public string CommandlineArguments
        {
            get => _commandlineArguments;
            set => Set(ref _commandlineArguments, value);
        }

        private bool _useCategory = false;

        public bool UseCategory
        {
            get => _useCategory;
            set => Set(ref _useCategory, value);
        }

        private BindableCollection<Category> _categories = new BindableCollection<Category>();

        public BindableCollection<Category> Categories
        {
            get => _categories;
            set => Set(ref _categories, value);
        }

        private Category _selectedCategory = new Category();

        public Category SelectedCategory
        {
            get => _selectedCategory;
            set => Set(ref _selectedCategory, value);
        }

        private bool _useMetadataServer = true;

        public bool UseMetadataServer
        {
            get => _useMetadataServer;
            set
            {
                Set(ref _useMetadataServer, value);
            }
        }

        private string _productCode = string.Empty;

        public string ProductCode
        {
            get => _productCode;
            set => Set(ref _productCode, value);
        }

        #endregion

        #region ::Constructors::
        
        public SingleFileRegisterViewModel()
        {
            _categories = IoC.Get<LibraryService>().Categories;

            Guid selectedCategory = IoC.Get<LibraryService>().SelectedCategory;
            _selectedCategory = App.DatabaseContext.Categories.Where(c => c.SystemId == selectedCategory).FirstOrDefault();
        }

        #endregion

        #region ::Methods::

        private async Task AddDataAsync()
        {
            try
            {
                Component mainComponent = new Component();
                mainComponent.Index = 0;
                mainComponent.FilePath = _filePath;

                if (Path.GetExtension(_filePath) == ".exe")
                {
                    mainComponent.Type = ComponentType.Executable;
                }
                else
                {
                    mainComponent.Type = ComponentType.Archive;
                }


            }
            catch (Exception ex)
            {
                Log.Warning(ex, "An exception occurred while adding a game data.");
            }
        }

        #endregion

        #region ::Command Actions::

        public void ExploreFile()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "Open";
            dialog.Filter = "Supported Files|*.exe;*.zip;*.7z;*.rar|Executable file (*.exe)|*.exe|Archive Files (*.zip, *.7z, &.rar)|*.zip;*.7z:*.rar";
            dialog.Multiselect = false;

            if (dialog.ShowDialog() == true)
            {
                FilePath = dialog.FileName;

                string productCode = MetadataServiceProvider.GetProductCodeFromPath(FilePath);

                if (string.IsNullOrEmpty(productCode))
                {
                    MessageBox.Show("상품 코드를 가져올 수 없습니다. 수동으로 게임 메타데이터를 입력해주세요.", "Kimera", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    ProductCode = productCode;
                }
            }
        }

        public async void GetGameMetadataFromProductCode()
        {
            Type serviceType;

            if (MetadataServiceProvider.TryValidProductCode(_productCode, out serviceType))
            {
                if (await MetadataServiceProvider.IsAvailableProductAsync(serviceType, _productCode).ConfigureAwait(false))
                {
                    _gameMetadata = await MetadataServiceProvider.GetMetadataAsync(serviceType, _productCode).ConfigureAwait(false);

                    MessageBox.Show("게임 메타데이터를 성공적으로 가져왔습니다.", "Kimera", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("존재하지 않는 상품입니다. 게임 메타데이터를 가져올 수 없습니다.", "Kimera", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("올바른 상품 코드가 아닙니다. 게임 메타데이터를 가져올 수 없습니다.", "Kimera", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public async void EditGameMetadata()
        {
            GameMetadataEditorViewModel viewModel = new GameMetadataEditorViewModel();
            await viewModel.LoadPackageMetadataAsync(_gameMetadata);

            bool? dialogResult = await IoC.Get<IWindowManager>().ShowDialogAsync(viewModel).ConfigureAwait(false);

            if (dialogResult == true)
            {
                _gameMetadata = viewModel.Metadata;
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

        public async void Confirm()
        {
            if (!ValidationHelper.IsValidGameMetadata(_gameMetadata))
            {
                if (UseMetadataServer)
                {
                    Type serviceType;

                    if (MetadataServiceProvider.TryValidProductCode(_productCode, out serviceType))
                    {
                        if (await MetadataServiceProvider.IsAvailableProductAsync(serviceType, _productCode).ConfigureAwait(false))
                        {
                            _gameMetadata = await MetadataServiceProvider.GetMetadataAsync(serviceType, _productCode).ConfigureAwait(false);
                        }
                        else
                        {
                            MessageBox.Show("존재하지 않는 상품입니다. 게임 메타데이터를 가져올 수 없습니다.", "Kimera", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                    }
                    else
                    {
                        MessageBox.Show("올바른 상품 코드가 아닙니다. 게임 메타데이터를 가져올 수 없습니다.", "Kimera", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("게임 메타데이터가 유효하지 않습니다. 각 값들을 형식에 맞게 수정해주세요.", "Kimera", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            // Add data.
            await AddDataAsync().ConfigureAwait(false);

            GoBack();

            MessageBox.Show("게임이 성공적으로 추가되었습니다.", "Kimera", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        #endregion
    }
}
