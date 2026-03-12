using UnityEngine;

public class FollowTargetCamera : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Keep initial proximity")]
    [Tooltip("If enabled, the script captures the camera's initial offset from the target at runtime and preserves it.")]
    public bool keepInitialOffset = true;

    // Captured at runtime (target-space) so proximity stays consistent as the car moves/rotates.
    private Vector3 _capturedLocalPosOffset;
    private Quaternion _capturedLocalRotOffset;
    private bool _captured;

    [Header("Smoothing")]
    [Range(0f, 30f)] public float positionLerp = 12f;
    [Range(0f, 30f)] public float rotationLerp = 12f;

    void Start()
    {
        CaptureIfNeeded();
    }

    void LateUpdate()
    {
        if (target == null) return;

        CaptureIfNeeded();

        // Keep the same proximity (offset) that existed at the moment play started.
        Vector3 desiredPos = target.TransformPoint(_capturedLocalPosOffset);
        Quaternion desiredRot = target.rotation * _capturedLocalRotOffset;

        float pt = positionLerp <= 0f ? 1f : 1f - Mathf.Exp(-positionLerp * Time.deltaTime);
        float rt = rotationLerp <= 0f ? 1f : 1f - Mathf.Exp(-rotationLerp * Time.deltaTime);

        transform.position = Vector3.Lerp(transform.position, desiredPos, pt);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRot, rt);
    }

    private void CaptureIfNeeded()
    {
        if (!keepInitialOffset) return;
        if (_captured) return;
        if (target == null) return;

        // Capture offset in target-local space based on the scene layout *right before Play*.
        _capturedLocalPosOffset = target.InverseTransformPoint(transform.position);
        _capturedLocalRotOffset = Quaternion.Inverse(target.rotation) * transform.rotation;
        _captured = true;
    }
}
