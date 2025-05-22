using MauiTasks.ViewModels;

namespace MauiTasks.Views;

public partial class TaskFormPage : ContentPage
{
    public TaskFormPage()
    {
        InitializeComponent();
        // BindingContext is set in XAML.
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        // Initialize ViewModel when the page appears,
        // as it might be receiving parameters or need to load data.
        if (BindingContext is TaskFormViewModel vm)
        {
            vm.Initialize();
        }
    }
}