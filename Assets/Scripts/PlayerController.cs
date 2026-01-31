using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Refs")]
    public TiltHandler tilt;


    [Header("Movement")]
    public float maxSpeed = 7f;
    public float moveForce = 35f;

    // Raise this to make it a bit easier to recover from tipping over
    float recoveryMultiplier = 10f; 

    Rigidbody rb;
    Vector2 moveInput;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        moveInput = Vector2.zero;
        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed) moveInput.y += 1;
            if (Keyboard.current.sKey.isPressed) moveInput.y -= 1;
            if (Keyboard.current.dKey.isPressed) moveInput.x += 1;
            if (Keyboard.current.aKey.isPressed) moveInput.x -= 1;
        }
        moveInput = moveInput.normalized;
    }

    void FixedUpdate()
    {
        float deltaTime = Time.fixedDeltaTime;

        Vector3 inputDir = new Vector3(moveInput.x, 0f, moveInput.y);
        if (inputDir.sqrMagnitude > 1f) inputDir.Normalize();

        if (tilt != null)
            tilt.SetLean(inputDir, deltaTime);

        ApplyInput(inputDir, deltaTime);

    }

    void ApplyInput(Vector3 inputDir, float deltaTime)
    {
        if (tilt == null) return;

        Vector3 vel = rb.linearVelocity;
        vel.y = 0f;

        // Alignment of 1 means leaning fully in the input direction (bad)
        Vector3 leanDir = tilt.LeanWorldDir.normalized;
        float alignment = Vector3.Dot(leanDir, inputDir);

        // Acceleration force should be faster the more we are leaning into the direction
        float leanSpeedMultiplier = Mathf.Max(0f, alignment) * tilt.CurrentLean;

        Vector3 force = inputDir * (moveForce * leanSpeedMultiplier);
        rb.AddForce(force, ForceMode.Acceleration);

        float speed = vel.magnitude;
        if (speed > maxSpeed)
        {
            Vector3 capped = vel.normalized * maxSpeed;
            rb.linearVelocity = new Vector3(capped.x, vel.y, capped.z);
        }

        rb.AddForce(-vel * recoveryMultiplier, ForceMode.Acceleration);
    }
}
