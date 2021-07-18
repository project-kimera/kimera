using Kimera.Commands;
using Kimera.Data.Entities;
using Kimera.Dialogs;
using Kimera.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using MessageBox = System.Windows.MessageBox;

namespace Kimera.ViewModels
{
    public class AddExecutableFileViewModel : ViewModelBase
    {
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

        public PackageMetadata PackageMetadata { get; set; }

        public GameMetadata GameMetadata { get; set; }

        public DelegateCommand ExploreFileCommand { get; }

        public DelegateCommand GetGameMetadataCommand { get; }

        public DelegateCommand EditGameMetadataCommand { get; }

        public RelayCommand<Window> CancelCommand { get; }

        public RelayCommand<Window> ConfirmCommand { get; }

        public AddExecutableFileViewModel()
        {
            PackageMetadata = new PackageMetadata();
            GameMetadata = new GameMetadata();

            ExploreFileCommand = new DelegateCommand(ExploreFile);
            GetGameMetadataCommand = new DelegateCommand(GetGameMetadata);
            EditGameMetadataCommand = new DelegateCommand(EditGameMetadata);
            CancelCommand = new RelayCommand<Window>(Cancel);
            ConfirmCommand = new RelayCommand<Window>(Confirm);
        }

        private void ExploreFile()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "Open";
            dialog.Filter = "Executable file (*.exe)|*.exe";
            dialog.Multiselect = false;
            
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                FilePath = dialog.FileName;
                ProductCode = DLSiteHelper.GetProductCodeFromPath(FilePath);
            }
        }

        private async void GetGameMetadata()
        {
            bool isValid = await DLSiteHelper.IsValidProductAsync(_productCode);
            
            if (isValid)
            {
                GameMetadata = await DLSiteHelper.GetGameMetadataAsync(_productCode);
                MessageBox.Show("메타데이터를 성공적으로 가져왔습니다.", "Kimera", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("상품을 찾을 수 없습니다.", "Kimera", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EditGameMetadata()
        {
            EditGameMetadataDialog dialog = new EditGameMetadataDialog(GameMetadata);
            dialog.ShowDialog();
        }

        private void Cancel(Window window)
        {
            if (window != null)
            {
                window.Close();
            }
        }

        private void Confirm(Window window)
        {
            if (!ValidationHelper.IsValid(window))
            {
                MessageBox.Show("게임을 등록할 수 없습니다. 필수 정보들을 작성해주세요.", "Kimera", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!ValidationHelper.IsValidGameMetadata(GameMetadata))
            {
                if (UseMetadataServer)
                {
                    GetGameMetadata();
                }
                else
                {
                    MessageBox.Show("메타데이터가 유효하지 않습니다. 각 값들을 형식에 맞게 수정해주세요.", "Kimera", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }



            if (window != null)
            {
                window.Close();
            }
        }
    }
}
