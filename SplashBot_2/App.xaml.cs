using System.Windows;

namespace SplashBot_2
{
    public partial class App : Application
    {
        //private TaskbarIcon taskbarIcon;
        // private HandyControl.Controls.NotifyIcon taskbarIcon;

        protected override void OnExit(ExitEventArgs e)
        {
            //   taskbarIcon.Dispose(); //the icon would clean up automatically, but this is cleaner
            base.OnExit(e);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            //create the notifyicon (it's a resource declared in NotifyIconResources.xaml
            //taskbarIcon = (TaskbarIcon)FindResource("TaskbarIcon");
            //    taskbarIcon = (HandyControl.Controls.NotifyIcon)FindResource("TaskbarIcon");
        }
    }
}