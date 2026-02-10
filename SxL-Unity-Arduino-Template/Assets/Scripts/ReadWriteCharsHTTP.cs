using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class ReadWriteCharsHTTP : MonoBehaviour
{
    [Header("Arduino HTTP Server")]
    public string arduinoIP = "192.168.0.101";  // Arduino's IP address
    public int arduinoPort = 80;
    
    [Header("Unity Objects")]
    public GameObject cube;
    
    [Header("Settings")]
    public float pollInterval = 0.5f;  // Poll Arduino every 0.5 seconds
    
    private string arduinoURL;
    private char receivedChar = ' ';
    
    void Start() {
        arduinoURL = "http://" + arduinoIP + ":" + arduinoPort;
        StartCoroutine(PollArduinoData());
    }
    
    void Update() {
        // Change cube color based on received character from Arduino
        if (receivedChar == 'a') {
            cube.GetComponent<Renderer>().material.color = Color.red;
        } else if (receivedChar == 'b') {
            cube.GetComponent<Renderer>().material.color = Color.blue;
        }
        
        // Send commands to Arduino when keys pressed
        if (Input.GetKeyDown(KeyCode.C)) {
            StartCoroutine(SendCommand('c'));
        }
        if (Input.GetKeyDown(KeyCode.D)) {
            StartCoroutine(SendCommand('d'));
        }
    }
    
    // Poll Arduino for data (GET /data)
    IEnumerator PollArduinoData() {
        while (true) {
            using (UnityWebRequest request = UnityWebRequest.Get(arduinoURL + "/data")) {
                yield return request.SendWebRequest();
                
                if (request.result == UnityWebRequest.Result.Success) {
                    string response = request.downloadHandler.text.Trim();
                    if (response.Length > 0) {
                        receivedChar = response[0];
                        Debug.Log("Received from Arduino: " + receivedChar);
                    }
                } else {
                    Debug.LogWarning("Failed to get data from Arduino: " + request.error);
                }
            }
            
            yield return new WaitForSeconds(pollInterval);
        }
    }
    
    // Send command to Arduino (GET /command - using GET to avoid HTTP/HTTPS issues)
    IEnumerator SendCommand(char cmd) {
        string url = arduinoURL + "/command?cmd=" + cmd;
        
        using (UnityWebRequest request = UnityWebRequest.Get(url)) {
            yield return request.SendWebRequest();
            
            if (request.result == UnityWebRequest.Result.Success) {
                Debug.Log("Sent command to Arduino: " + cmd + " - Response: " + request.downloadHandler.text);
            } else {
                Debug.LogError("Failed to send command: " + request.error);
            }
        }
    }
}
