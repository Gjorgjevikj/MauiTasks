using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text.Json;
using System.Windows.Input;
using MauiTasks.Models;
using MauiTasks.Helpers;
using MauiTasks.Views; // For nameof(TaskFormPage)

namespace MauiTasks.ViewModels
{
    public class TaskListViewModel : ViewModelBase
    {
        private readonly string _tasksFilePath = Path.Combine(FileSystem.AppDataDirectory, "tasks.json");

        public ObservableCollection<TaskViewModel> Tasks { get; } = new ObservableCollection<TaskViewModel>();

        private TaskViewModel _selectedTask;
        public TaskViewModel SelectedTask
        {
            get => _selectedTask;
            set => SetProperty(ref _selectedTask, value);
        }

        public ICommand AddTaskCommand { get; }
        public ICommand EditTaskCommand { get; }
        public ICommand DeleteTaskCommand { get; }
        public ICommand LoadTasksCommand { get; }
        public ICommand ItemTappedCommand { get; } // For double-tap edit on desktop

        public ICommand DeleteSelectedTaskCommand { get; }
        public TaskListViewModel()
        {
            DeleteSelectedTaskCommand = new RelayCommand(async () =>
            {
                if (SelectedTask != null)
                {
                    await DeleteTask(SelectedTask);
                }
            });

            AddTaskCommand = new RelayCommand(async () => await GoToTaskForm(null));
            EditTaskCommand = new RelayCommand(async (taskVM) =>
            {
                if (taskVM is TaskViewModel tvm)
                {
                    await GoToTaskForm(tvm.Model);
                }
            });
            DeleteTaskCommand = new RelayCommand(async (taskVM) =>
            {
                if (taskVM is TaskViewModel tvm)
                {
                    await DeleteTask(tvm);
                }
            });
            ItemTappedCommand = new RelayCommand(async (taskVM) => // For double-tap
            {
                if (taskVM is TaskViewModel tvm)
                {
                    await GoToTaskForm(tvm.Model);
                }
            });

            LoadTasksCommand = new RelayCommand(async () => await LoadTasksAsync());

            // Subscribe to messages from TaskFormViewModel
            MessagingCenter.Subscribe<TaskFormViewModel, TaskItem>(this, "TaskSaved", async (sender, taskItem) =>
            {
                await HandleTaskSaved(taskItem);
            });

            // Initial load
            _ = LoadTasksAsync(); // Fire and forget for constructor, or use a specific load command
        }

        private async Task GoToTaskForm(TaskItem taskToEdit)
        {
            NavigationParameterStore.TaskToEdit = taskToEdit; // Null if new task
            // Pass existing titles for validation, excluding the current task's title if editing
            NavigationParameterStore.ExistingTaskTitles = Tasks
                .Where(tvm => taskToEdit == null || tvm.Id != taskToEdit.Id)
                .Select(tvm => tvm.Title)
                .ToList();

            await Shell.Current.GoToAsync(nameof(TaskFormPage));
        }

        private async Task DeleteTask(TaskViewModel taskVMToDelete)
        {
            if (taskVMToDelete == null) return;

            bool confirmed = await Application.Current.MainPage.DisplayAlert(
                "Confirm Delete",
                $"Are you sure you want to delete '{taskVMToDelete.Title}'?",
                "Yes", "No");

            if (confirmed)
            {
                Tasks.Remove(taskVMToDelete);
                await SaveTasksAsync();
            }
        }

        public async Task HandleTaskSaved(TaskItem savedTaskItem)
        {
            if (savedTaskItem == null) return;

            var existingVm = Tasks.FirstOrDefault(tvm => tvm.Id == savedTaskItem.Id);
            if (existingVm != null) // It's an update
            {
                // Update the model within the existing TaskViewModel
                // This assumes TaskViewModel's properties are directly bound or will be refreshed.
                // For simplicity, we replace the properties of the model object that TaskViewModel holds.
                // If TaskViewModel was created with a copy, we'd need to update that copy.
                // Since TaskViewModel holds a reference to the TaskItem model passed via NavigationParameterStore,
                // the changes made in TaskFormViewModel should already be on that model instance.
                // We just need to ensure the TaskViewModel reflects these if it doesn't auto-update.
                existingVm.Title = savedTaskItem.Title; // This will call OnPropertyChanged in TaskViewModel
                existingVm.Description = savedTaskItem.Description;
                existingVm.DueDate = savedTaskItem.DueDate;
                existingVm.IsCompleted = savedTaskItem.IsCompleted;
                existingVm.Priority = savedTaskItem.Priority;
                existingVm.RefreshProperties(); // Ensure all bindings update
            }
            else // It's a new task
            {
                Tasks.Add(new TaskViewModel(savedTaskItem, async () => await SaveTasksAsync()));
            }
            await SaveTasksAsync();
        }


        public async Task LoadTasksAsync()
        {
            Tasks.Clear();
            List<TaskItem> loadedItems = new List<TaskItem>();

            if (File.Exists(_tasksFilePath))
            {
                try
                {
                    string json = await File.ReadAllTextAsync(_tasksFilePath);
                    loadedItems = JsonSerializer.Deserialize<List<TaskItem>>(json) ?? new List<TaskItem>();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error loading tasks: {ex.Message}");
                    // Optionally, display an error to the user
                }
            }

            if (!loadedItems.Any())
            {
                // Add sample data if no tasks are loaded
                loadedItems.Add(new TaskItem { Title = "Sample Task 1", Description = "Description for sample 1", DueDate = DateTime.Today.AddDays(1), Priority = TaskPriority.Medium });
                loadedItems.Add(new TaskItem { Title = "Sample Task 2", Description = "Description for sample 2", DueDate = DateTime.Today.AddDays(3), Priority = TaskPriority.High, IsCompleted = true });
            }

            foreach (var item in loadedItems)
            {
                Tasks.Add(new TaskViewModel(item, async () => await SaveTasksAsync()));
            }
        }

        public async Task SaveTasksAsync()
        {
            try
            {
                var itemsToSave = Tasks.Select(vm => vm.Model).ToList();
                string json = JsonSerializer.Serialize(itemsToSave, new JsonSerializerOptions { WriteIndented = true });
                await File.WriteAllTextAsync(_tasksFilePath, json);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error saving tasks: {ex.Message}");
                // Optionally, display an error to the user
            }
        }

        // Call this when the page appears to ensure data is fresh, especially after returning from TaskFormPage
        public async Task OnAppearing()
        {
            // If not relying on MessagingCenter for updates, you might reload or refresh here.
            // For now, MessagingCenter handles adds/updates, and IsCompleted toggle saves directly.
            // A general save might be good if other properties could change without explicit save triggers.
            // await SaveTasksAsync(); // Example: save on appearing if needed
        }
    }
}
