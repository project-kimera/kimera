using Caliburn.Micro;

namespace Kimera.ViewModels.Dialogs
{
    public class PasswordEditorViewModel : Screen
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

        private string _password = string.Empty;

        public string Password
        {
            get => _password;
            set => Set(ref _password, value);
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
