using System;
using System.Net;
using System.Text;
using System.Threading;
using UnityEngine;

public class HTTPServer : MonoBehaviour {
    public int port = 4000;
    private HttpListener listener;
    private Thread listenerThread;
    private CarController car;
    private float pendingSpeed = 0f;
    private float pendingSteering = 0f;
    private bool hasUpdate = false;

    void Start() {
        car = FindFirstObjectByType<CarController>();
        listener = new HttpListener();
        listener.Prefixes.Add($"http://*:{port}/");
        listener.Start();
        listenerThread = new Thread(ListenLoop);
        listenerThread.IsBackground = true;
        listenerThread.Start();
        Debug.Log($"HTTP Server listening on port {port}");
    }

    void Update() {
        if (hasUpdate && car != null) {
            car.speed = pendingSpeed;
            car.steering = pendingSteering;
            hasUpdate = false;
        }
    }

    void ListenLoop() {
        while (listener.IsListening) {
            var ctx = listener.GetContext();
            var req = ctx.Request;
            var res = ctx.Response;
            res.Headers.Add("Access-Control-Allow-Origin", "*");
            res.Headers.Add("Access-Control-Allow-Methods", "GET, OPTIONS");
            if (req.HttpMethod == "OPTIONS") {
                res.StatusCode = 200; res.Close(); continue;
            }
            string speedStr = req.QueryString["speed"] ?? "0";
            string steerStr = req.QueryString["steer"] ?? "0";
            pendingSpeed = Mathf.Clamp(float.Parse(speedStr), -1f, 1f);
            pendingSteering = Mathf.Clamp(float.Parse(steerStr), -1f, 1f);
            hasUpdate = true;
            byte[] buf = Encoding.UTF8.GetBytes("OK");
            res.ContentLength64 = buf.Length;
            res.OutputStream.Write(buf, 0, buf.Length);
            res.Close();
        }
    }

    void OnApplicationQuit() { listener?.Stop(); }
}