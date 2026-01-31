using System;
using UnityEngine;

public class TiltHandler : MonoBehaviour
{
    [Header("Lean")]
    [Min(0.001f)] public float maxLeanDeg = 25f;
    public float leanBuildRate = 80f;
    public float leanReturnRate = 40f;

    // Internal state: x = lean right/left, y = lean forward/back (in degrees)
    [NonSerialized] public Vector2 leanDeg;

    // Outputs (read-only from other scripts)
    public float CurrentLean { get; private set; }           // 0..1
    public Vector3 LeanWorldDir { get; private set; } = Vector3.forward;

    public void SetLean(Vector3 inputDirWorld, float dt)
    {
        // Convert input direction into local XZ "desired lean"
        Vector3 local = transform.InverseTransformDirection(inputDirWorld);
        Vector2 desired = new Vector2(local.x, local.z);

        Vector2 targetLean;

        if (desired.sqrMagnitude > 0.0001f)
        {
            desired.Normalize();
            targetLean = desired * maxLeanDeg;
            leanDeg = MoveToward(leanDeg, targetLean, leanBuildRate, dt);
        }
        else
        {
            targetLean = Vector2.zero;
            leanDeg = MoveToward(leanDeg, targetLean, leanReturnRate, dt);
        }

        // Direction of lean (world space)
        Vector3 leanLocalDir = new Vector3(leanDeg.x, 0f, leanDeg.y);
        if (leanLocalDir.sqrMagnitude < 0.0001f)
            leanLocalDir = Vector3.forward;

        LeanWorldDir = transform.TransformDirection(leanLocalDir.normalized);

        // Normalized amount
        CurrentLean = Mathf.Clamp01(leanDeg.magnitude / maxLeanDeg);
    }

    static Vector2 MoveToward(Vector2 current, Vector2 target, float rateDegPerSec, float dt)
    {
        Vector2 delta = target - current;
        float maxStep = rateDegPerSec * dt;

        float dist = delta.magnitude;
        if (dist <= maxStep || dist < 0.000001f)
            return target;

        return current + (delta / dist) * maxStep;
    }
}
