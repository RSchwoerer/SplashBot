using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace SplashBot_2
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            DataContext = new MainWindowViewModel();

            Icon = new BitmapImage(new Uri("pack://application:,,,/SplashBot_2;component/base-icon.ico"));
            InitializeComponent();
            //   Loaded += (_, __) => Hide();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var b = (Button)sender;
            var p = (Unsplasharp.Models.Photo)b.Tag;
            Process.Start(new ProcessStartInfo(p.Links.Html) { UseShellExecute = true });
        }

        private void TaskbarIcon_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            ((TaskbarIconViewModel)TaskbarIcon.DataContext).ShowWindowCommand.Execute(null);
        }
    }
}