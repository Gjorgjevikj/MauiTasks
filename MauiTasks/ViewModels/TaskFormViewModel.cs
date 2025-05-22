using System.Collections.ObjectModel;
using System.Windows.Input;
using MauiTasks.Models;
using MauiTasks.Helpers;

namespace MauiTasks.ViewModels
{
    public class TaskFormViewModel : ViewModelBase
    {
        private TaskItem _currentTaskItem;
        private bool _isEditing;
        private List<string> _existingTitlesForValidation;

        private string _title;
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        private string _description;
        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        private DateTime _dueDate;
        public DateTime DueDate
        {
            get => _dueDate;
            set => SetProperty(ref _dueDate, value);
        }

        private bool _isCompleted;
        public bool IsCompleted
        {
            get => _isCompleted;
            set => SetProperty(ref _isCompleted, value);
        }

        private TaskPriority _selectedPriority;
        public TaskPriority SelectedPriority
        {
            get => _selectedPriority;
            set => SetProperty(ref _selectedPriority, value);
        }

        public ObservableCollection<TaskPriority> PriorityOptions { get; }

        public ICommand SaveTaskCommand { get; }
        public ICommand CancelCommand { get; }

        public string PageTitle => _isEditing ? "Edit Task" : "Add New Task";

        public TaskFormViewModel()
        {
            PriorityOptions = new ObservableCollection<TaskPriority>(Enum.GetValues(typeof(TaskPriority)).Cast<TaskPriority>());
            SaveTaskCommand = new RelayCommand(async () => await SaveTask());
            CancelCommand = new RelayCommand(async () => await Shell.Current.GoToAsync("..")); // Navigate back

            // Load task passed via NavigationParameterStore (or initialize for new)
            // This is typically done in OnNavigatedTo or similar lifecycle event in the View's code-behind
            // which then calls an Initialize method on the ViewModel.
        }

        public void Initialize()
        {
            _currentTaskItem = NavigationParameterStore.TaskToEdit;
            _existingTitlesForValidation = NavigationParameterStore.ExistingTaskTitles ?? new List<string>();
            NavigationParameterStore.TaskToEdit = null; // Clear it after use
            NavigationParameterStore.ExistingTaskTitles = null;

            if (_currentTaskItem != null) // Editing existing task
            {
                _isEditing = true;
                Title = _currentTaskItem.Title;
                Description = _currentTaskItem.Description;
                DueDate = _currentTaskItem.DueDate;
                IsCompleted = _currentTaskItem.IsCompleted;
                SelectedPriority = _currentTaskItem.Priority;
            }
            else // Adding new task
            {
                _isEditing = false;
                _currentTaskItem = new TaskItem(); // Create a new model instance
                Title = "New Task";
                Description = string.Empty;
                DueDate = DateTime.Today.AddDays(1);
                IsCompleted = false;
                SelectedPriority = TaskPriority.Medium;
            }
            OnPropertyChanged(nameof(PageTitle));
        }


        private async Task SaveTask()
        {
            // Validation 1: Title not empty
            if (string.IsNullOrWhiteSpace(Title))
            {
                await Application.Current.MainPage.DisplayAlert("Validation Error", "Title cannot be empty.", "OK");
                return;
            }

            // Validation 2: Title uniqueness
            // _existingTitlesForValidation contains titles of *other* tasks.
            if (_existingTitlesForValidation.Any(t => t.Equals(Title, StringComparison.OrdinalIgnoreCase)))
            {
                await Application.Current.MainPage.DisplayAlert("Validation Error", "A task with this title already exists.", "OK");
                return;
            }


            // Populate the model
            _currentTaskItem.Title = Title;
            _currentTaskItem.Description = Description;
            _currentTaskItem.DueDate = DueDate;
            _currentTaskItem.IsCompleted = IsCompleted;
            _currentTaskItem.Priority = SelectedPriority;

            // Send message to TaskListViewModel to handle the actual save/add and persistence
            MessagingCenter.Send(this, "TaskSaved", _currentTaskItem);

            await Shell.Current.GoToAsync(".."); // Navigate back
        }
    }
}
