using CommunityToolkit.Mvvm.Input;
using System.Windows;
using System.Windows.Input;

namespace SplashBot_2
{
    internal class TaskbarIconViewModel
    {
        private readonly MainWindowViewModel mainWindowViewModel;

        public TaskbarIconViewModel(MainWindowViewModel mainWindowViewModel)
        {
            TestCommand = new AsyncRelayCommand(Test);
            this.mainWindowViewModel = mainWindowViewModel;
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
                    //CanExecuteFunc = () => Application.Current.MainWindow != null,
                    CommandAction = () => Application.Current.MainWindow.Hide()
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
                    //CanExecuteFunc = () => Application.Current.MainWindow == null,
                    CommandAction = () =>
                    {
                        //Application.Current.MainWindow = new MainWindow();
                        Application.Current.MainWindow.Show();
                    }
                };
            }
        }

        public IAsyncRelayCommand TestCommand { get; }

        private async Task Test()
        {
            await mainWindowViewModel.Test();
        }
    }
}