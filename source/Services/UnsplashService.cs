using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SplashBot.Services
{
    public class UnsplashChannel
    {
        public string Url { get; set; }
        public string Name { get; set; }
        public UnsplashChannelType Type { get; set; }
    }

    public class Wallpaper
    {
        public string id { get; set; }
        public string title { get; set; }
        public string Url { get; set; }
    }

    public enum UnsplashChannelType
    {
        Collection, Search, Random
    }

    internal class UnsplashService
    {
        public async Task<Wallpaper> GetWallpaper(UnsplashChannel channel)
        {
            string screenWidth = Screen.PrimaryScreen.Bounds.Width.ToString();
            string screenHeight = Screen.PrimaryScreen.Bounds.Height.ToString();
            var resolution = $"{screenWidth}x{screenHeight}";

            string url;
            switch (channel.Type)
            {
                case UnsplashChannelType.Collection:
                    url = $@"https://source.unsplash.com/collection/{channel.Url}/{resolution}";
                    break;

                case UnsplashChannelType.Search:
                    url = $@"https://source.unsplash.com/featured/{resolution}/?{channel.Url}";
                    break;

                case UnsplashChannelType.Random:
                    url = @"https://source.unsplash.com/random";
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            return new Wallpaper { Url = url };
        }
    }
}