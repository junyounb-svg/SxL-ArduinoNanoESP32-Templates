using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;

/// <summary>
/// Helper HTTP server with route registration. Used by ReadWriteCharsHTTPServer (cube/Arduino demo).
/// For the driving controller, use the HTTPServer MonoBehaviour component instead.
/// </summary>
public class HttpServerHelper
{
    private readonly int _port;
    private HttpListener _listener;
    private Thread _listenerThread;
    private readonly Dictionary<string, Func<HttpListenerContext, string>> _routes = new Dictionary<string, Func<HttpListenerContext, string>>(StringComparer.OrdinalIgnoreCase);

    public HttpServerHelper(int port)
    {
        _port = port;
    }

    public void AddRoute(string path, Func<HttpListenerContext, string> handler)
    {
        path = (path ?? "").TrimEnd('/');
        if (string.IsNullOrEmpty(path)) path = "/";
        _routes[path] = handler;
    }

    public void Start()
    {
        _listener = new HttpListener();
        _listener.Prefixes.Add($"http://*:{_port}/");
        _listener.Start();
        _listenerThread = new Thread(ListenLoop) { IsBackground = true };
        _listenerThread.Start();
    }

    public void Stop()
    {
        try
        {
            _listener?.Stop();
        }
        catch { }
    }

    private void ListenLoop()
    {
        while (_listener != null && _listener.IsListening)
        {
            try
            {
                var ctx = _listener.GetContext();
                var req = ctx.Request;
                var res = ctx.Response;
                res.Headers.Add("Access-Control-Allow-Origin", "*");
                res.Headers.Add("Access-Control-Allow-Methods", "GET, POST, OPTIONS");
                if (req.HttpMethod == "OPTIONS")
                {
                    res.StatusCode = 200;
                    res.Close();
                    continue;
                }

                string path = req.Url.AbsolutePath.TrimEnd('/');
                if (string.IsNullOrEmpty(path)) path = "/";

                string body = "Not Found";
                if (_routes.TryGetValue(path, out var handler))
                {
                    try
                    {
                        body = handler(ctx) ?? "";
                    }
                    catch (Exception ex)
                    {
                        body = "Error: " + ex.Message;
                    }
                }

                byte[] buf = Encoding.UTF8.GetBytes(body);
                res.ContentLength64 = buf.Length;
                res.OutputStream.Write(buf, 0, buf.Length);
                res.Close();
            }
            catch (HttpListenerException)
            {
                break;
            }
            catch (Exception)
            {
                // ignore
            }
        }
    }
}
