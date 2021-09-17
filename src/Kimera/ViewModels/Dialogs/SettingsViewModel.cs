using Caliburn.Micro;
using Kimera.Network.Utilities;
using Kimera.ViewModels.Specials;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Kimera.ViewModels.Dialogs
{
    public class SettingsViewModel : Screen
    {
        private Settings _settings = IoC.Get<Settings>();

        public bool UseAutoStarter
        {
            get => _settings.UseAutoStarter;
            set
            {
                _settings.UseAutoStarter = value;
                NotifyOfPropertyChange("UseAutoStarter");
            }
        }

        public string WorkDirectory
        {
            get => _settings.WorkDirectory;
            set
            {
                _settings.WorkDirectory = value;
                NotifyOfPropertyChange("WorkDirectory");
            }
        }

        public string MasterKeys
        {
            get => _settings.MasterKeys;
            set
            {
                _settings.MasterKeys = value;
                NotifyOfPropertyChange("MasterKeys");
            }
        }

        public bool UseAutoRemover
        {
            get => _settings.UseAutoRemover;
            set
            {
                _settings.UseAutoRemover = value;
                NotifyOfPropertyChange("UseAutoRemover");
            }
        }

        public int AutoRemovingInterval
        {
            get => _settings.AutoRemovingInterval;
            set
            {
                _settings.AutoRemovingInterval = value;
                NotifyOfPropertyChange("AutoRemovingInterval");
            }
        }

        public bool UseAntiDPIService
        {
            get => _settings.UseAntiDPIService;
            set
            {
                _settings.UseAntiDPIService = value;
                NotifyOfPropertyChange("UseAntiDPIService");
            }
        }

        private string _versionString = string.Empty;

        public string VersionString
        {
            get => _versionString;
        }

        public SettingsViewModel()
        {
            InitializeSettings();
        }

        private void InitializeSettings()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            _versionString = $"Kimera Version {version.Major}.{version.Minor} (Build {version.Build}, Revision {version.Revision})";
        }

        public async void EncryptDatabase()
        {
            StringEditorViewModel viewModel = new StringEditorViewModel();
            viewModel.Title = (string)App.Current.Resources["VIEW_STRINGEDITOR_ENCRYPT_DATABASE_TITLE"];
            viewModel.Caption = (string)App.Current.Resources["VIEW_STRINGEDITOR_ENCRYPT_DATABASE_CAPTION"];
            viewModel.Text = string.Empty;

            bool? dialogResult = await IoC.Get<IWindowManager>().ShowDialogAsync(viewModel).ConfigureAwait(false);

            if (dialogResult == true)
            {
                string password = viewModel.Text;

                if (GuardViewModel.ValidateSqliteSignature()) // If the database is encrypted...
                {
                    if (!App.DatabaseContext.ChangePassword(ref password))
                    {
                        MessageBox.Show((string)App.Current.Resources["VM_SETTINGS_CRYPTO_TASK_HAS_FAILED_MSG"], "Kimera", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                else
                {
                    if (!App.DatabaseContext.EncryptDatabase(ref password))
                    {
                        MessageBox.Show((string)App.Current.Resources["VM_SETTINGS_CRYPTO_TASK_HAS_FAILED_MSG"], "Kimera", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }

                password = null;
                viewModel.Text = null;

                MessageBox.Show((string)App.Current.Resources["VM_SETTINGS_ENCRYPTED_MSG"], "Kimera", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        public async void DecryptDatabase()
        {
            StringEditorViewModel viewModel = new StringEditorViewModel();
            viewModel.Title = (string)App.Current.Resources["VIEW_STRINGEDITOR_DECRYPT_DATABASE_TITLE"];
            viewModel.Caption = (string)App.Current.Resources["VIEW_STRINGEDITOR_DECRYPT_DATABASE_CAPTION"];
            viewModel.Text = string.Empty;

            bool? dialogResult = await IoC.Get<IWindowManager>().ShowDialogAsync(viewModel).ConfigureAwait(false);

            if (dialogResult == true)
            {
                MessageBoxResult result = MessageBox.Show((string)App.Current.Resources["VM_SETTINGS_DECRYPTION_REMINDER_MSG"], "Kimera", MessageBoxButton.OKCancel, MessageBoxImage.Question);

                if (result == MessageBoxResult.OK)
                {
                    string password = viewModel.Text;

                    if (GuardViewModel.TryPass(password))
                    {
                        if (App.DatabaseContext.DecryptDatabase(ref password))
                        {
                            MessageBox.Show((string)App.Current.Resources["VM_SETTINGS_DECRYPTED_MSG"], "Kimera", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            MessageBox.Show((string)App.Current.Resources["VM_SETTINGS_CRYPTO_TASK_HAS_FAILED_MSG"], "Kimera", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }
                    else
                    {
                        MessageBox.Show((string)App.Current.Resources["VM_SETTINGS_CRYPTO_WRONG_PASSWORD_MSG"], "Kimera", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }

                    password = null;
                    viewModel.Text = null;
                }
            }
        }

        public void ExploreWorkDirectory()
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.Title = "Open";
            dialog.IsFolderPicker = true;

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                if (Directory.Exists(dialog.FileName))
                {
                    WorkDirectory = dialog.FileName;
                }
            }
        }

        public void ShowOpenSources()
        {
            WebHelper.OpenUrl("https://github.com/project-kimera/kimera/blob/main/OPENSOURCES.md");
        }
    }
}
