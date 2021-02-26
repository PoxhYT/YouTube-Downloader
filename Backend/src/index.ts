import express from "express";

async function main() {
  const app = express();

  app.get("/", (req, res) => {
    res.json({ message: "Hey" });
  });

  const port = process.env.PORT || 3001;
  app.listen(port, () =>
    console.log(`Server is running at: http://localhost:${port}`)
  );
}

main();
