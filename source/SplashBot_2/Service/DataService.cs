using SQLite;
using System.Diagnostics;
using System.IO;
using System.Text.Json;

namespace SplashBot_2.Service
{
    [Table("PhotoHistory")]
    public class PhotoHistory
    {
        [Column("id")]
        public string Id { get; set; }

        [PrimaryKey, AutoIncrement]
        [Column("key")]
        public int Key { get; set; }

        [Ignore]
        public Models.Dto.Photo? Photo
        {
            get => PhotoJson == null ? null : JsonSerializer.Deserialize<Models.Dto.Photo>(PhotoJson);
            set => PhotoJson = value == null ? null : JsonSerializer.Serialize(value);
        }

        [Column("photoJson")]
        public string? PhotoJson { get; set; }
    }

    internal class DataService
    {
        private const string Col_PhotoHistory = "PhotoHistory";

        private const string dbpath = "splashbotdata";
        private SQLiteConnection _db;

        public DataService()
        {
            Initialize();
        }

        public async Task<int> AddPhotoToPhotoHistory(Unsplasharp.Models.Photo photo, string downloadLink = "")
        {
            var historyEntry = new PhotoHistory
            {
                Id = photo.Id,
                Photo = photo
            };

            return _db.Insert(historyEntry);
        }

        public async Task<Unsplasharp.Models.Photo?> GetLatPhoto()
        {
            try
            {
                var latestHistory = _db.Table<PhotoHistory>()
                    .OrderByDescending(p => p.Key)
                    .FirstOrDefault();

                if (latestHistory?.Photo == null)
                    return null;

                return latestHistory.Photo;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error getting latest photo: {ex.Message}");
                return null;
            }
        }

        public async Task<List<Unsplasharp.Models.Photo>?> GetPhotoHistory(int start = 0, int count = 1)
        {
            return new List<Unsplasharp.Models.Photo>();
            //using (var db = await CreateDb())
            //{
            //    var c = db.CreateCommand();
            //    c.CommandText = $@"SELECT * FROM PhotoHistory WHERE key > {start} ORDER BY key DESC LIMIT {count}";
            //    var result = await c.ExecuteReaderAsync();
            //    return result.ParsePhotos();
            //}
        }

        public async Task InitializeAppSettings(AppSettings appSettings)
        {
            try
            {
                _db.CreateTable<AppSettings>();

                // Try to retrieve existing settings
                var settings = _db.Table<AppSettings>().FirstOrDefault();

                if (settings != null)
                {
                    appSettings.RunAtStartup = settings.RunAtStartup;
                    appSettings.SearchText = settings.SearchText;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Error initializing app settings: {e.Message}");
            }
        }

        public async Task<int> UpdateAppSetting(string setting, string searchText)
        {
            return 1;
            //return await ExecuteNonQueryAsync(
            //     $@"UPDATE AppSettings SET {setting} = '{searchText}' WHERE key = 0");
        }

        private async Task Initialize()
        {
            try
            {
                var dbfilename = Path.Join(Path.GetDirectoryName(Environment.ProcessPath), dbpath);
                _db = new SQLiteConnection(dbfilename);
                _db.CreateTable<PhotoHistory>();
            }
            catch (Exception e)
            {
                throw;
            }

            //            await ExecuteNonQueryAsync(
            //                @"CREATE TABLE IF NOT EXISTS
            //                    PhotoHistory(
            //                    key INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
            //                    Id TEXT NOT NULL,
            //                    User_Name TEXT NULL,
            //                    Description TEXT NULL,
            //                    Links_Html TEXT NULL,
            //                    Url_Small TEXT NULL,
            //                    DownloadLink TEXT NULL,
            //                    Timestamp TEXT
            //                    )
            //                ");

            //            await ExecuteNonQueryAsync(
            //                @"CREATE TABLE IF NOT EXISTS
            //                    ""AppSettings"" (
            //	                    ""key""	INTEGER CHECK(""key"" = 0),
            //	                    ""RunAtStartup""	TEXT,
            //	                    ""NextUpdateTime""	TEXT,
            //	                    ""SearchText""	TEXT,
            //                   PRIMARY KEY(""key"")
            //)
            //                ");

            //            var settings = await ExecuteReader(
            //                @"SELECT * FROM AppSettings WHERE key = 0");

            //            if (settings.Rows.Count == 0)
            //            {
            //                await ExecuteNonQueryAsync(
            //                    @"INSERT INTO
            //                        AppSettings(
            //                            key,
            //                            RunAtStartup,
            //                            SearchText
            //                        )
            //                      VALUES (
            //                            0, 'true', 'bears,earth,landscape,northern lights'
            //                      )");
            //            }

            //await ExecuteNonQueryAsync(
            //    @"CREATE TABLE IF NOT EXISTS
            //        Search
            //    ");
        }
    }
}