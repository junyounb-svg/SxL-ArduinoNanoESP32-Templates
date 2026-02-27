# HTTP Smart Controller

A simple Node.js website with an interactive button that changes color on click and communicates with Unity via HTTP.

## Features

- Interactive button that cycles through 10 different colors
- Click counter to track total clicks
- Real-time color display
- Modern, responsive design
- REST API for state management
- **Unity Integration** - Sends color changes to Unity HTTP server in real-time
- Unity connection status indicator
- Configurable Unity server settings

## Setup

1. Install dependencies:
```bash
npm install
```

2. Start the server:
```bash
npm start
```
or
```bash
npm run dev
```

3. Open your browser and navigate to:
```
http://localhost:3000
```

## Unity Integration

This controller communicates with the Unity HTTP Server (`ReadWriteCharsHTTPServer.cs`) to change the cube color in real-time.

### How to Connect:

1. **Start Unity:**
   - Open your Unity project with `ReadWriteCharsHTTPServer.cs`
   - Make sure the server port is set to `4000` (default)
   - Press Play in Unity

2. **Start Node.js Server:**
   - Run `npm start` in this directory
   - The server will run on port `3000`

3. **Click the Button:**
   - Each button click sends the new color to Unity via HTTP
   - The Unity cube will change to match the button color
   - Connection status is displayed on the webpage

### Communication Details:

- **Node.js Server:** `localhost:3000`
- **Unity Server:** `localhost:4000` (default)
- **Protocol:** HTTP GET requests to `/command?r=<0-255>&g=<0-255>&b=<0-255>`
- **Color Format:** Hex colors (#3498db) are converted to RGB (52, 152, 219) before sending

### Changing Unity Server Address:

If Unity is running on a different machine or port:
1. Use the settings form on the webpage to update Unity host/port
2. Or edit `UNITY_HOST` and `UNITY_PORT` in `server.js`
3. Restart the Node.js server

## How It Works

- Click the button to cycle through different colors
- The server tracks the current color and click count
- Each color change is sent to Unity's `/command` endpoint with RGB values
- Unity updates the cube color in real-time
- Connection status is checked every 3 seconds

## API Endpoints

- `GET /api/state` - Get current button state (color and click count)
- `POST /api/click` - Update button state on click and send to Unity
- `GET /api/unity/status` - Check Unity connection status

## Technologies Used

- Node.js
- Express.js
- HTML5
- CSS3
- JavaScript (ES6+)
- Unity C# HTTP Server Integration
