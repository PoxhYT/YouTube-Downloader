import { downloadSong, getId, getName, pathToFfmpeg } from "./common";
import { spawn } from "child_process";

import bodyParser from "body-parser";
import express from "express";
import helmet from "helmet";
import path from "path";

const app = express();

app.use(helmet());

app.use(
  bodyParser.urlencoded({
    extended: true,
  })
);

app.use(bodyParser.json());

app.get("/", (req, res) => {
  res.json({ message: "Hey" });
});

app.post("/song", async (req, res) => {
  const { url, info } = req.body;

  if (!url) {
    res.status(400).json({ message: "Url Missing" });
    return;
  } else if (typeof url !== "string") {
    res.status(400).json({ message: "Url Missing" });
    return;
  }

  if (info === "true") {
    const image =
      "https://img.youtube.com/vi/" + getId(url) + "/maxresdefault.jpg";
    const name = await getName(url);
    console.log(name);
    res.json({
      imageurl: image,
      songName: name,
    });
    return;
  }

  downloadSong(getId(url), (pathToFile, outputPath, songName) => {
    const fixedMp3 = path.join(outputPath, "fixed.mp3");
    const command = spawn(pathToFfmpeg, ["-i", pathToFile, "-codec:a", "libmp3lame", "-b:a", "320k", fixedMp3]);

    command.on('error', (error) => {
      res.status(500).json({message: "Something wen't wrong...", error: JSON.stringify(error)});
    });

    command.on("close", code => {
      res.sendFile(fixedMp3);
    });
  });
});

const port = process.env.PORT || 3001;
app.listen(port, () =>
  console.log(`Server is running at: http://localhost:${port}`)
);
