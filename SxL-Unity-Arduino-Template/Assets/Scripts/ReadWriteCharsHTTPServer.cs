using System.Net;
using UnityEngine;

/// <summary>
/// Unity as HTTP Server - Arduino as HTTP Client
/// Arduino polls Unity for 'a'/'b' data, sends 'c'/'d' commands
/// </summary>
public class ReadWriteCharsHTTPServer : MonoBehaviour
{
    [Header("Server Settings")]
    public int serverPort = 8080;
    
    [Header("Unity Objects")]
    public GameObject cube;

    [Header("Movement")]
    public float moveSpeed = 5f;

    [Header("LED Control")]
    private int ledPin = 4;

    // Joystick input from website (thread-safe)
    private float joyX = 0f;
    private float joyY = 0f;
    private readonly object joyLock = new object();

    private HTTPServer server;
    private char currentChar = 'a';
    private int charsSent = 0;
    private int sendTimer = 0;
    private int sendTime = 100;  // Switch char every 1 second
    
    // Queue for thread-safe color changes
    private System.Collections.Generic.Queue<Color> colorQueue = new System.Collections.Generic.Queue<Color>();
    private readonly object queueLock = new object();
    
    void Start() {
        server = new HTTPServer(serverPort);
        
        // Register routes
        server.AddRoute("/", HandleRoot);
        server.AddRoute("/data", HandleData);
        server.AddRoute("/command", HandleCommand);
        
        server.Start();
        
        Debug.Log($"Unity HTTP Server started on port {serverPort}");
        Debug.Log("Configure Arduino to connect to this Unity IP");
    }
    
    void Update() {
        // Alternate between 'a' and 'b'
        if (sendTimer > 0) {
            sendTimer -= 1;
        } else {
            sendTimer = sendTime;
            charsSent += 1;
            currentChar = (charsSent % 2 == 0) ? 'a' : 'b';
        }

        // Move cube from joystick (X = horizontal, Y = forward/back in world Z)
        float x = 0f, y = 0f;
        lock (joyLock) {
            x = joyX;
            y = joyY;
        }
        if (cube != null && (x != 0f || y != 0f)) {
            Vector3 move = new Vector3(x, 0f, y) * (moveSpeed * Time.deltaTime);
            cube.transform.position += move;
        }
        
        // Process queued color changes on main thread
        lock (queueLock) {
            while (colorQueue.Count > 0) {
                Color color = colorQueue.Dequeue();
                if (cube != null) {
                    cube.GetComponent<Renderer>().material.color = color;
                    Debug.Log($"[Main Thread] Cube color changed to {color}");
                }
            }
        }
    }
    
    void OnDestroy() {
        server?.Stop();
    }
    
    // Root endpoint
    string HandleRoot(HttpListenerContext context) {
        return "<h1>Unity HTTP Server</h1>" +
               "<p>GET /data - Get current character (a or b)</p>" +
               "<p>GET /command?cmd=c - Change cube to RED</p>" +
               "<p>GET /command?cmd=d - Change cube to BLUE</p>" +
               "<p>GET /command?color=RRGGBB - Set cube color (hex)</p>" +
               "<p>GET /command?joyX=-1..1&joyY=-1..1 - Joystick (move cube)</p>";
    }
    
    // Data endpoint - Arduino polls this
    string HandleData(HttpListenerContext context) {
        Debug.Log($"[Server] Sending char: {currentChar}");
        return currentChar.ToString();
    }
    
    // Command endpoint - Arduino sends commands; website can send ?color=RRGGBB (hex)
    string HandleCommand(HttpListenerContext context) {
        string colorHex = context.Request.QueryString["color"];
        if (!string.IsNullOrEmpty(colorHex)) {
            Color? c = ParseHexColor(colorHex);
            if (c.HasValue) {
                lock (queueLock) {
                    colorQueue.Enqueue(c.Value);
                }
                Debug.Log($"[Server] Command: color={colorHex} - Queued color");
                return "Cube set to " + colorHex;
            }
        }

        string cmd = context.Request.QueryString["cmd"];
        if (cmd == "c") {
            lock (queueLock) {
                colorQueue.Enqueue(Color.red);
            }
            Debug.Log("[Server] Command: c - Queued RED color");
            return "Cube set to RED";
        }
        if (cmd == "d") {
            lock (queueLock) {
                colorQueue.Enqueue(Color.blue);
            }
            Debug.Log("[Server] Command: d - Queued BLUE color");
            return "Cube set to BLUE";
        }

        // Joystick from website: ?joyX=-0.5&joyY=0.8 (values -1 to 1)
        string jx = context.Request.QueryString["joyX"];
        string jy = context.Request.QueryString["joyY"];
        if (!string.IsNullOrEmpty(jx) && !string.IsNullOrEmpty(jy) &&
            float.TryParse(jx, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float fx) &&
            float.TryParse(jy, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float fy)) {
            fx = Mathf.Clamp(fx, -1f, 1f);
            fy = Mathf.Clamp(fy, -1f, 1f);
            lock (joyLock) {
                joyX = fx;
                joyY = fy;
            }
            return "joy " + fx.ToString("F2") + "," + fy.ToString("F2");
        }

        return "Invalid command";
    }

    // Parse hex string (e.g. "ff0000" or "#ff0000") to Unity Color
    static Color? ParseHexColor(string hex) {
        if (string.IsNullOrEmpty(hex)) return null;
        hex = hex.TrimStart('#');
        if (hex.Length != 6) return null;
        if (!int.TryParse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber, null, out int r)) return null;
        if (!int.TryParse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber, null, out int g)) return null;
        if (!int.TryParse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber, null, out int b)) return null;
        return new Color(r / 255f, g / 255f, b / 255f);
    }
}
