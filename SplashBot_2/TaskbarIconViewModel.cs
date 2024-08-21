using System.Windows;
using System.Windows.Input;
using Unsplasharp;

namespace SplashBot_2
{
    internal class TaskbarIconViewModel
    {
        public ICommand TestCommand
        {
            get
            {
                return new DelegateCommand { CommandAction = () => Test() };
            }
        }

        private async void Test()
        {
            var client = new UnsplasharpClient("asdfasdf");
            var photosFound = Task.Run(async () => await client.SearchPhotos("mountains")).GetAwaiter().GetResult();
            // var photosFound = Task.Run(async () => await client.GetRandomPhoto()).GetAwaiter().GetResult();
        }


        /// <summary>
        /// Shuts down the application.
        /// </summary>
        public ICommand ExitApplicationCommand
        {
            get
            {
                return new DelegateCommand { CommandAction = () => Application.Current.Shutdown() };
            }
        }

        /// <summary>
        /// Hides the main window. This command is only enabled if a window is open.
        /// </summary>
        public ICommand HideWindowCommand
        {
            get
            {
                return new DelegateCommand
                {
                    CanExecuteFunc = () => Application.Current.MainWindow != null,
                    CommandAction = () => Application.Current.MainWindow.Close()
                };
            }
        }

        /// <summary>
        /// Shows a window, if none is already open.
        /// </summary>
        public ICommand ShowWindowCommand
        {
            get
            {
                return new DelegateCommand
                {
                    CanExecuteFunc = () => Application.Current.MainWindow == null,
                    CommandAction = () =>
                    {
                        Application.Current.MainWindow = new MainWindow();
                        Application.Current.MainWindow.Show();
                    }
                };
            }
        }
    }
}