using MauiTasks.Models;

namespace MauiTasks.ViewModels
{
    // Wrapper ViewModel for a TaskItem to be used in ObservableCollections
    // and to handle UI-specific logic or commands related to a single task.
    public class TaskViewModel : ViewModelBase
    {
        private readonly TaskItem _taskItem;
        private readonly Action _triggerSaveListAction; // Action to notify TaskListViewModel to save

        public TaskItem Model => _taskItem;

        public Guid Id => _taskItem.Id;

        public string Title
        {
            get => _taskItem.Title;
            set
            {
                if (_taskItem.Title != value)
                {
                    _taskItem.Title = value;
                    OnPropertyChanged();
                    _triggerSaveListAction?.Invoke();
                }
            }
        }

        public string Description
        {
            get => _taskItem.Description;
            set
            {
                if (_taskItem.Description != value)
                {
                    _taskItem.Description = value;
                    OnPropertyChanged();
                    _triggerSaveListAction?.Invoke();
                }
            }
        }

        public DateTime DueDate
        {
            get => _taskItem.DueDate;
            set
            {
                if (_taskItem.DueDate != value)
                {
                    _taskItem.DueDate = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(FormattedDueDate)); // Update formatted string too
                    _triggerSaveListAction?.Invoke();
                }
            }
        }

        public string FormattedDueDate => DueDate.ToShortDateString();

        public bool IsCompleted
        {
            get => _taskItem.IsCompleted;
            set
            {
                if (_taskItem.IsCompleted != value)
                {
                    _taskItem.IsCompleted = value;
                    OnPropertyChanged();
                    _triggerSaveListAction?.Invoke();
                }
            }
        }

        public TaskPriority Priority
        {
            get => _taskItem.Priority;
            set
            {
                if (_taskItem.Priority != value)
                {
                    _taskItem.Priority = value;
                    OnPropertyChanged();
                    _triggerSaveListAction?.Invoke();
                }
            }
        }

        public TaskViewModel(TaskItem taskItem, Action triggerSaveListAction)
        {
            _taskItem = taskItem ?? throw new ArgumentNullException(nameof(taskItem));
            _triggerSaveListAction = triggerSaveListAction;
        }

        // Call this if the underlying model is updated externally and the VM needs to refresh all properties
        public void RefreshProperties()
        {
            OnPropertyChanged(nameof(Title));
            OnPropertyChanged(nameof(Description));
            OnPropertyChanged(nameof(DueDate));
            OnPropertyChanged(nameof(FormattedDueDate));
            OnPropertyChanged(nameof(IsCompleted));
            OnPropertyChanged(nameof(Priority));
        }
    }
}
