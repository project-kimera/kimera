using Caliburn.Micro;
using Kimera.Data.Entities;
using Kimera.IO;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Kimera.ViewModels.Dialogs
{
    public class ArchiveFileExplorerViewModel : Screen
    {
        private string _title = string.Empty;

        public string Title
        {
            get => _title;
            set => Set(ref _title, value);
        }

        private string _caption = string.Empty;

        public string Caption
        {
            get => _caption;
            set => Set(ref _caption, value);
        }

        private List<string> _extensions = new List<string>();

        public List<string> Extensions
        {
            get => _extensions;
            set => Set(ref _extensions, value);
        }

        private BindableCollection<Component> _components = new BindableCollection<Component>();

        public BindableCollection<Component> Components
        {
            get => _components;
            set => Set(ref _components, value);
        }

        private Component _selectedComponent = new Component();

        public Component SelectedComponent
        {
            get => _selectedComponent;
            set
            {
                Set(ref _selectedComponent, value);
                SetComponent(value.FilePath);
            }
        }

        private BindableCollection<string> _files = new BindableCollection<string>();

        public BindableCollection<string> Files
        {
            get => _files;
            set => Set(ref _files, value);
        }

        private string _selectedFile = string.Empty;

        public string SelectedFile
        {
            get => _selectedFile;
            set => Set(ref _selectedFile, value);
        }

        private async void SetComponent(string filePath)
        {
            try
            {
                bool errorFlag = false;

                List<string> files = new List<string>();

                ArchiveFileManager manager = new ArchiveFileManager(filePath);

                if (await manager.IsEncryptedAsync())
                {
                    string password = string.Empty;

                    while (true)
                    {
                        PasswordEditorViewModel viewModel = new PasswordEditorViewModel();
                        viewModel.Title = (string)App.Current.Resources["VIEW_PASSWORDEDITOR_DECRYPT_ARCHIVE_TITLE"];
                        viewModel.Caption = (string)App.Current.Resources["VIEW_PASSWORDEDITOR_DECRYPT_ARCHIVE_CAPTION"];
                        viewModel.Password = string.Empty;

                        bool? dialogResult = await IoC.Get<IWindowManager>().ShowDialogAsync(viewModel).ConfigureAwait(false);

                        if (dialogResult == true)
                        {
                            if (await manager.IsValidPasswordAsync(viewModel.Password))
                            {
                                password = viewModel.Password;
                                viewModel.Password = null;
                                break;
                            }
                            else
                            {
                                viewModel.Password = null;
                                MessageBox.Show((string)App.Current.Resources["VM_ARCHIVEFILEEXPLORER_INVALID_PW_MSG"], "Kimera", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                                continue;
                            }
                        }
                        else
                        {
                            errorFlag = true;
                            break;
                        }
                    }

                    if (!errorFlag)
                    {
                        files = await manager.GetFilesAsync(_extensions, password);
                        password = null;
                    }
                }
                else
                {
                    files = await manager.GetFilesAsync(_extensions);
                }

                Files = new BindableCollection<string>(files);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An exception occurred while processing a archive.");
                MessageBox.Show(string.Format((string)App.Current.Resources["VM_ARCHIVEFILEEXPLORER_EXCEPTION_MSG"], ex.Message), "Kimera", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        public async void Cancel()
        {
            await TryCloseAsync(false);
        }

        public async void Confirm()
        {
            await TryCloseAsync(true);
        }
    }
}
