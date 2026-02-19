using UnityEngine;

/// <summary>
/// Simple mouse control for the Cyborg-Pawn using its ROOT transform only (no bones).
/// 1) Whole character rotates left/right with mouse X (faces the cursor).
/// 2) Optional: character slides left/right or follows cursor on a plane.
/// Place this on any GameObject (e.g. UDP); it finds "Cyborg-Pawn" at runtime.
/// </summary>
public class CyborgHeadLook : MonoBehaviour
{
    [Header("Cyborg (optional)")]
    [Tooltip("Assign Cyborg-Pawn root, or leave empty to find by name.")]
    public Transform cyborgRoot;

    [Header("Rotate with mouse X")]
    [Tooltip("Whole character turns left/right to face cursor. Mouse X = yaw in degrees (e.g. -180 to 180).")]
    public bool rotateWithMouseX = true;
    [Tooltip("Smoothing for rotation; 0 = instant.")]
    [Range(0f, 1f)]
    public float rotateSmooth = 0.1f;

    [Header("Move with mouse (optional)")]
    [Tooltip("If true, cyborg moves on XZ plane to follow cursor (raycast to plane at cyborg height).")]
    public bool moveWithMouse;
    [Tooltip("Smoothing for position; 0 = instant.")]
    [Range(0f, 1f)]
    public float moveSmooth = 0.15f;
    [Tooltip("Plane height for cursor raycast (use cyborg's Y if unsure).")]
    public float planeHeight = 0f;

    float _currentYaw;
    Vector3 _targetPosition;
    Camera _cam;

    void Start()
    {
        Debug.Log("CyborgHeadLook: Script started on '" + gameObject.name + "'.");
        _cam = Camera.main;
        if (_cam == null) Debug.LogWarning("CyborgHeadLook: No Main Camera.");

        if (cyborgRoot == null)
        {
            GameObject go = GameObject.Find("Cyborg-Pawn");
            if (go != null)
            {
                cyborgRoot = go.transform;
                Debug.Log("CyborgHeadLook: Found Cyborg-Pawn, controlling root.");
            }
            else
                Debug.LogWarning("CyborgHeadLook: No 'Cyborg-Pawn' in scene. Drag prefab into Hierarchy.");
        }
        if (cyborgRoot != null)
        {
            _currentYaw = cyborgRoot.eulerAngles.y;
            _targetPosition = cyborgRoot.position;
        }
    }

    void LateUpdate()
    {
        if (cyborgRoot == null) return;

        // Rotate: mouse X → whole character yaw (faces left/right)
        if (rotateWithMouseX)
        {
            float mouseX = Input.mousePosition.x;
            float normalized = Mathf.Clamp01(mouseX / (float)Screen.width);
            float targetYaw = (normalized - 0.5f) * 360f; // full 360 range
            if (rotateSmooth <= 0f)
                _currentYaw = targetYaw;
            else
                _currentYaw = Mathf.LerpAngle(_currentYaw, targetYaw, Time.deltaTime / rotateSmooth);
            cyborgRoot.rotation = Quaternion.Euler(0f, _currentYaw, 0f);
        }

        // Move: raycast mouse to plane, cyborg moves to that XZ
        if (moveWithMouse && _cam != null)
        {
            Ray ray = _cam.ScreenPointToRay(Input.mousePosition);
            Plane plane = new Plane(Vector3.up, new Vector3(0f, planeHeight, 0f));
            if (plane.Raycast(ray, out float enter) && enter > 0f)
                _targetPosition = ray.GetPoint(enter);
            Vector3 pos = cyborgRoot.position;
            pos.x = moveSmooth <= 0f ? _targetPosition.x : Mathf.Lerp(pos.x, _targetPosition.x, Time.deltaTime / moveSmooth);
            pos.z = moveSmooth <= 0f ? _targetPosition.z : Mathf.Lerp(pos.z, _targetPosition.z, Time.deltaTime / moveSmooth);
            pos.y = cyborgRoot.position.y; // keep current Y
            cyborgRoot.position = pos;
        }
    }
}
