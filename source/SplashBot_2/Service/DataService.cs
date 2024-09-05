using Microsoft.Data.Sqlite;
using SplashBot_2.Models;
using Unsplasharp.Models;

namespace SplashBot_2.Service
{
    internal class DataService
    {
        private const string Col_PhotoHistory = "PhotoHistory";

        private const string dbpath = "splashbotdata";

        public DataService()
        {
            Initialize();
        }

        public async Task<int> AddPhotoToPhotoHistory(Photo photo, string downloadLink = "")
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

        public async Task<Photo?> GetLatPhoto()
        {
            var result = await ExecuteReader(
                @"SELECT * FROM PhotoHistory ORDER BY key DESC LIMIT 1");

            if (!result.HasRows) return null;

            return result.ParsePhotos().FirstOrDefault();
        }

        public async Task<List<Photo>?> GetPhotoHistory(int start = 0, int count = 1)
        {
            var result = await ExecuteReader(
                $@"SELECT * FROM PhotoHistory WHERE key > {start} ORDER BY key DESC LIMIT {count}");
            return result.ParsePhotos();
        }

        public async Task InitializeAppSettings(AppSettings appSettings)
        {
            var result = await ExecuteReader(
                @"SELECT * FROM AppSettings WHERE key = 0");
            result.Read();

            appSettings.Foo = "testing";
            appSettings.SearchText = result["SearchText"] as string;
        }

        public async Task<int> UpdateAppSetting(string setting, string searchText)
        {
            return await ExecuteNonQueryAsync(
                 $@"UPDATE AppSettings SET {setting} = '{searchText}' WHERE key = 0");
        }

        private static async Task<SqliteConnection> CreateDb()
        {
            var db = new SqliteConnection($"Filename={dbpath}");
            await db.OpenAsync();
            return db;
        }

        private static async Task<int> ExecuteNonQueryAsync(string command)
        {
            using (var db = await CreateDb())
            {
                var c = db.CreateCommand();
                c.CommandText = command;
                return await c.ExecuteNonQueryAsync();
            }
        }

        private static async Task<SqliteDataReader> ExecuteReader(string command)
        {
            using (var db = await CreateDb())
            {
                var c = db.CreateCommand();
                c.CommandText = command;
                return await c.ExecuteReaderAsync();
            }
        }

        private async Task Initialize()
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

            await ExecuteNonQueryAsync(
                @"CREATE TABLE IF NOT EXISTS
                    AppSettings(
                    key INTEGER PRIMARY KEY CHECK (key = 0),
                    NextUpdateTime TEXT,
                    SearchText TEXT
                    )
                ");

            var settings = await ExecuteReader(
                @"SELECT * FROM AppSettings WHERE key = 0");

            if (!settings.HasRows)
            {
                await ExecuteNonQueryAsync(
                    @"INSERT INTO
                        AppSettings(
                            key,
                            SearchText
                        )
                      VALUES (
                            0, 'bears,earth,landscape,northern lights'
                      )");
            }

            //await ExecuteNonQueryAsync(
            //    @"CREATE TABLE IF NOT EXISTS
            //        Search
            //    ");
        }
    }
}