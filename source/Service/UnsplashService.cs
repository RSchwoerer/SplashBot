using System.IO;
using System.Linq;
using System.Net.Http;
using System.Windows;
using Unsplasharp;

namespace SplashBot_2.Service
{
    internal class UnsplashService
    {
        private readonly UnsplasharpClient client;

        public UnsplashService()
        {
            string? key = GetKey();

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
            var screenWidth = (int)SystemParameters.PrimaryScreenWidth;
            var screenHeight = (int)SystemParameters.PrimaryScreenHeight;

            var photoResults = await client.GetRandomPhoto(UnsplasharpClient.Orientation.Landscape, "", photoSearchQuery, screenWidth, screenHeight, 1);
            Unsplasharp.Models.Photo firstPhoto = null;

            if (photoResults != null && photoResults.Any())
            {
                firstPhoto = photoResults.First();
            }
            else
            {
                // try a fallback call with no query
                var fallbackResults = await client.GetRandomPhoto(UnsplasharpClient.Orientation.Landscape, "", "", screenWidth, screenHeight, 1);
                if (fallbackResults != null && fallbackResults.Any())
                {
                    firstPhoto = fallbackResults.First();
                }
            }

            OnApiLimitUpdated();

            var filePath = string.Empty;
            if (firstPhoto == null)
            {
                // No photos from API — attempt to use a local bundled fallback image
                filePath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "default.jpg");
            }
            else
            {
                var dl = firstPhoto.Urls.Raw;
                filePath = await Download(dl);
            }

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

        private static string? GetKey()
        {
            if (File.Exists("SPLASHBOT_APIKEY"))
            {
                return File.ReadAllText("SPLASHBOT_APIKEY");
            }

            return Environment.GetEnvironmentVariable("SPLASHBOT_APIKEY", EnvironmentVariableTarget.User);
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