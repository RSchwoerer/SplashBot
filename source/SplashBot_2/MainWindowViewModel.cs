using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using Unsplasharp.Models;

namespace SplashBot_2
{
    internal class MainWindowViewModel : ObservableObject
    {
        private readonly DataService dataService;
        private readonly UnsplashService unsplashService;
        private ICommand _ExitApplicationCommand;
        private AsyncRelayCommand _NextImageCommand;
        private string apiCallsRemaining;
        private Photo currentPhoto;
        private string photoSearchQuery = "bears,earth,landscape,northern lights";
        private ScheduleService scheduleService;

        public MainWindowViewModel()
        {
            unsplashService = new UnsplashService();
            unsplashService.ApiLimitUpdated +=
                (sender, args) => { OnPropertyChanged(nameof(ApiLimitDetails)); };
            dataService = new DataService();
            scheduleService = new ScheduleService(() => NextImage());

            Initialize();
        }

        public string ApiLimitDetails =>
            $"{unsplashService.ApiCallsRemaining}/{unsplashService.ApiCallsLimit} requests remaining this hour";

        public string ApplicationTitle =>
            "SplashBot";

        public Photo CurrentPhoto { get => currentPhoto; set => SetProperty(ref currentPhoto, value); }

        public ICommand ExitApplicationCommand => _ExitApplicationCommand ??=
            new DelegateCommand { CommandAction = Application.Current.Shutdown };

        public IAsyncRelayCommand NextImageCommand => _NextImageCommand ??=
            new AsyncRelayCommand(NextImage);

        public string PhotoSearchQuery { get => photoSearchQuery; set => SetProperty(ref photoSearchQuery, value); }

        internal async Task NextImage()
        {
            CurrentPhoto = await unsplashService.SetTestBackground(PhotoSearchQuery);
            await dataService.AddPhotoToPhotoHistory(CurrentPhoto);
        }

        private async Task Initialize()
        {
            try
            {
                await dataService.Initialize();
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
                scheduleService.Start();
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
    }
}