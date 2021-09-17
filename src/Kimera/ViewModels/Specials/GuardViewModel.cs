using Caliburn.Micro;
using Kimera.Data.Contexts;
using Kimera.ViewModels.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Kimera.ViewModels.Specials
{
    public class GuardViewModel : Screen
    {
        private bool _isEncrypted = false;

        public bool IsEncrypted
        {
            get => _isEncrypted;
        }

        private string _message = (string)App.Current.Resources["VIEW_GUARD_NEEDS_PASSWORD_CAPTION"];

        public string Message
        {
            get => _message;
            set => Set(ref _message, value);
        }

        private string _password = string.Empty;

        public string Password
        {
            get => _password;
            set => Set(ref _password, value);
        }

        public GuardViewModel()
        {
            Initialize();
        }

        private async void Initialize()
        {
            var task = Task.Factory.StartNew(() =>
            {
                if (!File.Exists(Settings.DatabaseFilePath))
                {
                    _isEncrypted = false;
                    CallScouter(null);
                }
                else
                {
                    _isEncrypted = ValidateSqliteSignature();

                    if (!_isEncrypted)
                    {
                        CallScouter(null);
                    }
                }
            });

            await task;
        }

        private bool ValidateSqliteSignature()
        {
            byte[] sqliteDatabaseSignature = new byte[16] { 0x53, 0x51, 0x4c, 0x69, 0x74, 0x65, 0x20, 0x66, 0x6f, 0x72, 0x6d, 0x61, 0x74, 0x20, 0x33, 0x00 };
            using (FileStream stream = new FileStream(Settings.DatabaseFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    byte[] readBytes = reader.ReadBytes(16);

                    return !readBytes.SequenceEqual(sqliteDatabaseSignature);
                }
            }
        }

        private bool TryPass(string password)
        {
            try
            {
                using (KimeraContext context = new KimeraContext(Settings.DatabaseFilePath, password))
                {
                    context.Database.EnsureCreated();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        private void CallScouter(string password)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                INavigationService navigationService = IoC.Get<INavigationService>();
                navigationService.For<ScouterViewModel>()
                    .WithParam(s => s.Password, password)
                    .Navigate();
            });
        }

        public void Submit()
        {
            if (TryPass(_password))
            {
                CallScouter(_password);
            }
            else
            {
                Message = (string)App.Current.Resources["VIEW_GUARD_WRONG_PASSWORD_CAPTION"];
            }

            Password = string.Empty;
        }

        public async void Reset()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            string secretKey = new string(Enumerable.Repeat(chars, 10).Select(s => s[new Random().Next(s.Length)]).ToArray());

            StringEditorViewModel viewModel = new StringEditorViewModel();
            viewModel.Title = (string)App.Current.Resources["VIEW_STRINGEDITOR_RESET_APP_TITLE"];
            viewModel.Caption = string.Format((string)App.Current.Resources["VIEW_STRINGEDITOR_RESET_APP_CAPTION"], secretKey);
            viewModel.Text = string.Empty;

            bool? dialogResult = await IoC.Get<IWindowManager>().ShowDialogAsync(viewModel).ConfigureAwait(false);

            if (dialogResult == true)
            {
                if (viewModel.Text == secretKey)
                {
                    try
                    {
                        File.Delete(Settings.DatabaseFilePath);
                        Initialize();
                    }
                    catch
                    {
                        MessageBox.Show((string)App.Current.Resources["VM_GUARD_RESET_APP_EXCEPTION_MSG"], "Kimera", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                else
                {
                    MessageBox.Show((string)App.Current.Resources["VM_GUARD_RESET_APP_UNMATCHED_MSG"], "Kimera", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            }
        }
    }
}
