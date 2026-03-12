using System.Net;
using UnityEngine;

/// <summary>
/// Unity as HTTP Server - Arduino as HTTP Client
/// Arduino polls Unity for 'a'/'b' data, sends 'c'/'d' commands
/// </summary>
public class ReadWriteCharsHTTPServer : MonoBehaviour
{
    [Header("Server Settings")]
    public int serverPort = 4000;
    
    [Header("Unity Objects")]
    public GameObject cube;
    
    [Header("LED Control")]
    private int _ledPin = 4;
    
    private HttpServerHelper server;
    private char currentChar = 'a';
    private int charsSent = 0;
    private int sendTimer = 0;
    private int sendTime = 100;  // Switch char every 1 second
    
    // Queue for thread-safe color changes
    private System.Collections.Generic.Queue<Color> colorQueue = new System.Collections.Generic.Queue<Color>();
    private readonly object queueLock = new object();
    
    void Start() {
        server = new HttpServerHelper(serverPort);
        
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
               "<p>GET /command?r=122&g=162&b=247 - Set cube color (0-255 RGB)</p>";
    }
    
    // Data endpoint - Arduino polls this
    string HandleData(HttpListenerContext context) {
        Debug.Log($"[Server] Sending char: {currentChar}");
        return currentChar.ToString();
    }
    
    // Command endpoint - Arduino or web sends commands
    // Supports: /command?cmd=c|d  OR  /command?r=255&g=0&b=0 (0-255)
    string HandleCommand(HttpListenerContext context) {
        string cmd = context.Request.QueryString["cmd"];
        string rStr = context.Request.QueryString["r"];
        string gStr = context.Request.QueryString["g"];
        string bStr = context.Request.QueryString["b"];
        
        // RGB from web button (or any client)
        if (!string.IsNullOrEmpty(rStr) && !string.IsNullOrEmpty(gStr) && !string.IsNullOrEmpty(bStr)) {
            if (int.TryParse(rStr, out int r) && int.TryParse(gStr, out int g) && int.TryParse(bStr, out int b)) {
                r = Mathf.Clamp(r, 0, 255);
                g = Mathf.Clamp(g, 0, 255);
                b = Mathf.Clamp(b, 0, 255);
                Color color = new Color(r / 255f, g / 255f, b / 255f);
                lock (queueLock) {
                    colorQueue.Enqueue(color);
                }
                Debug.Log($"[Server] Command: RGB({r},{g},{b}) - Queued color");
                return $"Cube set to RGB({r},{g},{b})";
            }
        }
        
        // Legacy: single-char commands
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
        
        return "Invalid command (use cmd=c|d or r,g,b query params)";
    }
}
