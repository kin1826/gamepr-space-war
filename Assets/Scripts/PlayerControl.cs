using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour
{
    [Header("Components")]
    public CharacterController controller;
    public Animator anim;

    [Header("Camera")]
    public Transform cameraPivot;

    [Header("Movement")]
    public float walkSpeed = 2.5f;
    public float runSpeed = 6f;
    public float gravity = -19.62f;
    public float jumpHeight = 1.5f;

    [Header("Mouse Look")]
    public float mouseSensitivity = 0.05f;
    public float rotationSmooth = 8f;
    public float maxLookAngle = 70f;

    private InputSystem_Actions input;

    private Vector2 moveInput;
    private Vector2 lookInput;

    private Vector3 velocity;
    private bool isGrounded;

    private bool jumpPressed;
    private bool isRunning;
    private bool isAiming;
    private bool reloadPressed;

    // rotation
    private float yaw;
    private float pitch;

    private float currentYaw;
    private float currentPitch;

    void Awake()
    {
        if (controller == null)
            controller = GetComponent<CharacterController>();

        if (anim == null)
            anim = GetComponent<Animator>();

        input = new InputSystem_Actions();
    }

    void OnEnable()
    {
        input.Enable();

        // 🎮 MOVE
        input.Player.Move.performed += ctx =>
        {
            moveInput = ctx.ReadValue<Vector2>();
        };

        input.Player.Move.canceled += ctx =>
        {
            moveInput = Vector2.zero;
        };

        // 🎮 LOOK
        input.Player.Look.performed += ctx =>
        {
            lookInput = ctx.ReadValue<Vector2>();
        };

        input.Player.Look.canceled += ctx =>
        {
            lookInput = Vector2.zero;
        };

        // 🎮 JUMP
        input.Player.Jump.performed += ctx =>
        {
            jumpPressed = true;
        };

        // 🎮 AIM
        input.Player.Aim.performed += ctx =>
        {
            isAiming = true;
        };

        input.Player.Aim.canceled += ctx =>
        {
            isAiming = false;
        };

        // 🎮 RELOAD
        input.Player.Reload.performed += ctx =>
        {
            reloadPressed = true;
        };

        // 🎮 RUN
        input.Player.Run.performed += ctx =>
        {
            isRunning = true;
        };

        input.Player.Run.canceled += ctx =>
        {
            isRunning = false;
        };
    }

    void OnDisable()
    {
        input.Disable();
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        yaw = transform.eulerAngles.y;
        currentYaw = yaw;

        pitch = 0;
        currentPitch = 0;
    }

    void Update()
    {
        HandleMouseLook();
        HandleMovement();
        HandleGravity();
        HandleJump();
        HandleAnimator();
        HandleReload();
    }

    void HandleMouseLook()
    {
        // 🎯 target rotation
        yaw += lookInput.x * mouseSensitivity;
        pitch -= lookInput.y * mouseSensitivity;

        pitch = Mathf.Clamp(pitch, -maxLookAngle, maxLookAngle);

        // 🎯 smooth
        currentYaw = Mathf.Lerp(currentYaw, yaw, Time.deltaTime * rotationSmooth);
        currentPitch = Mathf.Lerp(currentPitch, pitch, Time.deltaTime * rotationSmooth);

        // 🎯 player xoay ngang
        transform.rotation = Quaternion.Euler(0f, currentYaw, 0f);

        // 🎯 camera xoay dọc
        cameraPivot.localRotation = Quaternion.Euler(currentPitch, 0f, 0f);
    }

    void HandleMovement()
    {
        float currentSpeed = isRunning ? runSpeed : walkSpeed;

        Vector3 move =
            transform.right * moveInput.x +
            transform.forward * moveInput.y;

        controller.Move(move * currentSpeed * Time.deltaTime);
    }

    void HandleGravity()
    {
        isGrounded = controller.isGrounded;

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }

    void HandleJump()
    {
        if (!jumpPressed) return;

        if (isGrounded)
        {
            anim.SetTrigger("JumpTrigger");

            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        jumpPressed = false;
    }

    void HandleAnimator()
    {
        anim.SetFloat("Horizontal", moveInput.x, 0.1f, Time.deltaTime);

        float animSpeed = moveInput.y;

        if (isRunning && moveInput.y > 0)
        {
            animSpeed *= 2f;
        }

        anim.SetFloat("Speed", animSpeed, 0.1f, Time.deltaTime);

        anim.SetBool("IsAiming", isAiming);
    }

    void HandleReload()
    {
        if (!reloadPressed) return;

        anim.ResetTrigger("ReloadTrigger");
        anim.SetTrigger("ReloadTrigger");

        reloadPressed = false;
    }
}