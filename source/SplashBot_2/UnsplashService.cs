using System.IO;
using System.Net.Http;
using System.Windows;
using Unsplasharp;

namespace SplashBot_2
{
    internal class UnsplashService
    {
        private readonly UnsplasharpClient client;

        public UnsplashService()
        {
            var key = Environment.GetEnvironmentVariable("SPLASHBOT_APIKEY", EnvironmentVariableTarget.User);

            if (key == null ||
                string.IsNullOrEmpty(key))
            {
                MessageBox.Show(
                    "Missing Unsplash Access Key.\r\n" +
                    "Create an environment variable named 'SPLASHBOT_APIKEY' with Access Key value.",
                    "SplashBot Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                Application.Current.Shutdown();
            }

            client = new UnsplasharpClient(key);
        }

        public event EventHandler<EventArgs> ApiLimitUpdated;

        public int ApiCallsLimit => client.MaxRateLimit;
        public int ApiCallsRemaining => client.RateLimitRemaining;
        private static HttpClient downloaderClient { get; } = new HttpClient();

        public async Task<Unsplasharp.Models.Photo> GetPhoto(string photoId)
        {
            var photo = await client.GetPhoto(photoId);
            OnApiLimitUpdated();
            return photo;
        }

        public async Task<Unsplasharp.Models.Photo> SetTestBackground(string photoSearchQuery)
        {
            //var photosFound = Task.Run(async () => await client.SearchPhotos("mountains")).GetAwaiter().GetResult();
            // var photosFound = Task.Run(async () => await client.GetRandomPhoto()).GetAwaiter().GetResult();

            // cast truncating ok here.
            var screenWidth = (int)System.Windows.SystemParameters.PrimaryScreenWidth;
            var screenHeight = (int)System.Windows.SystemParameters.PrimaryScreenHeight;

            var photoResults = await client.GetRandomPhoto(UnsplasharpClient.Orientation.Landscape, "", photoSearchQuery, screenWidth, screenHeight, 1);
            OnApiLimitUpdated();

            var firstPhoto = photoResults.First();

            var dl = await client.GetPhotoDownloadLink(firstPhoto.Id);
            OnApiLimitUpdated();

            var filePath = await Download(dl);

            if (Environment.OSVersion.Version.Major >= 8)
            {
                // Windows 10
                Utility.SetWallpaper.Apply(null, filePath);
            }
            else
            {
                Utility.SetWallpaperLegacy.Apply(filePath);
            }

            return firstPhoto;
        }

        private async Task<string> Download(string url)
        {
            var filePath = Path.GetTempFileName();
            await downloaderClient.DownloadFileTaskAsync(new Uri(url), filePath);
            return filePath;
        }

        private void OnApiLimitUpdated()
        {
            ApiLimitUpdated?.Invoke(this, EventArgs.Empty);
        }
    }
}