# Arduino Nano ESP32 Examples

Simple Arduino-to-Arduino communication examples using the Arduino Nano ESP32.

## 📋 Overview

These examples demonstrate wireless communication between two Arduino Nano ESP32 boards using WiFi. Perfect for learning IoT basics and building distributed sensor networks.

---

## 🌐 Examples

### 1. **UDP Simple Send/Receive**

**Folders**: `ArduinoESP32NanoUDPSimpleSend` + `ArduinoESP32NanoUDPSimpleReceive`

**How it works**: One Arduino sends sensor data via UDP, the other receives and controls an LED.

#### **Sender** (ArduinoESP32NanoUDPSimpleSend)
- **Hardware**:
  - Potentiometer at A0 (controls brightness 0-255)
  - Button at pin 2 (on/off command)
  - LED at pin 4 (local feedback)
- **Does**: Reads sensors → Sends UDP packets every 100ms
- **LED behavior**: Shows button state (ON when pressed)

#### **Receiver** (ArduinoESP32NanoUDPSimpleReceive)
- **Hardware**:
  - LED at pin 4 (controlled remotely)
- **Does**: Receives UDP packets → Controls LED brightness and on/off
- **LED behavior**: 
  - Button ON → LED brightness = potentiometer value
  - Button OFF → LED off

#### **Setup**:
1. Update WiFi credentials in both sketches
2. Upload **Receiver** first → note its IP from Serial Monitor
3. Update **Sender** sketch with Receiver's IP
4. Upload **Sender**
5. Turn potentiometer, press button → Receiver LED responds!

#### **Message Format**: `"brightness,button"` (e.g., `"128,1"`)

---

### 2. **HTTP Simple Client/Server**

**Folders**: `ArduinoESP32NanoHTTPSimpleClient` + `ArduinoESP32NanoHTTPSimpleServer`

**How it works**: One Arduino runs HTTP server, the other polls it as a client. Same hardware setup as UDP example.

#### **Client** (ArduinoESP32NanoHTTPSimpleClient)
- **Hardware**:
  - Potentiometer at A0
  - Button at pin 2
  - LED at pin 4 (local feedback)
- **Does**: Reads sensors → Sends HTTP GET requests every 100ms
- **Endpoint**: `GET /control?brightness=X&button=X`

#### **Server** (ArduinoESP32NanoHTTPSimpleServer)
- **Hardware**:
  - LED at pin 4 (controlled remotely)
- **Does**: Receives HTTP requests → Controls LED
- **Port**: 80 (standard HTTP)
- **Endpoint**: `GET /control?brightness=X&button=X`

#### **Setup**:
1. Update WiFi credentials in both sketches
2. Upload **Server** first → note its IP
3. Update **Client** sketch with Server's IP
4. Upload **Client**
5. Turn potentiometer, press button → Server LED responds!

#### **Debugging**: You can test the server using a web browser:
- Navigate to `http://[SERVER_IP]/control?brightness=128&button=1`

---

## 🔧 Hardware Requirements

### For Each Arduino:
- **Arduino Nano ESP32** board
- **LED** + 220Ω resistor (optional if using built-in LED)
- **For Sender/Client only**:
  - 10kΩ potentiometer
  - Push button
  - Breadboard and jumper wires

### Wiring:

**Sender/Client Arduino**:
```
Potentiometer:
  - VCC → 3.3V
  - GND → GND
  - Signal → A0

Button:
  - One side → Pin 2
  - Other side → GND
  (Internal pullup enabled in code)

LED:
  - Anode (+) → Pin 4
  - Cathode (-) → 220Ω resistor → GND
```

**Receiver/Server Arduino**:
```
LED:
  - Anode (+) → Pin 4
  - Cathode (-) → 220Ω resistor → GND
```

---

## 🚀 Getting Started

### Prerequisites
1. **Arduino IDE** with ESP32 board support:
   - Install "Arduino ESP32 Boards" by Arduino
   - Select Board: **Arduino Nano ESP32**

2. **WiFi Network**:
   - 2.4GHz WiFi (ESP32 doesn't support 5GHz)
   - Know your SSID and password

### Quick Start Guide

1. **Update WiFi Credentials**:
   ```cpp
   const char *ssid = "YOUR_WIFI_SSID";
   const char *password = "YOUR_WIFI_PASSWORD";
   ```

2. **Upload Receiver/Server First**:
   - Upload the sketch
   - Open Serial Monitor (9600 or 115200 baud)
   - Note the IP address printed (e.g., 192.168.0.101)

3. **Configure Sender/Client**:
   - Update the receiver/server IP in the code
   - Upload the sketch

4. **Test**:
   - Turn potentiometer → LED brightness changes
   - Press button → LED turns on/off

---

## 🆚 UDP vs HTTP - Which to Use?

| Feature | UDP | HTTP |
|---------|-----|------|
| **Speed** | Very fast | Slower (request/response overhead) |
| **Latency** | Low (~10-50ms) | Medium (~50-200ms) |
| **Reliability** | Packets can be lost | Guaranteed delivery |
| **Debugging** | Wireshark, packet capture | Web browser, cURL |
| **Simplicity** | More code | Cleaner REST API |
| **Use Case** | Real-time control, games | Status updates, commands |

**Recommendation**: 
- Use **UDP** for real-time control (games, robotics)
- Use **HTTP** for monitoring, status updates, or when debugging matters

---

## 🐛 Troubleshooting

### Arduino Won't Connect to WiFi
- ✅ Check SSID and password spelling
- ✅ Ensure WiFi is 2.4GHz (not 5GHz)
- ✅ Check Serial Monitor for error messages
- ✅ Press reset button to see initialization messages

### Communication Not Working
- ✅ Both Arduinos connected to WiFi (check Serial Monitor)
- ✅ Correct IP addresses configured
- ✅ Same WiFi network
- ✅ Router doesn't have "AP Isolation" enabled
- ✅ Firewall not blocking ports

### LED Not Responding
- ✅ Check wiring (LED polarity, resistor)
- ✅ Check Serial Monitor for received messages
- ✅ Verify sender is actually sending (check logs)
- ✅ Try different LED pin if built-in LED doesn't work

---

## 📊 Serial Monitor Output

You should see clear logs showing communication:

**Sender/Client**:
```
WiFi Connected!
Client IP: 192.168.0.102
→ Sent: Brightness=128, Button=ON
→ Sent: Brightness=200, Button=OFF
```

**Receiver/Server**:
```
WiFi Connected!
Server IP: 192.168.0.101
← Received: Brightness=128, Button=ON
← Received: Brightness=200, Button=OFF
```

---

## 💡 Tips

- **Always upload receiver/server first** to get its IP address
- **Use Serial Monitor** to verify WiFi connection and debug
- **Start simple**: Test with one example before moving to complex projects
- **IP addresses change**: If Arduino gets a new IP, update the sender/client code
- **Power cycle**: Reset both Arduinos if connection is lost

---

## 🔗 Related Examples

See the **SxL-Unity-Arduino-Template** folder for Unity integration examples using the same communication methods!

---

## 📝 License

Educational templates - free to use and modify!
