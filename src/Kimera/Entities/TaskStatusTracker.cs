using Caliburn.Micro;

namespace Kimera.Entities
{
    public class TaskStatusTracker : PropertyChangedBase
    {
        private TaskRecordType _result = TaskRecordType.Working;

        public TaskRecordType Result
        {
            get => _result;
            set => Set(ref _result, value);
        }

        private string _caption = string.Empty;

        public string Caption
        {
            get => _caption;
            set => Set(ref _caption, value);
        }

        private bool _isWorking = false;

        public bool IsWorking
        {
            get => _isWorking;
            set => Set(ref _isWorking, value);
        }

        private bool _isIndeterminate = false;

        public bool IsIndeterminate
        {
            get => _isIndeterminate;
            set => Set(ref _isIndeterminate, value);
        }

        private double _progress = 0.0;

        public double Progress
        {
            get => _progress;
            set => Set(ref _progress, value);
        }
    }
}
