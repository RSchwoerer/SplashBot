using Microsoft.Extensions.DependencyInjection;
using SplashBot_2.Service;
using System.Windows;

namespace SplashBot_2
{
    public partial class App : Application
    {
        public App()
        {
            Services = ConfigureServices();
            InitializeComponent();
        }

        public new static App Current => (App)Application.Current;

        public IServiceProvider Services { get; }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
        }

        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection()
                .AddSingleton<AppSettings>()
                .AddSingleton<DataService>()
                .AddSingleton<ScheduleService>()
                .AddSingleton<UnsplashService>()
                .AddTransient<MainWindowViewModel>();

            return services.BuildServiceProvider();
        }
    }
}