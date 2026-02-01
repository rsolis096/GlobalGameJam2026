using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class JasminePlayerController_FakePitch : MonoBehaviour
{
    [Header("Refs")]
    public TiltHandler tilt;
    public Transform PlayerRoot; // optional visual root (used only for moving position below)

    [Header("Fail")]
    public float failTiltAngleDeg = 40f;
    public bool isFailed;

    [Header("Movement")]
    public float maxSpeed = 7f;
    public float recoveryMultiplier = 5f;

    [Header("Recovery (simple)")]
    public float uprightAssist = 25f;
    public float angularDamping = 6f;
    [Range(0f, 1f)] public float inputAssist = 0.35f;
    public float maxAngVel = 15f;

    [Header("Yaw (A/D)")]
    public float yawSpeedDegPerSec = 180f;

    [Header("Fake Pitch (W/S)")]
    public float pitchAccelDegPerSec2 = 160f;     // slower = smaller
    public float pitchDecelDegPerSec2 = 240f;     // slower = smaller
    public float maxPitchSpeedDegPerSec = 45f;    // slower = smaller
    public float pitchTorquePerDegPerSec = 0.9f;  // how strongly pitchVel drives torque (slower = smaller)

    [Header("Pitch Limits")]
    public float maxForwardPitchDeg = 30f;
    public float maxBackwardPitchDeg = 20f;

    Rigidbody rb;
    Vector2 leanInput;

    float tiltAngle;
    float pitchVelDeg; // our “fake physics” angular velocity
    Vector3 actualLeanDir = Vector3.zero;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = true;

        if (PlayerRoot == null) PlayerRoot = transform;
    }

    void Update()
    {
        leanInput = Vector2.zero;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed) leanInput.y += 1;
            if (Keyboard.current.sKey.isPressed) leanInput.y -= 1;

            if (Keyboard.current.dKey.isPressed) leanInput.x += 1;
            if (Keyboard.current.aKey.isPressed) leanInput.x -= 1;
        }

        leanInput.x = Mathf.Clamp(leanInput.x, -1f, 1f);
        leanInput.y = Mathf.Clamp(leanInput.y, -1f, 1f);
    }

    void FixedUpdate()
    {
        if (isFailed) return;

        float dt = Time.fixedDeltaTime;

        // Fail check (original behavior)
        tiltAngle = Mathf.Lerp(tiltAngle, Vector3.Angle(transform.up, Vector3.up), dt);
        if (tiltAngle > failTiltAngleDeg)
        {
            isFailed = true;
            Debug.Log($"Failed! tiltAngle={tiltAngle:F2}");
            return;
        }

        // Optional safety clamp
        if (rb.angularVelocity.magnitude > maxAngVel)
            rb.angularVelocity = rb.angularVelocity.normalized * maxAngVel;

        // A/D: clean yaw (no spin-out), preserves current lean (x/z)
        ApplyYawMoveRotation(dt);

        // W/S: fake pitch “physics” (accel/decel), but applied as torque so gravity/falling still works
        UpdatePitchVelocity(dt);
        ApplyPitchTorqueFromVelocity();

        // For tilt handler + recovery + lean force: ONLY W/S intent, aligned to current facing
        Vector3 tiltInputDir = GetTiltWorldDirFromPitchInput();

        if (tilt != null)
            tilt.SetLean(tiltInputDir, dt);

        ApplyRecoveryTorque(tiltInputDir);
        ApplyLeanInput(tiltInputDir, dt);
    }

    // ---------- Rotation ----------

    void ApplyYawMoveRotation(float dt)
    {
        float yawInput = leanInput.x;
        if (Mathf.Abs(yawInput) < 0.0001f) return;

        // preserve current pitch/roll from physics, only change yaw
        Vector3 e = rb.rotation.eulerAngles;
        float newY = e.y + yawInput * yawSpeedDegPerSec * dt;

        rb.MoveRotation(Quaternion.Euler(e.x, newY, e.z));
    }

    void UpdatePitchVelocity(float dt)
    {
        float input = leanInput.y;

        if (Mathf.Abs(input) > 0.0001f)
        {
            pitchVelDeg += input * pitchAccelDegPerSec2 * dt;
            pitchVelDeg = Mathf.Clamp(pitchVelDeg, -maxPitchSpeedDegPerSec, maxPitchSpeedDegPerSec);
        }
        else
        {
            pitchVelDeg = Mathf.MoveTowards(pitchVelDeg, 0f, pitchDecelDegPerSec2 * dt);
        }

        // Stop pushing further if we’re already at limits (based on current physical pitch)
        float currentPitch = GetCurrentPitchDeg();

        if ((currentPitch >= maxForwardPitchDeg && pitchVelDeg > 0f) ||
            (currentPitch <= -maxBackwardPitchDeg && pitchVelDeg < 0f))
        {
            pitchVelDeg = 0f;
        }
    }

    void ApplyPitchTorqueFromVelocity()
    {
        if (Mathf.Abs(pitchVelDeg) < 0.0001f) return;

        // torque around local right -> pitches forward/back relative to facing
        Vector3 axis = transform.right;
        float torque = pitchVelDeg * pitchTorquePerDegPerSec;

        rb.AddTorque(axis * torque, ForceMode.Acceleration);
    }

    // Estimate current pitch by removing yaw, then reading local X angle
    float GetCurrentPitchDeg()
    {
        Vector3 fwd = Vector3.ProjectOnPlane(transform.forward, Vector3.up);
        if (fwd.sqrMagnitude < 1e-6f) return 0f;

        Quaternion yaw = Quaternion.LookRotation(fwd.normalized, Vector3.up);
        Quaternion localNoYaw = Quaternion.Inverse(yaw) * rb.rotation;

        Vector3 e = localNoYaw.eulerAngles;
        float x = e.x;
        if (x > 180f) x -= 360f; // signed [-180..180]
        return x;
    }

    Vector3 GetTiltWorldDirFromPitchInput()
    {
        if (Mathf.Abs(leanInput.y) < 0.0001f) return Vector3.zero;
        return transform.forward * leanInput.y; // W forward, S backward
    }

    // ---------- Original-ish stabilization + movement ----------

    void ApplyRecoveryTorque(Vector3 inputDir)
    {
        rb.AddTorque(-rb.angularVelocity * angularDamping, ForceMode.Acceleration);

        float tilt01 = Mathf.Clamp01(tiltAngle / failTiltAngleDeg);

        Vector3 desiredUp = Vector3.up;

        if (inputDir.sqrMagnitude > 1e-6f)
        {
            Vector3 dir = inputDir;
            dir.y = 0f;
            dir.Normalize();
            desiredUp = (Vector3.up + dir * inputAssist).normalized;
        }

        Vector3 axis = Vector3.Cross(transform.up, desiredUp);
        rb.AddTorque(axis * (uprightAssist * tilt01), ForceMode.Acceleration);
    }

    Vector3 leanDir = Vector3.zero;
    float leanSpeedMultiplier = 0f;

    void ApplyLeanInput(Vector3 inputDir, float dt)
    {
        if (tilt == null) return;

        Vector3 vel = rb.linearVelocity;
        vel.y = 0f;

        // No W/S => no lean acceleration
        if (inputDir.sqrMagnitude < 1e-6f)
        {
            rb.AddForce(-vel * recoveryMultiplier, ForceMode.Acceleration);
            return;
        }

        leanDir = tilt.LeanWorldDir.normalized;

        float alignment = Vector3.Dot(leanDir, inputDir.normalized);
        leanSpeedMultiplier = Mathf.Max(0f, alignment) * tilt.CurrentLean;

        rb.AddForce(inputDir.normalized * leanSpeedMultiplier, ForceMode.Acceleration);
        rb.AddForce(-vel * recoveryMultiplier, ForceMode.Acceleration);

        actualLeanDir = Vector3.ProjectOnPlane(Vector3.down, transform.up);
        actualLeanDir = Vector3.ProjectOnPlane(actualLeanDir, Vector3.up);
        if (actualLeanDir.sqrMagnitude > 1e-6f) actualLeanDir.Normalize();
        else actualLeanDir = Vector3.zero;

        float speed01 = Mathf.Clamp01(tiltAngle / failTiltAngleDeg);
        speed01 *= speed01;

        float rootMoveSpeed = maxSpeed * speed01;

        if (PlayerRoot != null && actualLeanDir != Vector3.zero)
            PlayerRoot.position += actualLeanDir * (rootMoveSpeed * dt);
    }
}
