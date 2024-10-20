using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using SplashBot_2.Service;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using Unsplasharp.Models;

namespace SplashBot_2
{
    internal class MainWindowViewModel : ObservableObject
    {
        private readonly DataService dataService;
        private readonly AppSettings settings;
        private readonly UnsplashService unsplashService;
        private ICommand _ExitApplicationCommand;
        private AsyncRelayCommand _GetHistoryCommand;
        private bool _IsRunAtStartupSelected;
        private AsyncRelayCommand _NextImageCommand;
        private AsyncRelayCommand _ToggleRunAtStartupCommand;
        private string apiCallsRemaining;
        private Photo currentPhoto;
        private string photoSearchQuery = "bears,earth,landscape,northern lights";
        private ScheduleService scheduleService;

        public MainWindowViewModel(AppSettings settings, DataService ds, ScheduleService ss, UnsplashService us)
        {
            this.settings = settings;
            unsplashService = us /*new UnsplashService()*/;
            unsplashService.ApiLimitUpdated +=
                (sender, args) => { OnPropertyChanged(nameof(ApiLimitDetails)); };
            dataService = ds /*new DataService()*/;
            scheduleService = ss /*new ScheduleService(() => NextImage())*/;

            Initialize();
        }

        public string ApiLimitDetails =>
            $"{unsplashService.ApiCallsRemaining}/{unsplashService.ApiCallsLimit} requests remaining this hour";

        public string ApplicationTitle =>
            "SplashBot";

        public Photo CurrentPhoto { get => currentPhoto; set => SetProperty(ref currentPhoto, value); }

        public ICommand ExitApplicationCommand => _ExitApplicationCommand ??=
            new DelegateCommand { CommandAction = Application.Current.Shutdown };

        public IAsyncRelayCommand GetHistoryCommand => _GetHistoryCommand ??=
            new AsyncRelayCommand(GetHistory);

        public bool IsRunAtStartupSelected { get => _IsRunAtStartupSelected; set => SetProperty(ref _IsRunAtStartupSelected, value); }

        public IAsyncRelayCommand NextImageCommand => _NextImageCommand ??=
                            new AsyncRelayCommand(NextImage);

        public string PhotoSearchQuery { get => photoSearchQuery; set => SetProperty(ref photoSearchQuery, value); }

        public IAsyncRelayCommand ToggleRunAtStartupCommand => _ToggleRunAtStartupCommand ??=
                        new AsyncRelayCommand(ToggleRunAtStartup);

        internal async Task NextImage()
        {
            settings.SearchText = PhotoSearchQuery;
            CurrentPhoto = await unsplashService.SetTestBackground(PhotoSearchQuery);
            await dataService.AddPhotoToPhotoHistory(CurrentPhoto);
        }

        private async Task GetHistory()
        {
            var h = await dataService.GetPhotoHistory(0, 10);
        }

        private async Task Initialize()
        {
            try
            {
                PhotoSearchQuery = settings.SearchText;
                _IsRunAtStartupSelected = settings.RunAtStartup;
                SetRunAtStartup(IsRunAtStartupSelected);

                CurrentPhoto = await dataService.GetLatPhoto();
                if (CurrentPhoto != null)
                {
                    CurrentPhoto = await unsplashService.GetPhoto(CurrentPhoto.Id);
                }
                else
                {
                    await NextImage();
                }

                Application.Current.Exit += OnApplicationExit;
                SystemEvents.PowerModeChanged += OnPowerChange;
                SystemEvents.SessionSwitch += OnSessionSwitch;
                scheduleService.Start(() => NextImage());
            }
            catch (Exception e)
            {
                Debug.Assert(false, e.Message);
                throw;
            }
        }

        private void OnApplicationExit(object sender, EventArgs e)
        {
            SystemEvents.PowerModeChanged -= OnPowerChange;
            SystemEvents.SessionSwitch -= OnSessionSwitch;
        }

        private void OnPowerChange(object sender, PowerModeChangedEventArgs e)
        {
            switch (e.Mode)
            {
                case PowerModes.Resume:
                    scheduleService.Resume();
                    break;

                case PowerModes.Suspend:
                    scheduleService.Stop();
                    break;
            }
        }

        private void OnSessionSwitch(object sender, SessionSwitchEventArgs e)
        {
            switch (e.Reason)
            {
                case SessionSwitchReason.SessionLock:
                    // Going into lock/standby screen
                    scheduleService.Stop();
                    break;

                case SessionSwitchReason.SessionUnlock:
                    // Back from lock/standby
                    scheduleService.Resume();
                    break;
            }
        }

        private void SetRunAtStartup(bool runAtStartup)
        {
            if (runAtStartup)
            {
                Utility.OperatingSystem.CreateStartupShortcut();
            }
            else
            {
                Utility.OperatingSystem.RemoveStartupShortcut();
            }
        }

        private async Task ToggleRunAtStartup()
        {
            IsRunAtStartupSelected = !IsRunAtStartupSelected;
            settings.RunAtStartup = IsRunAtStartupSelected;
            SetRunAtStartup(IsRunAtStartupSelected);
        }
    }
}