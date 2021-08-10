using Caliburn.Micro;
using Kimera.Entities;

namespace Kimera.Services
{
    public class TaskService : PropertyChangedBase
    {
        private BindableCollection<TaskStatusTracker> _tasks = new BindableCollection<TaskStatusTracker>();

        public BindableCollection<TaskStatusTracker> Tasks
        {
            get => _tasks;
            set => Set(ref _tasks, value);
        }

        public TaskService()
        {
            TaskStatusTracker taskStatus = new TaskStatusTracker();
            taskStatus.IsIndeterminate = true;
            taskStatus.IsWorking = true;
            taskStatus.Result = TaskRecordType.Working;
            taskStatus.Caption = "TaskService 테스트 작업중";

            Add(taskStatus);
        }

        public void Add(TaskStatusTracker task)
        {
            if (!_tasks.Contains(task))
            {
                Tasks.Insert(0, task);
            }
        }

        public void Reset()
        {
            Tasks.Clear();
        }
    }
}
