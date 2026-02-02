using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.GraphicsBuffer;

public class PlayerController : MonoBehaviour
{
    [Header("Refs")]
    public TiltHandler tilt;
    public Transform Visuals;
    public GrabController grabController;

    [Header("Movement")]
    public float maxSpeed = 7f;
    public float tiltAngle;

    [Header("Fail")]
    public float failTiltAngleDeg = 40f;
    public bool isFailed;

    public float recoveryMultiplier = 5f;

    [Header("Recovery (simple)")]
    public float uprightAssist = 25f;
    public float angularDamping = 6f;
    [Range(0f, 1f)] public float inputAssist = 0.35f;
    public float maxAngVel = 15f;

    Rigidbody rb;
    Vector2 leanInput;

    public Transform PlayerRoot;

    public float maxRotation = 30f;
    public float currentRotation = 0f;

    Vector3 actualLeanDir = Vector3.zero;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        grabController = GetComponentInChildren<GrabController>();

    }

    void Update()
    {
        //Debug.Log("Tilt Angle: " + tiltAngle);
        leanInput = Vector2.zero;
        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed) leanInput.y += 1;
            if (Keyboard.current.sKey.isPressed) leanInput.y -= 1;
            if (Keyboard.current.dKey.isPressed) leanInput.x += 1;
            if (Keyboard.current.aKey.isPressed) leanInput.x -= 1;

        }
        leanInput = leanInput.normalized;
    }

    void FixedUpdate()
    {
        if (isFailed) return;

        float deltaTime = Time.fixedDeltaTime;

        tiltAngle = Mathf.Lerp(tiltAngle, Vector3.Angle(transform.up, Vector3.up), deltaTime);

        if (tiltAngle > failTiltAngleDeg)
        {
            isFailed = true;
            StartCoroutine(GameFailRoutine());
            return;
        }

        Vector3 leanInputDir = new Vector3(leanInput.x, 0f, leanInput.y);
        if (leanInputDir.sqrMagnitude > 1f) leanInputDir.Normalize();

        if (tilt != null)
            tilt.SetLean(leanInputDir, deltaTime);

        ApplyRecoveryTorque(leanInputDir);

        ApplyLeanInput(leanInputDir, deltaTime);
    }

    public IEnumerator GameFailRoutine() {
        Debug.Log($"Failed! tiltAngle={tiltAngle}");

        UIOverlayController.Instance.FadeText.text = "You Fail!";
        yield return UIOverlayController.Instance.StartCoroutine(UIOverlayController.Instance.FadeRoutine(5f, fadeOut: false));
        
        Level.LevelInstance.ReloadLevel();

        Color imageColor = UIOverlayController.Instance.FadeImage.color;
        imageColor.a = 0f;
        UIOverlayController.Instance.FadeImage.color = imageColor;

        Color textColor = UIOverlayController.Instance.FadeText.color;
        textColor.a = 0f;
        UIOverlayController.Instance.FadeText.color = textColor;

        Debug.Log("Level Reloaded");
    }

    void ApplyRotation()
    {
        if (actualLeanDir.sqrMagnitude < 1e-6f) return;

        Quaternion targetYaw = Quaternion.LookRotation(actualLeanDir, Vector3.up);

        Vector3 currentEuler = rb.rotation.eulerAngles;
        Vector3 targetEuler = targetYaw.eulerAngles;

        rb.MoveRotation(Quaternion.Euler(currentEuler.x, targetEuler.y, currentEuler.z));
    }

    void ApplyRecoveryTorque(Vector3 inputDir)
    {
        rb.AddTorque(-rb.angularVelocity * angularDamping, ForceMode.Acceleration);

        float tilt01 = Mathf.Clamp01(tiltAngle / failTiltAngleDeg);

        Vector3 desiredUp = Vector3.up;

        if (inputDir.sqrMagnitude > 1e-6f)
        {
            inputDir.y = 0f;
            inputDir.Normalize();
            desiredUp = (Vector3.up + inputDir * inputAssist).normalized;

            Vector3 currentUp = transform.up;
            Vector3 axis = Vector3.Cross(currentUp, desiredUp);

            rb.AddTorque(axis * (uprightAssist * tilt01), ForceMode.Acceleration);

        }


    }

    Vector3 leanDir = Vector3.zero;
    float leanSpeedMultiplier = 0f;

    void ApplyLeanInput(Vector3 inputDir, float deltaTime)
    {
        if (tilt == null) return;

        Vector3 vel = rb.linearVelocity;
        vel.y = 0f;

        leanDir = tilt.LeanWorldDir.normalized;
        //Debug.DrawLine(transform.position, transform.position + leanDir * 10f, Color.red);

        float alignment = Vector3.Dot(leanDir, inputDir);
        leanSpeedMultiplier = Mathf.Max(0f, alignment) * tilt.CurrentLean;

        Vector3 force = inputDir * leanSpeedMultiplier;
        rb.AddForce(force, ForceMode.Acceleration);

        rb.AddForce(-vel * recoveryMultiplier, ForceMode.Acceleration);

        actualLeanDir = Vector3.ProjectOnPlane(Vector3.down, transform.up);
        actualLeanDir = Vector3.ProjectOnPlane(actualLeanDir, Vector3.up);

        if (actualLeanDir.sqrMagnitude > 1e-6f)
            actualLeanDir.Normalize();
        else
            actualLeanDir = Vector3.zero;

        //Debug.DrawLine(transform.position, transform.position + actualLeanDir * 6f, Color.cyan);

        float speed01 = Mathf.Clamp01(tiltAngle / failTiltAngleDeg);
        speed01 *= speed01;

        float rootMoveSpeed = maxSpeed * speed01;
        if (PlayerRoot != null && actualLeanDir != Vector3.zero)
        {
            PlayerRoot.position += actualLeanDir * (rootMoveSpeed * deltaTime);
        }

        if (Visuals)
        {
            if (actualLeanDir.magnitude > 1e-6f)
            {
                actualLeanDir.y = 0;
                Quaternion rotation = Quaternion.LookRotation(actualLeanDir);
                Visuals.transform.localRotation = Quaternion.Slerp(Visuals.transform.localRotation, rotation, deltaTime * 2f);
            }
        }
    }

}
