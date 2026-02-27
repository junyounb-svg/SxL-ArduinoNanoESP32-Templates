const express = require('express');
const path = require('path');
const http = require('http');

const app = express();
const PORT = 3000;
const UNITY_HOST = 'localhost';
const UNITY_PORT = 4000;

app.use(express.static('public'));
app.use(express.json());

let buttonState = {
  color: '#3498db',
  clickCount: 0,
  unityConnected: false
};

function hexToRgb(hex) {
  const result = /^#?([a-f\d]{2})([a-f\d]{2})([a-f\d]{2})$/i.exec(hex);
  return result ? {
    r: parseInt(result[1], 16),
    g: parseInt(result[2], 16),
    b: parseInt(result[3], 16)
  } : null;
}

function sendColorToUnity(hexColor) {
  const rgb = hexToRgb(hexColor);
  if (!rgb) {
    console.error('Invalid hex color:', hexColor);
    return Promise.reject('Invalid color');
  }

  const url = `http://${UNITY_HOST}:${UNITY_PORT}/command?r=${rgb.r}&g=${rgb.g}&b=${rgb.b}`;
  
  return new Promise((resolve, reject) => {
    const req = http.get(url, (res) => {
      let data = '';
      res.on('data', (chunk) => { data += chunk; });
      res.on('end', () => {
        console.log(`[Unity] Color sent: RGB(${rgb.r},${rgb.g},${rgb.b}) - Response: ${data}`);
        buttonState.unityConnected = true;
        resolve(data);
      });
    });
    
    req.on('error', (err) => {
      console.error('[Unity] Connection failed:', err.message);
      buttonState.unityConnected = false;
      reject(err);
    });
    
    req.setTimeout(2000, () => {
      req.destroy();
      console.error('[Unity] Connection timeout');
      buttonState.unityConnected = false;
      reject(new Error('Timeout'));
    });
  });
}

app.get('/api/state', (req, res) => {
  res.json(buttonState);
});

app.post('/api/click', async (req, res) => {
  buttonState.clickCount++;
  buttonState.color = req.body.color || buttonState.color;
  
  try {
    await sendColorToUnity(buttonState.color);
  } catch (error) {
    console.error('Failed to send color to Unity:', error.message);
  }
  
  res.json(buttonState);
});

app.get('/api/unity/status', (req, res) => {
  res.json({ connected: buttonState.unityConnected });
});

app.listen(PORT, () => {
  console.log(`Server running at http://localhost:${PORT}`);
  console.log(`Unity server expected at http://${UNITY_HOST}:${UNITY_PORT}`);
  console.log('Press Ctrl+C to stop the server');
});
