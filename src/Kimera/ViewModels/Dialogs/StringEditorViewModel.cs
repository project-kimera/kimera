using Caliburn.Micro;

namespace Kimera.ViewModels.Dialogs
{
    public class StringEditorViewModel : Screen
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

        private string _text = string.Empty;

        public string Text
        {
            get => _text;
            set => Set(ref _text, value);
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
