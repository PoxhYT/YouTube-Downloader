using System;
using System.Collections.Generic;
using System.Text;

namespace YoutubeDownloaderClient
{
    public class MusicInfo
    {
        public readonly string songName;
        public readonly string imageurl;

        public MusicInfo(string songName, string imageurl)
        {
            this.songName = songName;
            this.imageurl = imageurl;
        }
    }
}
