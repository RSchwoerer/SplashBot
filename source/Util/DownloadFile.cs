using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace SplashBot.Util
{
    public static class DownloadFile
    {
        public static async Task<string> Get(string url)
        {
            var filePath = Path.GetTempFileName();
            await new WebClient().DownloadFileTaskAsync(new Uri(url), filePath);
            return filePath;





            /*
                var s = new WebClient().OpenRead(uri.ToString());

                var img = Image.FromStream(s);
                var tempPath = Path.Combine(Path.GetTempPath(), "wallpaper.bmp");
                img.Save(tempPath, ImageFormat.Bmp);

             */
        }
    }
}