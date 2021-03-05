import YoutubeMp3Downloader from "youtube-mp3-downloader";

import fetch from "node-fetch";
import rimraf from "rimraf";
import fs from "fs";
import path from "path";

const pathToFfmpeg = process.env.production ? "/usr/bin/ffmpeg" : "./ffmpeg/bin/ffmpeg.exe";

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

  const data = JSON.parse(await (await fetch(httpUrl)).text());

  return data.title;
}

async function downloadSong(
  id: string,
  callback: (fileOutput: string, usedPath: string, songName: string) => void
): Promise<void> {
  const downloadPath = `./downloads/${id}/`;

  if (fs.existsSync(path.join(process.cwd(), downloadPath))) {
    rimraf.sync(path.join(process.cwd(), downloadPath));
    await delay(50);
  }

    fs.mkdirSync(path.join(process.cwd(), downloadPath).slice(0, -1));

  const outputPath = path.join(process.cwd(), downloadPath)

  var YD = new YoutubeMp3Downloader({
    ffmpegPath: pathToFfmpeg,
    outputPath: outputPath,
    youtubeVideoQuality: "highestaudio",
    queueParallelism: 2,
    progressTimeout: 2000,
    allowWebm: false,
  });

  YD.download(id);

  YD.on("finished", () => {
    const filePath = fs.readdirSync(outputPath)[0];

    callback(path.join(outputPath, filePath), outputPath, filePath);
  });      
}

function delay(ms: number) {
  return new Promise( resolve => setTimeout(resolve, ms) );
}

export { downloadSong, getId, getName, pathToFfmpeg };
