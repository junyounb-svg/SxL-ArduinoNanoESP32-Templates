const express = require('express');
const path = require('path');

const app = express();
const PORT = 3000;

app.use(express.static(path.join(__dirname, 'public')));

app.listen(PORT, () => {
  console.log('Server running at http://localhost:' + PORT);
  console.log('Unity HTTP server should run on port 4000 (same machine or set in controller)');
  console.log('Press Ctrl+C to stop');
});
