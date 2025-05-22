using MauiTasks.Models;

namespace MauiTasks.Helpers
{
    // A simple static class to pass complex objects during navigation
    // if query parameters are not sufficient or convenient.
    public static class NavigationParameterStore
    {
        public static TaskItem TaskToEdit { get; set; }
        public static List<string> ExistingTaskTitles { get; set; } // For validation
    }
}
