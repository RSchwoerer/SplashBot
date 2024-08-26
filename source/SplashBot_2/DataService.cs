using Microsoft.Data.Sqlite;
using Unsplasharp.Models;

namespace SplashBot_2
{
    internal class DataService
    {
        private const string Col_PhotoHistory = "PhotoHistory";
        private const string dbpath = "splashbotdata";

        public async Task<int> AddPhotoToPhotoHistory(Unsplasharp.Models.Photo photo, string downloadLink = "")
        {
            return await ExecuteNonQueryAsync(
                @$"INSERT INTO
                    PhotoHistory (
                        Id,
                        User_Name,
                        Links_Html,
                        Url_Small,
                        DownloadLink,
                        Timestamp
                    )
                    VALUES (
                        '{photo.Id}',
                        '{photo.User.Name}',
                        '{photo.Links.Html}',
                        '{photo.Urls.Small}',
                        '{downloadLink}',
                        '{new DateTimeOffset(DateTime.Now.ToUniversalTime())}'
                    )
                ");
        }

        public async Task<Photo> GetLatPhoto()
        {
            var result = await ExecuteReader(
                @"SELECT * FROM PhotoHistory ORDER BY key DESC LIMIT 1");

            if (!result.HasRows) return null;

            result.Read();
            return new Photo
            {
                // key = result.GetString(0)
                Id = result.GetString(1),
                User = new User
                { Name = result.GetString(2) },
                Links = new PhotoLinks
                { Html = result.GetString(3) },
                Urls = new Urls
                { Small = result.GetString(4) },
            };
        }

        public async Task Initialize()
        {
            await ExecuteNonQueryAsync(
                @"CREATE TABLE IF NOT EXISTS
                    PhotoHistory(
                    key INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                    Id TEXT NOT NULL,
                    User_Name TEXT NULL,
                    Links_Html TEXT NULL,
                    Url_Small TEXT NULL,
                    DownloadLink TEXT NULL,
                    Timestamp TEXT
                    )
                ");

            //await ExecuteNonQueryAsync(
            //    @"CREATE TABLE IF NOT EXISTS
            //        Search
            //    ");
        }

        private static async Task<SqliteCommand> CreateCommand()
        {
            var db = new SqliteConnection($"Filename={dbpath}");
            await db.OpenAsync();
            return db.CreateCommand();
        }

        private static async Task<int> ExecuteNonQueryAsync(string command)
        {
            var c = await CreateCommand();
            c.CommandText = command;
            return await c.ExecuteNonQueryAsync();
        }

        private static async Task<SqliteDataReader> ExecuteReader(string command)
        {
            var c = await CreateCommand();
            c.CommandText = command;
            return await c.ExecuteReaderAsync();
        }
    }
}