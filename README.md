# SxL Arduino Templates

Educational templates for Arduino Nano ESP32 communication - Unity integration and Arduino-to-Arduino examples.

---

## 📦 What's Inside

This repository contains two main sets of examples:

### 🎮 **Unity ↔ Arduino Communication**
**Folder**: `SxL-Unity-Arduino-Template/`

Complete Unity project with multiple communication methods between Unity and Arduino Nano ESP32:
- **Serial** - USB cable communication
- **UDP** - WiFi wireless networking
- **HID Keyboard** - Arduino as USB keyboard device
- **HTTP** - RESTful web API (both Unity as client/server)

**Pattern**: Arduino sends 'a'/'b' to change Unity cube color, Unity sends 'c'/'d' to control Arduino LED.

[📖 Full Unity Documentation →](SxL-Unity-Arduino-Template/README.md)

---

### 🤖 **Arduino ↔ Arduino Communication**
**Folder**: `Arduino Nano ESP32 Examples/`

Simple Arduino-to-Arduino wireless communication examples:
- **UDP Send/Receive** - Fast, low-latency wireless control
- **HTTP Client/Server** - RESTful API between Arduinos

**Hardware**: Sender has potentiometer + button, Receiver has LED. Control brightness and on/off wirelessly!

[📖 Full Arduino Documentation →](Arduino%20Nano%20ESP32%20Examples/README.md)

---

## 🚀 Quick Start

### Option 1: Unity Integration
1. Open `SxL-Unity-Arduino-Template/` in Unity
2. Choose a communication method (Serial, UDP, HTTP, Keyboard)
3. Upload corresponding Arduino sketch from `Assets/Arduino/`
4. Configure and play!

### Option 2: Arduino-to-Arduino
1. Wire up two Arduino Nano ESP32 boards
2. Upload receiver/server sketch first, note IP
3. Upload sender/client sketch with receiver's IP
4. Control LED wirelessly with potentiometer and button!

---

## 🔌 Communication Methods Comparison

| Method | Use Case | Pros | Cons | Wireless |
|--------|----------|------|------|----------|
| **Serial** | Unity prototyping | Simple, reliable, fast | Requires USB cable | ❌ |
| **UDP** | Real-time gaming | Fast, low latency | Setup complexity, firewall | ✅ |
| **HID Keyboard** | Input-only | No drivers needed | One-way only | ❌ |
| **HTTP** | REST API, monitoring | Debuggable, standard | Higher latency | ✅ |

---

## 🛠️ Hardware Requirements

- **Arduino Nano ESP32** (1 for Unity, 2 for Arduino-to-Arduino)
- **USB-C cable** (for Serial/HID/programming)
- **WiFi network** (2.4GHz for UDP/HTTP)
- **Optional for Arduino examples**:
  - Potentiometer (10kΩ)
  - Push button
  - LED + 220Ω resistor
  - Breadboard and wires

---

## 📚 Documentation Structure

```
SxL-Arduino-Templates/
├── README.md                           # This file
├── SxL-Unity-Arduino-Template/         # Unity integration examples
│   ├── README.md                       # Unity documentation
│   ├── Assets/
│   │   ├── Arduino/                    # Arduino sketches
│   │   │   ├── SerialReadWriteChars/
│   │   │   ├── UDPReadWriteChars/
│   │   │   ├── KeyboardReadWriteChars/
│   │   │   ├── HTTPServerReadWriteChars/
│   │   │   └── HTTPClientReadWriteChars/
│   │   ├── Scripts/                    # Unity C# scripts
│   │   └── Scenes/                     # Unity scenes
└── Arduino Nano ESP32 Examples/        # Arduino-to-Arduino examples
    ├── README.md                       # Arduino documentation
    ├── ArduinoESP32NanoUDPSimpleSend/
    ├── ArduinoESP32NanoUDPSimpleReceive/
    ├── ArduinoESP32NanoHTTPSimpleClient/
    └── ArduinoESP32NanoHTTPSimpleServer/
```

---

## 🎯 Learning Path

### Beginner
1. Start with **Unity Serial** example - simplest setup
2. Try **Arduino UDP Send/Receive** - learn wireless basics

### Intermediate
3. **Unity UDP** - add wireless to Unity
4. **Unity HTTP Client** - learn REST APIs

### Advanced
5. **Unity HTTP Server** - Unity serves multiple Arduinos
6. **HID Keyboard** - custom input devices

---

## 🐛 Common Issues & Solutions

### Serial
- **Unity not receiving**: Close Arduino Serial Monitor, check COM port
- **Wrong port**: Use Device Manager (Windows) to find correct COM port

### WiFi (UDP/HTTP)
- **Arduino won't connect**: Check WiFi is 2.4GHz, verify credentials
- **Unity not receiving UDP**: Windows firewall may block - works better on Mac
- **Can't find Arduino IP**: Check router's DHCP client list

### Unity
- **Cube not changing color**: Assign cube in Inspector
- **HTTP "Insecure connection"**: Use `System.Net.Http.HttpClient` (already done)

---

## 💡 Tips

- **Start simple**: Begin with Serial or Arduino-to-Arduino UDP
- **Use Serial Monitor**: Essential for debugging WiFi connection
- **Check IP addresses**: WiFi examples require correct IPs
- **Firewall matters**: Windows may block UDP/HTTP ports
- **Read the logs**: Both Unity Console and Arduino Serial Monitor show detailed status

---

## 🔗 Resources

- [Arduino Nano ESP32 Official Docs](https://docs.arduino.cc/hardware/nano-esp32)
- [ESP32 WiFi Library](https://docs.espressif.com/projects/arduino-esp32/en/latest/api/wifi.html)
- [Unity System.IO.Ports](https://learn.microsoft.com/en-us/dotnet/api/system.io.ports)

---

## 📝 License

These are educational templates - free to use and modify for your projects!

---

## 🤝 Contributing

Found a bug or have an improvement? Feel free to submit issues or pull requests!

---

## 📧 Support

- Check README files in each folder for detailed documentation
- Use Serial Monitor and Unity Console for debugging
- Ensure hardware is wired correctly (see wiring diagrams in docs)

---

**Happy Making!** 🚀
