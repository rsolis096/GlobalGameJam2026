using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("Follow")]
    public Transform target;
    public Vector3 offset = new Vector3(0f, 2.5f, -5f);
    public float followSmooth = 12f;

    void LateUpdate()
    {
        if (!target) return;

        // Compute desired position
        Vector3 desiredPos = target.position + offset;

        // Smoothly interpolate to the desired position
        float posAlpha = 1f - Mathf.Exp(-followSmooth * Time.deltaTime);
        transform.position = Vector3.Lerp(transform.position, desiredPos, posAlpha);

        // Keep rotation fixed (optional: identity)
        // Comment out the next line if you want to set rotation in inspector
        transform.rotation = Quaternion.identity;
    }
}