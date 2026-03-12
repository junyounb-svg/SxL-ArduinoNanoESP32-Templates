# LAN Mobile Controller Assignment – Step-by-Step Guide

This guide walks you through completing the assignment: **a simple interactive Unity scene with at least two controllable elements** (steering + accel/brake) **and an HTTP server in Unity** that responds to a **mobile website controller** on the same LAN.

---

## What You Already Have

- **Unity project**: `Fast&FuriousUnity` with a city + car scene and cameras
- **Network**: Laptop and iPad on **NETGEAR39-5G-1**
- **Node.js** installed
- **Scripts**: `HTTPServer.cs` (Unity HTTP server), `CarController.cs` (car movement)

---

## Part 1: Unity Scene Setup (Laptop)

### Step 1.1 – Open your driving scene

1. In Unity, open your **city + car** scene (e.g. **SimplePoly City - Low Poly Assets_Demo Scene** or whichever scene has the road and car).
2. If you haven’t added a car yet: drag a vehicle prefab from  
   `Assets/SimplePoly City - Low Poly Assets/Prefab/Vehicles/` into the scene and place it on the road.

### Step 1.2 – Add CarController to the car

1. Select the **car** GameObject in the Hierarchy.
2. In the Inspector, click **Add Component**.
3. Search for **CarController** and add it.
4. Leave default values (or set **Max Speed** and **Turn Speed** if you like).

### Step 1.3 – Add the HTTP server to the scene

1. In the Hierarchy, right‑click → **Create Empty**. Name it **HTTPServer**.
2. With **HTTPServer** selected, in the Inspector click **Add Component**.
3. Search for **HTTPServer** (script in `Assets/Scripts/HTTP/HTTPServer.cs`) and add it.
4. Set **Port** to **4000** (default). This is the port Unity will listen on for commands from the mobile site.

### Step 1.4 – Save and test (keyboard optional)

1. Save the scene (Ctrl+S / Cmd+S).
2. Press **Play**. In the Console you should see: **"HTTP Server listening on port 4000"**.
3. Optional: Temporarily add keyboard input to `CarController` to test driving (e.g. W/S for speed, A/D for steering) so you know the car moves correctly. You can remove keyboard later and rely only on the HTTP values.

---

## Part 2: Mobile Controller Website (Same Laptop)

The website is in **HTTP-Smart-Controller**. It serves a page with **steering** and **accel/brake** that send `speed` and `steer` to Unity.

### Step 2.1 – Open the project in Cursor

1. Open Cursor.
2. Open the folder: **LAN Mobile Website Interface Template** (the one that contains `HTTP-Smart-Controller` and `Fast&FuriousUnity`).

### Step 2.2 – Install dependencies (if not already done)

1. Open the terminal in Cursor (Terminal → New Terminal).
2. Go into the **HTTP-Smart-Controller** folder and install:

   ```bash
   cd "HTTP-Smart-Controller"
   npm install
   ```

### Step 2.3 – Start the web server

1. In the same terminal, run:

   ```bash
   npm run dev
   ```

2. You should see something like: **"Server running at http://localhost:3000"**.
3. Leave this terminal running (do not press Ctrl+C yet). This server **delivers the website** to your browser and iPad.

### Step 2.4 – Test on your laptop first

1. In a browser, go to: **http://localhost:3000**
2. You should see the **Driving Controller** page (steering + accel/brake).
3. Set **Unity host** to **localhost** and **port** to **4000** (or use the defaults if they match).
4. In Unity, press **Play** so the HTTP server is listening.
5. On the website, press **Gas** or **Brake** and move the **Steering** slider. The car in Unity should move and turn.

If it works on the laptop, the same URL with your laptop’s **LAN IP** will work on the iPad.

---

## Part 3: Find Your Laptop’s LAN IP Address

Your iPad will connect to the **website** and to **Unity** using this IP.

1. Open Terminal (Mac) or Command Prompt (Windows).
2. Run:
   - **Mac**: `ifconfig | grep "inet "`
   - **Windows**: `ipconfig`
3. Find the address for your **Wi‑Fi adapter** (e.g. **192.168.1.x** or **10.0.0.x**). That is your **LAN IP**.  
   Ignore `127.0.0.1` (that’s only for the same machine).

Example: if your Wi‑Fi shows **192.168.1.105**, then:
- Website on iPad: **http://192.168.1.105:3000**
- Unity server: **192.168.1.105:4000**

---

## Part 4: Use the Controller on Your iPad (Same LAN)

### Step 4.1 – Unity and website must be running

1. On the **laptop**:

   **Unity**
   - Open your driving scene in Unity and press **Play**.
   - The HTTP server will listen on port **4000** (you should see “HTTP Server listening on port 4000” in the Unity Console).

   **Website (Terminal)**
   - **Faster:** Open the **LAN Mobile Website Interface Template** folder in **Cursor** (File → Open Folder…, then choose that folder). In Cursor, open the integrated terminal (**Terminal → New Terminal**, or `` Ctrl+` `` / **Cmd+`**). The terminal starts in the project root, so you only need:
     ```bash
     cd "HTTP-Smart-Controller"
     npm run dev
     ```
   - **Or** use the system Terminal: open **Terminal** (Spotlight → “Terminal”), then go into the project folder. If you’re already in the project root (**LAN Mobile Website Interface Template**), run:
     ```bash
     cd "HTTP-Smart-Controller"
     ```
     If Terminal opened somewhere else (e.g. your home folder), use the full path:
     ```bash
     cd "/Users/YourUsername/Desktop/CMU Academics/2nd Year - 2nd Sem/Lab/Exercise 6/LAN Mobile Website Interface Template/HTTP-Smart-Controller"
     ```
     (Replace `YourUsername` with your Mac username, or drag **HTTP-Smart-Controller** into the Terminal window to paste its path.)
   - Start the website server:
     ```bash
     npm run dev
     ```
   - You should see **“Server running at http://localhost:3000”**. Leave the terminal open; **Ctrl+C** stops the server.

### Step 4.2 – Open the website on the iPad

1. On the iPad, connect to **NETGEAR39-5G-1** (same network as the laptop).
2. Open **Safari** (or Chrome).
3. In the address bar type: **http://YOUR_LAPTOP_IP:3000**  
   Example: **http://192.168.1.105:3000**
4. The Driving Controller page should load.

### Step 4.3 – Point the controller at Unity

1. On the webpage, find **Unity Server Settings**.
2. Set **Unity host** to the **same LAN IP** (e.g. **192.168.1.105**).  
   The page can default this to the same host as the page; if you opened by IP, that’s correct.
3. Set **Unity port** to **4000**.
4. Tap **Update settings** (if there is one) or just use the controls.

### Step 4.4 – Drive

1. Use **Gas** and **Brake** for speed.
2. Use the **Steering** slider (or buttons) for left/right.
3. The car in Unity on your laptop should respond.

---

## Part 5: Troubleshooting

### Website doesn’t load on iPad

- Check laptop and iPad are on **NETGEAR39-5G-1**.
- Use the laptop’s **LAN IP** (e.g. 192.168.x.x), not `localhost`.
- Make sure **npm run dev** is still running on the laptop.

### Buttons/slider don’t move the car

- Unity must be **in Play mode** and show **"HTTP Server listening on port 4000"**.
- On the iPad, **Unity host** must be the laptop’s LAN IP and **port** must be **4000**.
- Try opening **http://LAPTOP_IP:4000** in the iPad browser; you should get a short response (e.g. "OK") when the request is valid. If you get “cannot connect,” Unity isn’t reachable (firewall or wrong IP/port).

### Firewall blocking Unity (port 4000)

- **Mac**: System Settings → Network → Firewall (or Security & Privacy → Firewall). Allow **Unity** for incoming connections, or temporarily turn the firewall off to test.
- **Windows**: Allow **Unity** through the firewall (search “Allow an app through Windows Firewall”) for Private networks.

### Port 8080 or 4000 already in use

- In Unity, change the **HTTPServer** script’s **Port** (e.g. to **5000**).
- In the website’s Unity Server Settings, set **port** to the same number (e.g. **5000**).
- Restart Unity Play and refresh the browser.

### Changes to the website don’t show up

- Stop the server with **Ctrl+C** in the terminal, then run **npm run dev** again.
- Hard refresh the browser (e.g. Ctrl+Shift+R or Cmd+Shift+R).

---

## Summary Checklist

- [ ] Unity scene has a **car** with **CarController** and an **HTTPServer** GameObject with **HTTPServer** script (port 4000).
- [ ] **npm install** and **npm run dev** run in **HTTP-Smart-Controller** without errors.
- [ ] On the laptop, **http://localhost:3000** shows the controller and **http://localhost:4000** (with Unity in Play) responds.
- [ ] Laptop’s LAN IP is known; on iPad you open **http://LAN_IP:3000** and set Unity to **LAN_IP:4000**.
- [ ] With Unity in Play and the website open on the iPad, steering and accel/brake control the car.

---

## Assignment Requirements Met

- **Two controllable elements**: (1) **Steering**, (2) **Accel/Brake** (gas and brake).
- **Unity HTTP server**: Listens on port 4000 and applies `speed` and `steer` to the car.
- **Mobile website**: Smart controller interface on a separate device (iPad) on the same LAN, sending HTTP requests to Unity.

Once all steps work, you’re done. Good luck.
