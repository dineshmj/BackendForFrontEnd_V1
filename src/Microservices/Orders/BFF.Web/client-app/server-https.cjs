// server-https.cjs
const { createServer } = require('https');
const { parse } = require('url');
const fs = require('fs');
const path = require('path');
const next = require('next');

const dev = true;
const hostname = 'localhost';
const port = 33500; // or whatever your SPA HTTPS port is

const app = next({ dev, hostname, port });
const handle = app.getRequestHandler();

// Adjust certsDir to where your certs actually are
// For example: client-app/certs/localhost.key & localhost.crt
const certsDir = path.join(__dirname, 'certs');

const httpsOptions = {
  key: fs.readFileSync(path.join(certsDir, 'localhost.key')),
  cert: fs.readFileSync(path.join(certsDir, 'localhost.crt')),
};

app.prepare().then(() => {
  createServer(httpsOptions, (req, res) => {
    const parsedUrl = parse(req.url, true);
    handle(req, res, parsedUrl);
  }).listen(port, () => {
    console.log(`✅ NextJS SPA running at https://localhost:${port}`);
  });
});