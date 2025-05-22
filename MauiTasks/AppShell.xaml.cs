using MauiTasks.Views;

namespace MauiTasks;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(nameof(TaskListPage), typeof(TaskListPage));
        Routing.RegisterRoute(nameof(TaskFormPage), typeof(TaskFormPage));
    }
}
