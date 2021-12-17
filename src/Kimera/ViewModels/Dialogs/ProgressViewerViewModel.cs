using Caliburn.Micro;

namespace Kimera.ViewModels.Dialogs
{
    public class ProgressViewerViewModel : Screen
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

        private double _progress = 0.0;

        public double Progress
        {
            get => _progress;
            set => Set(ref _progress, value);
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
