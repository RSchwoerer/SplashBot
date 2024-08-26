using SplashBot.Services;

namespace SplashBot
{
    public static class ChannelFactory
    {
        public static UnsplashChannel[] CollectionChannels() =>
            new[]
            {
                new UnsplashChannel
                {
                    Name = "Backgrounds / Textures",
                    Url = "1368747",
                    Type = UnsplashChannelType.Collection
                },
                new UnsplashChannel
                {
                    Name = "Earth Planets",
                    Url = "894",
                    Type = UnsplashChannelType.Collection
                },
                new UnsplashChannel
                {
                    Name = "Good Doggos of Unsplash",
                    Url = "1270951",
                    Type = UnsplashChannelType.Collection
                },
                new UnsplashChannel
                {
                    Name = "One Color",
                    Url = "1103088",
                    Type = UnsplashChannelType.Collection
                },
                new UnsplashChannel
                {
                    Name = "Patterns and Textures",
                    Url = "175083",
                    Type = UnsplashChannelType.Collection
                },
                new UnsplashChannel
                {
                    Name = "Undisturbed Pattern Wallpapers",
                    Url = "151521",
                    Type = UnsplashChannelType.Collection
                },
            };

        public static UnsplashChannel RandomChannel() =>
            new UnsplashChannel { Name = "Random", Type = UnsplashChannelType.Random };

        public static UnsplashChannel[] SearchChannels()
        {
            return new[]
                   {
                       new UnsplashChannel
                       {
                           Name = "Nature",
                           Url = "nature",
                           Type = UnsplashChannelType.Search
                       },
                       new UnsplashChannel
                       {
                           Name = "Earth && Planets",
                           Url = "earth,planets",
                           Type = UnsplashChannelType.Search
                       },
                       new UnsplashChannel
                       {
                           Name = "Doggos",
                           Url = "dog,doggo",
                           Type = UnsplashChannelType.Search
                       }
                   };
        }
    }
}