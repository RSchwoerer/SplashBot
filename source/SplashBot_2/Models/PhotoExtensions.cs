using System.Data.Common;
using Unsplasharp.Models;

namespace SplashBot_2.Models
{
    public static class PhotoExtensions
    {
        public static List<Photo>? ParsePhotos(this DbDataReader rd)
        {
            if (!rd.HasRows) return null;

            var photos = new List<Photo>();
            while (rd.Read())
            {
                var photo = new Photo
                {
                    // key = rd.GetString(0)
                    Id = rd["Id"] as string,
                    User = new User
                    {
                        Name = rd["User_Name"] as string
                    },
                    Links = new PhotoLinks
                    {
                        Html = rd["Links_Html"] as string
                    },
                    Urls = new Urls
                    {
                        Small = rd["Url_Small"] as string
                    },
                };
                photos.Add(photo);
            }
            return photos;
        }
    }
}