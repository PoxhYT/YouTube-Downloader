using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using System.Net;
using System.Text.RegularExpressions;

namespace YoutubeDownloaderClient
{
    public static class HttpRequest
    {
        private static readonly HttpClient client = new HttpClient();

        public static async Task getSong(string url)
        {
            var info = await getInfos(url);

            Console.WriteLine(info.imageurl);

            WebClient webclient = new WebClient();

            var image = "https://www.interactivesearchmarketing.com/wp-content/uploads/2014/06/png-vs-jpeg.jpg";

            //webclient.DownloadFile("https://firebasestorage.googleapis.com/v0/b/angular-letschat.appspot.com/o/preset%2FprofilePictures%2Ffemale-avatar.png?alt=media&token=f6043a05-e29b-4010-9cc5-a0adc90a0fb1", "hello.png");

            byte[] imageBytes = null;

            try
            {
               imageBytes = await webclient.DownloadDataTaskAsync(new Uri(image));
            }
            catch { Console.WriteLine("test!"); }
                  
            //var imageBytes = webclient.DownloadData(url);

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
            return;
            AddMp3Tags(fileName);
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


        public static async Task<MusicInfo> getInfos(string url)
        {
            var values = new Dictionary<string, string>
            {
                { "url", url },
                { "info", "true" }
            };
            var content = new FormUrlEncodedContent(values);
            var response = await (await client.PostAsync("http://localhost:3001/song", content)).Content.ReadAsStringAsync();
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

        public static void AddMp3Tags(string path)
        {
            TagLib.File file = TagLib.File.Create(path);
            setCover(path);
            file.Tag.Title = "GAYPORN";
            file.Tag.Performers = "JAN, Alex".Split(',');
            file.Tag.Album = "HOMO";
            file.Tag.Track = (uint)1;
            file.Tag.Year = (uint)2018;
            file.Save();
        }
    }
}
