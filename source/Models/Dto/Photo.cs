using SQLite;

namespace SplashBot_2.Models.Dto
{
    public class Photo
    {
        [Column("description")]
        public string Description { get; set; }

        [Column("id")]
        public string Id { get; set; }

        [PrimaryKey, AutoIncrement]
        [Column("key")]
        public int Key { get; set; }

        [Column("Links_Html")]
        public string Links_Html { get; set; }

        [Column("Timestamp")]
        public DateTimeOffset Timestamp { get; set; }

        [Column("Urls_Raw")]
        public string Urls_Raw { get; set; }

        [Column("Urls_Small")]
        public string Urls_Small { get; set; }

        [Column("User_Name")]
        public string User_Name { get; set; }

        public static implicit operator Photo(Unsplasharp.Models.Photo o)
        {
            if (o == null) return null;
            return new Photo
            {
                Id = o.Id,
                User_Name = o.User.Name,
                Description = o.Description,
                Links_Html = o.Links.Html,
                Urls_Small = o.Urls.Small,
                Urls_Raw = o.Urls.Raw,
                Timestamp = new DateTimeOffset(DateTime.Now.ToUniversalTime()),
            };
        }

        public static implicit operator Unsplasharp.Models.Photo(Photo o)
        {
            if (o == null) return null;
            return new Unsplasharp.Models.Photo
            {
                Id = o.Id,
                User = new Unsplasharp.Models.User { Name = o.User_Name },
                Description = o.Description,
                Links = new Unsplasharp.Models.PhotoLinks { Html = o.Links_Html, },
                Urls = new Unsplasharp.Models.Urls { Raw = o.Urls_Raw, Small = o.Urls_Small },
            };
        }
    }
}