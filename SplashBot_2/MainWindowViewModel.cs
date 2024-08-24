using CommunityToolkit.Mvvm.ComponentModel;
using System.Diagnostics;
using Unsplasharp.Models;

namespace SplashBot_2
{
    internal class MainWindowViewModel : ObservableObject
    {
        private readonly DataService dataService;
        private readonly UnsplashService unsplashService;
        private string apiCallsRemaining;
        private Photo currentPhoto;

        public MainWindowViewModel()
        {
            TaskbarIconViewModel = new TaskbarIconViewModel(this);
            unsplashService = new UnsplashService();
            dataService = new DataService();

            Initialize();
        }

        public string ApiLimitDetails => $"{unsplashService.ApiCallsRemaining}/{unsplashService.ApiCallsLimit} requests remaining this hour";
        public string ApplicationTitle => "SplashBot";
        //public string ApiLimitDetails { get => apiCallsRemaining; set => SetProperty(ref apiCallsRemaining, value); }

        public Unsplasharp.Models.Photo CurrentPhoto { get => currentPhoto; set => SetProperty(ref currentPhoto, value); }

        public TaskbarIconViewModel TaskbarIconViewModel { get; }

        internal async Task Test()
        {
            CurrentPhoto = await unsplashService.SetTestBackground();
            OnPropertyChanged(nameof(ApiLimitDetails));
            await dataService.UpdateCurrentPhoto(CurrentPhoto);
        }

        private async Task Initialize()
        {
            try
            {
                await dataService.Initialize();
                CurrentPhoto = await dataService.GetLatPhoto();
            }
            catch (Exception e)
            {
                Debug.Assert(false, e.Message);
                throw;
            }
        }
    }
}