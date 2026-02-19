#include <WiFi.h>
#include <WiFiUdp.h>
#include <Wire.h>
#include <LiquidCrystal_I2C.h>

// Install Board: "Arduino Nano ESP32" (or "esp32 by expressif").
// Install library: LiquidCrystal_I2C (Sketch > Include Library > Manage Libraries).

// Replace with your network credentials (e.g. NETGEAR39-5G-1 and its password)
const char *ssid = "NETGEAR39";  // e.g. "NETGEAR39-5G-1"
const char *password = "freshbreeze181";

// Set up UDP
WiFiUDP udp;
const char *udpAddress = "172.168.10.11";  // Your Mac/PC IP (run ifconfig on Mac, ipconfig on Windows)
const int udpSenderPort = 5006;   // Arduino sends to Unity on this port
const int udpReceiverPort = 5005; // Arduino receives from Unity on this port
String sentMessage;
String receivedMessage;

// LED
const int ledPin = 4;

// LCD1602 I2C. Using ESP32 default I2C pins (21, 22). Wire: LCD SDA -> GPIO21, LCD SCL -> GPIO22.
// If still no device found, try 4 and 5 again and swap the two wires (SDA<->SCL).
#define LCD_SDA 21
#define LCD_SCL 22
#define LCD_I2C_ADDR 0x27
LiquidCrystal_I2C lcd(LCD_I2C_ADDR, 16, 2);

int charsSent = 0;
int sendTimer = 0;
int sendTime = 300;   // send every ~3 sec (300 * 10ms) to avoid flooding and endPacket errors

// Fake LCD messages: show at consistent intervals (no Unity needed)
const char *lcdMessages[] = {
  "Systems nominal.",
  "All sensors OK.",
  "Ready to assist.",
  "Scanning...",
  "No threats found.",
  "Connection stable.",
  "Standing by.",
  "Awaiting input.",
  "Processing...",
  "Cyborg online.",
  "Hello, human.",
  "What next?",
  "At your service.",
  "Listening.",
  "All good here.",
};
const int lcdMessageCount = 15;
int messageTimer = 0;
const int messageInterval = 500;  // show new message every ~5 sec (500 * 10ms)

void scanI2C() {
  Serial.println();
  Serial.print("I2C scan SDA=");
  Serial.print(LCD_SDA);
  Serial.print(" SCL=");
  Serial.println(LCD_SCL);
  int n = 0;
  for (byte addr = 0x08; addr < 0x78; addr++) {
    Wire.beginTransmission(addr);
    if (Wire.endTransmission() == 0) {
      Serial.print("  Device at 0x");
      Serial.println(addr, HEX);
      n++;
    }
  }
  if (n == 0) Serial.println("  No I2C device found. Try pins 21,22 or swap SDA/SCL.");
  Serial.println();
}

void setup() {
  Serial.begin(9600);
  Serial.println("UDPReadWriteChars setup START");  // First line - if you don't see this, wrong port or wrong sketch
  delay(1000);

  Wire.begin(LCD_SDA, LCD_SCL);
  Serial.println("I2C scan:");
  scanI2C();
  delay(500);

  lcd.init();
  lcd.backlight();
  lcd.setCursor(0, 0);
  lcd.print("Cyborg ready!");
  lcd.setCursor(0, 1);
  lcd.print("Waiting WiFi...");
  randomSeed(analogRead(0));  // for random messages

  initUDP();

  pinMode(ledPin, OUTPUT);
  lcd.setCursor(0, 1);
  lcd.print("OK ");
  lcd.print(WiFi.localIP().toString());
  messageTimer = messageInterval;  // show IP for one interval, then start random messages
}

void loop() {
  // Check WiFi connection status
  if (WiFi.status() != WL_CONNECTED) {
    Serial.println("WiFi disconnected! Attempting to reconnect...");
    digitalWrite(ledPin, LOW);
    WiFi.begin(ssid, password);
    delay(5000);
    return;
  }

  // Listen for a message from UDP (UDP -> Arduino ESP32)
  receivedMessage = receiveUDP();

  if (receivedMessage.length() > 0) {
    // LED: 'c' = on, 'd' = off (from Unity C/D keys or your logic)
    if (receivedMessage == "c") {
      digitalWrite(ledPin, HIGH);
    } else if (receivedMessage == "d") {
      digitalWrite(ledPin, LOW);
    }
    // LCD messages are faked below; we don't display UDP text on the LCD
  }

  // Fake LCD: show random conversational message at consistent intervals
  if (messageTimer > 0) {
    messageTimer--;
  } else {
    messageTimer = messageInterval;
    int idx = random(lcdMessageCount);
    const char *msg = lcdMessages[idx];
    lcd.setCursor(0, 1);
    lcd.print("                ");  // clear line 1 (16 chars)
    lcd.setCursor(0, 1);
    lcd.print(msg);
    Serial.println("LCD: " + String(msg));
  }

  //Sending data via UDP on a repeating timer (only if WiFi connected):
  if(sendTimer > 0) {
    sendTimer -= 1;
  } else {
    sendTimer = sendTime;

    //Switch between sending chars 'a' or 'b':
    charsSent += 1;
    if(charsSent % 2 == 0) {
      sentMessage = "a";
    } else {
      sentMessage = "b";
    }

    // Send message to UDP
    sendUDP(sentMessage);
  }

  //Quick delay:
  delay(10);
}

void initUDP() {
  // Connect to WiFi
  WiFi.begin(ssid, password);
  Serial.println("Connecting to WiFi...");
  Serial.print("SSID: ");
  Serial.println(ssid);
  
  while (WiFi.status() != WL_CONNECTED) {
    delay(1000);
    Serial.print(".");
  }
  
  Serial.println("");
  Serial.println("Connected to WiFi!");
  Serial.print("SSID: ");
  Serial.println(WiFi.SSID());
  Serial.print("Arduino IP address: ");
  Serial.println(WiFi.localIP());
  Serial.print("Subnet Mask: ");
  Serial.println(WiFi.subnetMask());
  Serial.print("Gateway IP: ");
  Serial.println(WiFi.gatewayIP());
  Serial.print("DNS IP: ");
  Serial.println(WiFi.dnsIP());
  Serial.print("MAC Address: ");
  Serial.println(WiFi.macAddress());

  // Start listening for UDP on the receive port
  udp.begin(udpReceiverPort);
  Serial.println("UDP Initialized");
}

String receiveUDP() {
  int packetSize = udp.parsePacket();
  if (packetSize > 0) {
    char incomingPacket[255];
    int len = udp.read(incomingPacket, 255);
    if (len > 0) {
      incomingPacket[len] = 0; //Null-terminate string for formatting.
      String message = String(incomingPacket);
      Serial.println("← Received: '" + message + "' from " + udp.remoteIP().toString() + ":" + String(udp.remotePort()) + " (" + String(len) + " bytes)");
      return message;
    } else {
      Serial.println("ERROR: Receive failed - read 0 bytes");
      return "";
    }
  }
  return "";
}

void sendUDP(String message) {
  int beginResult = udp.beginPacket(udpAddress, udpSenderPort);
  if (beginResult == 0) {
    Serial.println("ERROR: Send failed - beginPacket");
    return;
  }
  
  udp.write((const uint8_t *)message.c_str(), message.length());
  int endResult = udp.endPacket();
  if (endResult != 1) {
    Serial.println("ERROR: Send failed - endPacket (result: " + String(endResult) + ")");
  }
}
