using SplashBot_2.Models;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
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
            using (var db = await CreateDb())
            {
                var c = db.CreateCommand();
                c.CommandText = @"SELECT * FROM PhotoHistory ORDER BY key DESC LIMIT 1";
                var result = await c.ExecuteReaderAsync();

                if (!result.HasRows) return null;

                return result.ParsePhotos().FirstOrDefault();
            }
        }

        public async Task<List<Photo>?> GetPhotoHistory(int start = 0, int count = 1)
        {
            using (var db = await CreateDb())
            {
                var c = db.CreateCommand();
                c.CommandText = $@"SELECT * FROM PhotoHistory WHERE key > {start} ORDER BY key DESC LIMIT {count}";
                var result = await c.ExecuteReaderAsync();
                return result.ParsePhotos();
            }
        }

        public async Task InitializeAppSettings(AppSettings appSettings)
        {
            try
            {
                using (var db = await CreateDb())
                {
                    var c = db.CreateCommand();
                    c.CommandText = @"SELECT * FROM AppSettings WHERE key = 0";
                    var ad = new SQLiteDataAdapter(c);
                    DataTable dt = new();
                    ad.Fill(dt); //fill the datasource

                    if (dt.Rows.Count > 0)
                    {
                        appSettings.Foo = "testing";
                        appSettings.RunAtStartup = (dt.Rows[0]["RunAtStartup"] as string ?? "").ToLower() == "true";
                        appSettings.SearchText = dt.Rows[0]["SearchText"] as string;
                    }

                    //var result = await c.ExecuteReaderAsync();
                    //result.Read();
                    //appSettings.Foo = "testing";
                    //appSettings.RunAtStartup = (result["RunAtStartup"] as string).ToLower() == "true";
                    //appSettings.SearchText = result["SearchText"] as string;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("err");
            }
        }

        public async Task<int> UpdateAppSetting(string setting, string searchText)
        {
            return await ExecuteNonQueryAsync(
                 $@"UPDATE AppSettings SET {setting} = '{searchText}' WHERE key = 0");
        }

        private static async Task<SQLiteConnection> CreateDb()
        {
            var dbfilename = Path.Join(Path.GetDirectoryName(Environment.ProcessPath), dbpath);
            var cs = new SQLiteConnectionStringBuilder { DataSource = dbfilename };
            var db = new SQLiteConnection(cs.ToString());
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

        private static async Task<DataTable> ExecuteReader(string command)
        {
            DataTable dt = new DataTable();
            using (var db = await CreateDb())
            {
                var c = db.CreateCommand();
                c.CommandText = command;
                var ad = new SQLiteDataAdapter(c);
                ad.Fill(dt); //fill the datasource
                return dt;
                //return await c.ExecuteReaderAsync();
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
                    ""AppSettings"" (
	                    ""key""	INTEGER CHECK(""key"" = 0),
	                    ""RunAtStartup""	TEXT,
	                    ""NextUpdateTime""	TEXT,
	                    ""SearchText""	TEXT,
                   PRIMARY KEY(""key"")
)
                ");

            var settings = await ExecuteReader(
                @"SELECT * FROM AppSettings WHERE key = 0");

            if (settings.Rows.Count == 0)
            {
                await ExecuteNonQueryAsync(
                    @"INSERT INTO
                        AppSettings(
                            key,
                            RunAtStartup,
                            SearchText
                        )
                      VALUES (
                            0, 'true', 'bears,earth,landscape,northern lights'
                      )");
            }

            //await ExecuteNonQueryAsync(
            //    @"CREATE TABLE IF NOT EXISTS
            //        Search
            //    ");
        }
    }
}