using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }

    [Header("References")]
    public Camera playerCamera;

    [Header("Movement")]
    public float walkSpeed = 6f;
    public float runSpeed = 12f;
    public float jumpPower = 7f;
    public float gravity = 20f;

    [Header("Crouch")]
    public float defaultHeight = 2f;
    public float crouchHeight = 1f;
    public float crouchSpeed = 3f;

    [Header("Look")]
    public float lookSpeed = 2f;
    public float lookXLimit = 45f;

    private CharacterController characterController;
    private Vector3 moveDirection;
    private float rotationX;

    private bool canMove = true;
    public bool CanMove => canMove;

    void Awake()
    {
        characterController = GetComponent<CharacterController>();

        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        DisableAllMinigameCameras();
        playerCamera.enabled = true;
    }

    private void DisableAllMinigameCameras()
    {
        Camera[] allCameras = FindObjectsByType<Camera>(FindObjectsSortMode.None);

        foreach (var cam in allCameras)
        {
            cam.enabled = false;
        }
    }

    void Update()
    {
        if (!canMove || Keyboard.current == null || Mouse.current == null)
            return;

        HandleMovement();
        HandleMouseLook();
    }

    public void SetMovement(bool val)
    {
        canMove = val;
    }

    public void SwitchTo(Camera newCamera)
    {
        if (playerCamera != null)
            playerCamera.enabled = false;

        playerCamera = newCamera;

        playerCamera.enabled = true;
    }

    private void HandleMovement()
    {
        bool isRunning = Keyboard.current.leftShiftKey.isPressed;
        bool isCrouching = Keyboard.current.rKey.isPressed;

        float speed = isRunning ? runSpeed : walkSpeed;

        if (isCrouching)
        {
            characterController.height = crouchHeight;
            speed = crouchSpeed;
        }
        else
        {
            characterController.height = defaultHeight;
        }

        Vector2 moveInput = Vector2.zero;

        if (Keyboard.current.wKey.isPressed) moveInput.y += 1;
        if (Keyboard.current.sKey.isPressed) moveInput.y -= 1;
        if (Keyboard.current.dKey.isPressed) moveInput.x += 1;
        if (Keyboard.current.aKey.isPressed) moveInput.x -= 1;

        moveInput = moveInput.normalized;

        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        float moveDirectionY = moveDirection.y;

        moveDirection = (forward * moveInput.y + right * moveInput.x) * speed;

        if (characterController.isGrounded)
        {
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                moveDirection.y = jumpPower;
            }
            else
            {
                moveDirection.y = -2f; // keeps grounded
            }
        }
        else
        {
            moveDirection.y = moveDirectionY - gravity * Time.deltaTime;
        }

        characterController.Move(moveDirection * Time.deltaTime);
    }

    private void HandleMouseLook()
    {
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        float mouseX = mouseDelta.x * lookSpeed * 0.1f;
        float mouseY = mouseDelta.y * lookSpeed * 0.1f;

        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);

        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
        transform.rotation *= Quaternion.Euler(0f, mouseX, 0f);
    }
}
