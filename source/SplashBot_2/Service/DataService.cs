using SQLite;
using System.Diagnostics;
using System.IO;

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

        [Column("photo")]
        public Models.Dto.Photo? Photo { get; set; }
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
            return 0;
            //return await ExecuteNonQueryAsync(
            //    @$"INSERT INTO
            //        PhotoHistory (
            //            Id,
            //            User_Name,
            //            Description,
            //            Links_Html,
            //            Url_Small,
            //            DownloadLink,
            //            Timestamp
            //        )
            //        VALUES (
            //            '{photo.Id}',
            //            '{photo.User.Name}',
            //            '{photo.Description}',
            //            '{photo.Links.Html}',
            //            '{photo.Urls.Small}',
            //            '{downloadLink}',
            //            '{new DateTimeOffset(DateTime.Now.ToUniversalTime())}'
            //        )
            //    ");
        }

        public async Task<Unsplasharp.Models.Photo?> GetLatPhoto()
        {
            return new Unsplasharp.Models.Photo();
            //using (var db = await CreateDb())
            //{
            //    var c = db.CreateCommand();
            //    c.CommandText = @"SELECT * FROM PhotoHistory ORDER BY key DESC LIMIT 1";
            //    var result = await c.ExecuteReaderAsync();

            //    if (!result.HasRows) return null;

            //    return result.ParsePhotos().FirstOrDefault();
            //}
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
                //using (var db = await CreateDb())
                //{
                //    var c = db.CreateCommand();
                //    c.CommandText = @"SELECT * FROM AppSettings WHERE key = 0";
                //    var ad = new SQLiteDataAdapter(c);
                //    DataTable dt = new();
                //    ad.Fill(dt); //fill the datasource

                //    if (dt.Rows.Count > 0)
                //    {
                //        appSettings.Foo = "testing";
                //        appSettings.RunAtStartup = (dt.Rows[0]["RunAtStartup"] as string ?? "").ToLower() == "true";
                //        appSettings.SearchText = dt.Rows[0]["SearchText"] as string;
                //    }

                //    //var result = await c.ExecuteReaderAsync();
                //    //result.Read();
                //    //appSettings.Foo = "testing";
                //    //appSettings.RunAtStartup = (result["RunAtStartup"] as string).ToLower() == "true";
                //    //appSettings.SearchText = result["SearchText"] as string;
                //}
            }
            catch (Exception e)
            {
                Debug.WriteLine("err");
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