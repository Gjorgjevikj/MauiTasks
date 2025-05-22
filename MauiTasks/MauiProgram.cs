using Microsoft.Extensions.Logging;
using MauiTasks.ViewModels; // If registering ViewModels for DI
using MauiTasks.Views;     // If registering Views for DI

namespace MauiTasks;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif
        // Optional: Register ViewModels and Pages for Dependency Injection
        // builder.Services.AddSingleton<TaskListViewModel>(); // Or Transient
        // builder.Services.AddTransient<TaskFormViewModel>();

        // builder.Services.AddSingleton<TaskListPage>(); // Or Transient
        // builder.Services.AddTransient<TaskFormPage>();


        return builder.Build();
    }
}
