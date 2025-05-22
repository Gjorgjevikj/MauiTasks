using MauiTasks.ViewModels;

namespace MauiTasks.Views;

public partial class TaskListPage : ContentPage
{
    public TaskListPage()
    {
        InitializeComponent();
        // BindingContext is set in XAML for this simple case.
        // If more complex initialization is needed for the ViewModel,
        // you might do it here:
        // var viewModel = new TaskListViewModel();
        // BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is TaskListViewModel vm)
        {
            // ViewModel can handle its own appearing logic if needed
            await vm.OnAppearing();
        }
    }

    // Note on Delete Key:
    // Implementing the "Delete" key press for a CollectionView item on Windows/Catalyst
    // purely from XAML/ViewModel is non-trivial in .NET MAUI without custom renderers or
    // more complex event aggregation.
    // A common approach for desktop would be:
    // 1. Ensure CollectionView has SelectionMode="Single" and SelectedItem bound to ViewModel.
    // 2. Provide a "Delete Selected" button or MenuBarItem command.
    // 3. For direct key press: Handle KeyDown event at the Page level (or a focused element).
    //    This might require platform-specific considerations or a behavior.
    //    The provided XAML uses SwipeView for delete, which is cross-platform.
    //    Double-tap is added as an alternative edit gesture.
}

