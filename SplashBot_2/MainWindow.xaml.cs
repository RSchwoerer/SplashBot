using System.ComponentModel;
using System.Windows;
using System.Windows.Media.Imaging;

namespace SplashBot_2
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            DataContext = new MainWindowViewModel();

            Title = "SplashBot";
            Icon = new BitmapImage(new Uri("pack://application:,,,/SplashBot_2;component/base-icon.ico"));
            InitializeComponent();
            Loaded += (_, __) => Hide();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        private void TaskbarIcon_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            ((TaskbarIconViewModel)TaskbarIcon.DataContext).ShowWindowCommand.Execute(null);
        }
    }
}