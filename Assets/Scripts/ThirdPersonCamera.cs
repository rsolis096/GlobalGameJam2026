using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("Follow")]
    public Transform target;                 // bean or CameraTarget on bean
    public Vector3 offset = new Vector3(0f, 2.5f, -5f);
    public float followSmooth = 12f;
    public float rotateSmooth = 12f;

    [Header("Movement Reference")]
    public Transform movementReferenceYawOnly; // assign CameraPivot

    void LateUpdate()
    {
        if (!target) return;

        // --- yaw-only from target (ignore pitch/roll) ---
        Vector3 forward = target.forward;
        forward.y = 0f;
        if (forward.sqrMagnitude < 0.0001f) forward = Vector3.forward;
        forward.Normalize();

        Quaternion yaw = Quaternion.LookRotation(forward, Vector3.up);

        // --- position follow ---
        Vector3 desiredPos = target.position + yaw * offset;
        float posAlpha = 1f - Mathf.Exp(-followSmooth * Time.deltaTime);
        transform.position = Vector3.Lerp(transform.position, desiredPos, posAlpha);

        // --- look at target (world up) ---
        Vector3 lookDir = target.position - transform.position;
        if (lookDir.sqrMagnitude > 0.0001f)
        {
            Quaternion desiredRot = Quaternion.LookRotation(lookDir.normalized, Vector3.up);
            float rotAlpha = 1f - Mathf.Exp(-rotateSmooth * Time.deltaTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, desiredRot, rotAlpha);
        }

        // --- update movement reference (yaw only, stable) ---
        if (movementReferenceYawOnly)
        {
            Vector3 rigForward = transform.forward;
            rigForward.y = 0f;
            if (rigForward.sqrMagnitude < 0.0001f) rigForward = Vector3.forward;
            rigForward.Normalize();

            movementReferenceYawOnly.rotation = Quaternion.LookRotation(rigForward, Vector3.up);
            movementReferenceYawOnly.position = transform.position; // optional, doesn’t really matter
        }
    }
}
