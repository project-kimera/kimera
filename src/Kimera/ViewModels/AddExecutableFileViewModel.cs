using Kimera.Common.Commands;
using Kimera.Data.Entities;
using Kimera.Data.Enums;
using Kimera.Data.Extensions;
using Kimera.Dialogs;
using Kimera.Network;
using Kimera.Services;
using Kimera.Utilities;
using Serilog;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using MessageBox = System.Windows.MessageBox;

namespace Kimera.ViewModels
{
    public class AddExecutableFileViewModel : ViewModelBase
    {
        #region ::Variables & Properties::

        private GameMetadata _gameMetadata = new GameMetadata();

        private string _filePath = string.Empty;

        public string FilePath
        {
            get
            {
                return _filePath;
            }
            set
            {
                _filePath = value;
                RaisePropertyChanged();
            }
        }

        private string _entryPointFilePath = string.Empty;

        public string EntryPointFilePath
        {
            get
            {
                return _entryPointFilePath;
            }
            set
            {
                _entryPointFilePath = value;
                RaisePropertyChanged();
            }
        }

        private string _commandlineArguments = string.Empty;

        public string CommandlineArguments
        {
            get
            {
                return _commandlineArguments;
            }
            set
            {
                _commandlineArguments = value;
                RaisePropertyChanged();
            }
        }

        private bool _useCategory = false;

        public bool UseCategory
        {
            get
            {
                return _useCategory;
            }
            set
            {
                _useCategory = value;
                RaisePropertyChanged();
            }
        }

        private ObservableCollection<Category> _categories = LibraryService.Instance.Categories;

        public ObservableCollection<Category> Categories
        {
            get
            {
                return _categories;
            }
            set
            {
                _categories = value;
                RaisePropertyChanged();
            }
        }

        private Category _currentCategory = App.DatabaseContext.Categories.Where(c => c.SystemId == LibraryService.Instance.SelectedCategory).FirstOrDefault();

        public Category CurrentCategory
        {
            get
            {
                return _currentCategory;
            }
            set
            {
                _currentCategory = value;
                RaisePropertyChanged();
            }
        }

        private bool _useMetadataServer = true;

        public bool UseMetadataServer
        {
            get
            {
                return _useMetadataServer;
            }
            set
            {
                _useMetadataServer = value;

                if (value == false)
                {
                    ProductCode = "RJ000000";
                }

                RaisePropertyChanged();
            }
        }

        private string _productCode = string.Empty;

        public string ProductCode
        {
            get
            {
                return _productCode;
            }
            set
            {
                _productCode = value;
                RaisePropertyChanged();
            }
        }

        public DelegateCommand ExploreFileCommand { get; }

        public DelegateCommand GetGameMetadataCommand { get; }

        public DelegateCommand EditGameMetadataCommand { get; }

        public DelegateCommand GoBackCommand { get; }

        public RelayCommand<Page> ConfirmCommand { get; }

        #endregion

        #region ::Constructors::

        public AddExecutableFileViewModel()
        {
            ExploreFileCommand = new DelegateCommand(ExploreFile);
            GetGameMetadataCommand = new DelegateCommand(GetGameMetadata);
            EditGameMetadataCommand = new DelegateCommand(EditGameMetadata);
            GoBackCommand = new DelegateCommand(GoBack);
            ConfirmCommand = new RelayCommand<Page>(Confirm);
        }

        #endregion

        #region ::Methods::

        private async Task AddDataAsync()
        {
            try
            {
                using (var transaction = await App.DatabaseContext.Database.BeginTransactionAsync())
                {

                    Game game = new Game();

                    // Add the game information.
                    GameMetadata gameMetadata = _gameMetadata.Copy();
                    gameMetadata.FirstTime = DateTime.Now;
                    gameMetadata.LastTime = DateTime.Now;

                    PackageMetadata packageMetadata = new PackageMetadata(PackageType.Executable, EntryPointFilePath, CommandlineArguments);
                    packageMetadata.AddComponent(new Component(ComponentType.Executable, 0, FilePath));

                    // Set game metadatas and package status.
                    game.SetGameMetadata(gameMetadata);
                    game.SetPackageMetadata(packageMetadata);
                    game.PackageStatus = PackageStatus.NeedProcessing;

                    // Subscribes categories.
                    CategorySubscription subscription = new CategorySubscription(Settings.GUID_ALL_CATEGORY, game.SystemId);

                    await App.DatabaseContext.Games.AddAsync(game).ConfigureAwait(false);
                    await App.DatabaseContext.CategorySubscriptions.AddAsync(subscription).ConfigureAwait(false);

                    await App.DatabaseContext.SaveChangesAsync().ConfigureAwait(false);

                    Log.Information("A game data has been registerd.");

                    await transaction.CommitAsync();
                }
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "An exception occurred while adding a game data.");
            }
        }

        #endregion

        #region ::Command Actions::

        private void ExploreFile()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "Open";
            dialog.Filter = "Executable file (*.exe)|*.exe";
            dialog.Multiselect = false;
            
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                FilePath = dialog.FileName;

                string productCode = MetadataServiceProvider.GetProductCodeFromPath(FilePath);

                if (string.IsNullOrEmpty(productCode))
                {
                    MessageBox.Show("상품 코드를 가져올 수 없습니다. 수동으로 메타데이터를 입력해주세요.", "Kimera", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    ProductCode = productCode;
                }
            }
        }

        private async void GetGameMetadata()
        {
            string typeName;

            if (MetadataServiceProvider.TryValidProductCode(_productCode, out typeName))
            {
                if (await MetadataServiceProvider.IsAvailableProductAsync(typeName, _productCode))
                {
                    _gameMetadata = await MetadataServiceProvider.GetMetadataAsync(typeName, _productCode).ConfigureAwait(false);

                    MessageBox.Show("메타데이터를 성공적으로 가져왔습니다.", "Kimera", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("존재하지 않는 상품입니다. 메타데이터를 가져올 수 없습니다.", "Kimera", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("올바른 상품 코드가 아닙니다. 메타데이터를 가져올 수 없습니다.", "Kimera", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EditGameMetadata()
        {
            EditGameMetadataDialog dialog = new EditGameMetadataDialog(_gameMetadata);
            dialog.ShowDialog();
        }

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

        private async void Confirm(Page page)
        {
            if (!ValidationHelper.IsValid(page))
            {
                MessageBox.Show("게임을 등록할 수 없습니다. 필수 정보들을 작성해주세요.", "Kimera", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!ValidationHelper.IsValidGameMetadata(_gameMetadata))
            {
                if (UseMetadataServer)
                {
                    string typeName;

                    if (MetadataServiceProvider.TryValidProductCode(_productCode, out typeName))
                    {
                        if (await MetadataServiceProvider.IsAvailableProductAsync(typeName, _productCode))
                        {
                            _gameMetadata = await MetadataServiceProvider.GetMetadataAsync(typeName, _productCode).ConfigureAwait(false);
                        }
                        else
                        {
                            MessageBox.Show("존재하지 않는 상품입니다. 메타데이터를 가져올 수 없습니다.", "Kimera", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                    }
                    else
                    {
                        MessageBox.Show("올바른 상품 코드가 아닙니다. 메타데이터를 가져올 수 없습니다.", "Kimera", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("메타데이터가 유효하지 않습니다. 각 값들을 형식에 맞게 수정해주세요.", "Kimera", MessageBoxButton.OK, MessageBoxImage.Error);
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
