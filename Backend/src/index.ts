import { downloadSong, getId, getName } from "./common";

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

  downloadSong(getId(url), (pathToFile) => {
    res.sendFile(pathToFile);
  });
});

const port = process.env.PORT || 3001;
app.listen(port, () =>
  console.log(`Server is running at: http://localhost:${port}`)
);
