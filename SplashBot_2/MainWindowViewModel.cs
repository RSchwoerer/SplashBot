using CommunityToolkit.Mvvm.ComponentModel;
using Unsplasharp.Models;

namespace SplashBot_2
{
    internal class MainWindowViewModel : ObservableObject
    {
        private readonly UnsplashService unsplashService;
        private string apiCallsRemaining;
        private Photo currentPhoto;

        public MainWindowViewModel()
        {
            TaskbarIconViewModel = new TaskbarIconViewModel(this);
            unsplashService = new UnsplashService();
        }

        public string ApiCallsRemaining { get => apiCallsRemaining; set => SetProperty(ref apiCallsRemaining, value); }

        public Unsplasharp.Models.Photo CurrentPhoto { get => currentPhoto; set => SetProperty(ref currentPhoto, value); }

        public TaskbarIconViewModel TaskbarIconViewModel { get; }

        internal async Task Test()
        {
            CurrentPhoto = await unsplashService.SetTestBackground();
            ApiCallsRemaining = unsplashService.ApiCallsRemaining.ToString();
        }
    }
}