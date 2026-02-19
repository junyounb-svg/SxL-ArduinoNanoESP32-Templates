using UnityEngine;

/// <summary>
/// Sends random "cyborg speech" (greetings/phrases) to the wireless Arduino over UDP
/// so the LCD can display them. Assign the same UDPSender used for LED (c/d) commands.
/// </summary>
public class CyborgSpeechUDP : MonoBehaviour
{
    [Header("UDP")]
    [Tooltip("Assign the UDPSender (e.g. on the UDP GameObject). If null, will try GetComponent on this object.")]
    public UDPSender sender;

    [Header("Cyborg speech")]
    [Tooltip("Interval in seconds between sending a random message.")]
    public float intervalSeconds = 3f;
    [Tooltip("Max length for one line (LCD1602 is 16 chars). We send one string; Arduino can split.")]
    public int maxMessageLength = 16;

    static readonly string[] Greetings = new[]
    {
        "HELLO HUMAN",
        "I AM CYBORG",
        "JOYSTICK OK",
        "LOOK AT ME",
        "GREETINGS",
        "SYSTEMS GO",
        "NICE DAY",
        "HELLO WORLD",
        "UNITY + ARD",
        "WIRELESS OK",
        "HI THERE",
        "BEEP BOOP",
        "ALL SYSTEMS",
        "READY",
        "HOWDY",
        "EYES ON YOU"
    };

    float _timer;

    void Start()
    {
        if (sender == null) sender = GetComponent<UDPSender>();
        if (sender == null) Debug.LogWarning("CyborgSpeechUDP: No UDPSender assigned.");
        _timer = intervalSeconds;
    }

    void Update()
    {
        if (sender == null) return;

        _timer -= Time.deltaTime;
        if (_timer > 0f) return;

        _timer = intervalSeconds;
        string msg = Greetings[Random.Range(0, Greetings.Length)];
        if (msg.Length > maxMessageLength)
            msg = msg.Substring(0, maxMessageLength);
        sender.sendMessage(msg);
        Debug.Log("Cyborg speech sent: " + msg);
    }
}
