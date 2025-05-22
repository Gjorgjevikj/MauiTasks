namespace MauiTasks;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        MainPage = new AppShell();
    }
}



//namespace MauiTasks
//{
//    public partial class App : Application
//    {
//        public App()
//        {
//            InitializeComponent();
//        }

//        protected override Window CreateWindow(IActivationState? activationState)
//        {
//            return new Window(new AppShell());
//        }
//    }
//}