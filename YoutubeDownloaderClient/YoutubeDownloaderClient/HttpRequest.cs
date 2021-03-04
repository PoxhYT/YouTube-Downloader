using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using System.Net;
using System.Text.RegularExpressions;
using System.ComponentModel;

namespace YoutubeDownloaderClient
{
    public static class HttpRequest
    {
        private static readonly HttpClient client = new HttpClient();

        private static MusicInfo info;
        private static string url;
        private static string outputPath;

        public static async Task getSong(string localurl)
        {
            var localinfo = await getInfos(localurl);

            WebClient webclient = new WebClient();
            var localoutputPath = $"./{localinfo.songName}.jpg";

            webclient.DownloadFileAsync(new Uri(localinfo.imageurl), localoutputPath);

            info = localinfo;
            url = localurl;
            outputPath = localoutputPath;

            webclient.DownloadFileCompleted += wc_DownloadFileCompleted;  
        }


        public static async Task<MusicInfo> getInfos(string localurl)
        {
            var values = new Dictionary<string, string>
            {
                { "url", localurl },
                { "info", "true" }
            };
            var content = new FormUrlEncodedContent(values);
            var response = await (await client.PostAsync("http://localhost:3001/song", content)).Content.ReadAsStringAsync();
            Console.WriteLine(response);
            return JsonConvert.DeserializeObject<MusicInfo>(response);
        }

        public static void setCover(string pathToMp3)
        {
            TagLib.File file = TagLib.File.Create(pathToMp3);
            TagLib.Picture pic = new TagLib.Picture();
            pic.Type = TagLib.PictureType.FrontCover;
            pic.Description = "Cover";
            pic.MimeType = System.Net.Mime.MediaTypeNames.Image.Jpeg;
            MemoryStream ms = new MemoryStream();
            ms.Position = 0;
            pic.Data = TagLib.ByteVector.FromStream(ms);
            file.Tag.Pictures = new TagLib.IPicture[] { pic };
            file.Save();
            ms.Close();
        }

        private static async void wc_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {

            var imageBytes = File.ReadAllBytes(outputPath);

            var values = new Dictionary<string, string>
            {
                { "url", url },
            };

            var content = new FormUrlEncodedContent(values);

            var response = await client.PostAsync("http://localhost:3001/song", content);

            string fileName = info.songName + ".mp3";

            await using var ms = await response.Content.ReadAsStreamAsync();
            await using var fs = File.Create(fileName);
            ms.Seek(0, SeekOrigin.Begin);
            ms.CopyTo(fs);
            fs.Close();
            ms.Close();
            var tFile = TagLib.File.Create(fileName);

            var cover = new TagLib.Id3v2.AttachmentFrame
            {
                Type = TagLib.PictureType.FrontCover,
                Description = "Cover",
                MimeType = System.Net.Mime.MediaTypeNames.Image.Jpeg,
                Data = imageBytes,
                TextEncoding = TagLib.StringType.UTF16
            };

            tFile.Tag.Pictures = new TagLib.IPicture[] { cover };
            tFile.Save();
        }
    }
}
