using UnityEngine;
using UnityEngine.InputSystem;

public class FigureController : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 3f;
    public float runSpeed = 6f;
    public float gravity = -9.8f;
    public float jumpForce = 5f;

    [Header("Mouse Look")]
    public float mouseSensitivity = 2f;
    public Transform cameraTransform;

    private CharacterController controller;

    private Vector2 moveInput;
    private Vector2 lookInput;

    private float yVelocity;
    private bool isGrounded;

    private float yaw;
    private float pitch;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        yaw = transform.eulerAngles.y;
        pitch = 0f;
    }

    void Update()
    {
        // 🟢 GROUND
        isGrounded = controller.isGrounded;
        if (isGrounded && yVelocity < 0)
            yVelocity = -2f;

        // =========================
        // 🖱️ MOUSE LOOK
        // =========================
        yaw += lookInput.x * mouseSensitivity;
        pitch -= lookInput.y * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, -70f, 70f);

        // xoay player theo Y
        transform.rotation = Quaternion.Euler(0, yaw, 0);

        // xoay camera theo X
        cameraTransform.localRotation = Quaternion.Euler(pitch, 0, 0);

        // =========================
        // 🎮 MOVE (theo camera)
        // =========================
        Vector3 moveDir = transform.forward * moveInput.y + transform.right * moveInput.x;

        bool isRunning = moveInput.y > 0.9f;
        float speed = isRunning ? runSpeed : walkSpeed;

        controller.Move(moveDir * speed * Time.deltaTime);

        // =========================
        // 🌍 GRAVITY
        // =========================
        yVelocity += gravity * Time.deltaTime;
        controller.Move(Vector3.up * yVelocity * Time.deltaTime);
    }

    // 🎮 INPUT

    public void OnMove(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext ctx)
    {
        lookInput = ctx.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && isGrounded)
        {
            yVelocity = Mathf.Sqrt(jumpForce * -2f * gravity);
        }
    }
}