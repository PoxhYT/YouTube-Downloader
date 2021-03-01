import YoutubeMp3Downloader from "youtube-mp3-downloader";

import fetch from "node-fetch";
import rimraf from "rimraf";
import fs from "fs";

const pathToFfmpeg = "./ffmpeg/bin/ffmpeg.exe";

function getId(url: string): string {
  var video_id = url.split("v=")[1];
  var ampersandPosition = video_id.indexOf("&");
  if (ampersandPosition != -1) {
    video_id = video_id.substring(0, ampersandPosition);
  }

  return video_id;
}

async function getName(url: string) {
  const httpUrl =
    "https://noembed.com/embed?url=https://www.youtube.com/watch?v=" +
    getId(url);

  console.log(httpUrl);
  console.log(httpUrl);
  console.log(httpUrl);

  const data = JSON.parse(await (await fetch(httpUrl)).text());

  return data.title;
}

function downloadSong(
  id: string,
  callback: (fileOutput: string) => void
): void {
  const path = `./downloads/${id}/`;

  if (fs.existsSync(path)) {
    rimraf.sync(path);
  }

  fs.mkdirSync(path);

  var YD = new YoutubeMp3Downloader({
    ffmpegPath: pathToFfmpeg,
    outputPath: path,
    youtubeVideoQuality: "highestaudio",
    queueParallelism: 2,
    progressTimeout: 2000,
    allowWebm: false,
  });

  YD.download(id);

  YD.on("finished", () => {
    const filePath = fs.readdirSync(path)[0];

    callback(path.substring(1) + filePath);
  });
}

export { downloadSong, getId, getName };
