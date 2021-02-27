import bodyParser from "body-parser";
import express from "express";
import ytdl from "ytdl-core";
import fetch from "node-fetch";
import fs from "fs";

async function main() {
  const app = express();

  app.use(
    bodyParser.urlencoded({
      extended: true,
    })
  );

  app.use(bodyParser.json())

  app.get("/", (req, res) => {
    res.json({ message: "Hey" });
  });

  app.post("/song", async (req, res) => {
    const { url, info } = req.body;
    if(info === "true") {
      const image = "https://img.youtube.com/vi/" + getId(url) + "/maxresdefault.jpg";
      const name = await getName(url);
      console.log(name);
      res.json({
        imageurl:image,
        songName:name      
      }) 
      return;     
    }
    const stream = ytdl(url, {
      quality: "highest",
      filter: "audioonly",
      format: {bitrate:320}
    });
    stream.pipe(fs.createWriteStream("Test.mp3"));
  });

  const port = process.env.PORT || 3001;
  app.listen(port, () =>
    console.log(`Server is running at: http://localhost:${port}`)
  );
}

function getId(url:string) {
  if(url.includes("youtube.com")) {
    const array = url.split("=")  
    return array[array.length-1];
    return;
  }
  const array = url.split("/")  
  return array[array.length-1];
}

async function getName(url:string) {
  const data = JSON.parse(await (await fetch("https://noembed.com/embed?url=https://www.youtube.com/watch?v=" + getId(url))).text());
  return data.title;
}

main();
