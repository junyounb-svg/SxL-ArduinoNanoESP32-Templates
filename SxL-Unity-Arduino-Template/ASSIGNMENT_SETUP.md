# Assignment Setup: Cyborg Head + Wireless LCD

This project uses **one scene** that combines:
- **HID (wired):** Your Arduino joystick + toggle button move the system mouse → the Cube (or your cyborg head) looks at the mouse.
- **UDP (wireless):** Unity sends random “cyborg speech” to the Arduino → LCD displays it; C/D keys (or your logic) still control the LED.

## HID vs UDP (quick reference)

| | **HID (wired)** | **UDP (wireless)** |
|---|-----------------|---------------------|
| **What** | Arduino acts as USB keyboard/mouse | WiFi messages between Unity and Arduino |
| **Direction** | One-way: Arduino → computer | Two-way |
| **Unity** | Uses `Input.mousePosition`, `Input.GetKey` | Uses `UDPSender` / `UDPReceiver` |
| **Your use** | Joystick + button (mouse control) | LED + LCD (cyborg speech) |

---

## Unity: Scene and scripts

1. **Scene:** `Assets/Scenes/ReadWriteCharsUDP.unity`
   - **Cyborg-Pawn** (prefab) has `CyborgHeadLook` on its root; the **head** bone is assigned so the cyborg’s head looks at the mouse. The Cube no longer has this script.
   - **UDP** GameObject has `CyborgSpeechUDP`: it sends a random greeting every few seconds to the Arduino. Sender is already assigned.

2. **Wired Arduino (HID):** No Unity setup. Plug in the board; when the button toggles mouse mode, the joystick moves the cursor and the head follows.

3. **UDP settings (Inspector):**
   - **UDPReceiver:** Port **5006**, Auto Start ON.
   - **UDPSender:** Your **Arduino’s IP**, Port **5005**, Auto Start ON.
   - Find your Mac IP: Terminal → `ifconfig` (e.g. `en0` → `inet`).

---

## Arduino: Wireless (UDP + LED + LCD)

1. **WiFi:** In `UDPReadWriteChars.ino` set `ssid` and `password` (e.g. NETGEAR39-5G-1).
2. **Unity’s IP:** Set `udpAddress` to your computer’s IP (same as in Unity UDPSender).
3. **LCD:** Install **LiquidCrystal_I2C** (Sketch → Include Library → Manage Libraries). Connect LCD1602 I2C: **SDA → A4**, **SCL → A5**, VCC, GND. If the display is blank, try I2C address **0x3F** instead of **0x27** in the sketch.
4. **Ports:** Arduino sends to Unity on **5006**, receives on **5005** (already set).
5. Upload, open Serial Monitor (9600) to see the Arduino’s IP. In the router (e.g. routerlogin.net for Netgear), reserve a **static IP** for the Arduino (DHCP → Address Reservation, use the Arduino’s MAC address).

---

## Quick test

1. **HID:** Run the scene, toggle mouse control on the wired Arduino, move the joystick → the Cyborg-Pawn’s head should follow the cursor.
2. **UDP:** Run the scene with the wireless Arduino on the same network; LCD should show “Cyborg ready” then random phrases every few seconds. Press **C** / **D** in Unity to turn the LED on/off.
