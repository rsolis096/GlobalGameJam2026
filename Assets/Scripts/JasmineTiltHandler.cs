using System;
using UnityEngine;

public class JasmineTiltHandler : MonoBehaviour
{
    [Header("Lean")]
    [Min(0.001f)] public float maxLeanDeg = 50f;
    public float leanBuildRate = 80f;
    public float leanReturnRate = 40f;

    [NonSerialized] public Vector2 leanDeg;

    public float CurrentLean; 
    public Vector3 LeanWorldDir = Vector3.forward;

    [Range(0f, 1f)] public float failLean01 = 0.95f; 
    public bool IsFailed = false;

    void OnEnable()
    {
        leanDeg = Vector2.zero;
        CurrentLean = 0f;
        IsFailed = false;
        LeanWorldDir = Vector3.forward;
    }

    public void SetLean(Vector3 inputDirWorld, float dt)
    {
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

        leanDeg = Vector2.ClampMagnitude(leanDeg, maxLeanDeg);  

        Vector3 leanLocalDir = new Vector3(leanDeg.x, 0f, leanDeg.y);
        if (leanLocalDir.sqrMagnitude > 1e-6f)
            LeanWorldDir = transform.TransformDirection(leanLocalDir.normalized);

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
