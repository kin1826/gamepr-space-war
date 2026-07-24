using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Speed")]
    public float baseSpeed = 2f;
    public float maxSpeed = 100f;
    public float acceleration = 20f;
    public float speedSmooth = 7f;

    [Header("Rotation")]
    public float mouseSensitivity = 0.05f;
    public float rotationSmooth = 5f;

    [Header("Roll")]
    public float rollAngle = 25f;
    public float rollSmooth = 3f;
    public float rollReturnSpeed = 1.5f;

    [Header("Engine Light")]
    public Light leftEngineLight;
    public Light rightEngineLight;

    public float minLightIntensity = 300f;
    public float maxLightIntensity = 800f;

    [Header("Speed Control")]
    public float brakeSpeed = 15f;   // phanh mạnh
    public float idleDrag = 3f;      // thả tay giảm nhẹ

    [Header("Lift")]
    public float liftSpeed = 10f;

    private bool upPressed;
    private bool downPressed;

    private InputSystem_Actions input;

    private float currentSpeed;
    private float targetSpeed;

    private float yaw;
    private float pitch;

    private float currentYaw;
    private float currentPitch;

    private float roll;
    private float rollVelocity;

    private float throttleInput;
    private float rollInput;
    private Vector2 lookInput;

    private Rigidbody rb;

    private bool initialized = false;

    void Awake()
    {
        input = new InputSystem_Actions();
    }

    void OnEnable()
    {
        input.Enable();

        input.Player.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        input.Player.Look.canceled += ctx => lookInput = Vector2.zero;

        input.Player.Jump.performed += ctx => upPressed = true;
        input.Player.Jump.canceled += ctx => upPressed = false;

        input.Player.Crouch.performed += ctx => downPressed = true;
        input.Player.Crouch.canceled += ctx => downPressed = false;

        input.Player.Move.performed += ctx =>
        {
            Vector2 v = ctx.ReadValue<Vector2>();
            throttleInput = v.y;
            rollInput = v.x;
            Debug.Log("Enter, Current speed: " + currentSpeed);
        };

        input.Player.Move.canceled += ctx =>
        {
            throttleInput = 0;
            rollInput = 0;
            Debug.Log("Move input canceled. Forward/throttle: 0, Roll: 0");
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
        pitch = transform.eulerAngles.x;

        currentYaw = yaw;
        currentPitch = pitch;

        currentSpeed = baseSpeed;
        targetSpeed = baseSpeed;

        // 🔥 delay 1 frame tránh giật lúc start
        Invoke(nameof(EnableControl), 0.1f);

        rb = GetComponent<Rigidbody>();
    }

    void EnableControl()
    {
        initialized = true;
    }

    void Update()
    {
        if (!initialized || InventoryController.IsInventoryOpen) return;

        // 🎮 INPUT XOAY (chậm + mượt)
        yaw += lookInput.x * mouseSensitivity;
        pitch -= lookInput.y * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, -50f, 50f);

        // 🔥 smooth rotation (inertia)
        currentYaw = Mathf.Lerp(currentYaw, yaw, Time.deltaTime * rotationSmooth);
        currentPitch = Mathf.Lerp(currentPitch, pitch, Time.deltaTime * rotationSmooth);

        // 🎯 ROLL
        float targetRoll = -rollInput * rollAngle;

        if (rollInput == 0)
        {
            targetRoll = 0;
        }

        roll = Mathf.SmoothDamp(roll, targetRoll, ref rollVelocity, 1f / rollSmooth);

        transform.rotation = Quaternion.Euler(currentPitch, currentYaw, roll);
    }

    void FixedUpdate()
    {
        if (!initialized || InventoryController.IsInventoryOpen) return;

        // 🚀 THROTTLE (mượt)
        // 🚀 THROTTLE LOGIC
        if (throttleInput > 0) // W
        {
            targetSpeed += throttleInput * acceleration * Time.fixedDeltaTime;
        }
        else if (throttleInput < 0) // S (phanh mạnh)
        {
            targetSpeed = Mathf.Lerp(targetSpeed, baseSpeed, Time.fixedDeltaTime * brakeSpeed);
        }
        else // thả tay (giảm nhẹ hơn)
        {
            targetSpeed = Mathf.Lerp(targetSpeed, baseSpeed, Time.fixedDeltaTime * idleDrag);
        }

        // clamp
        targetSpeed = Mathf.Clamp(targetSpeed, baseSpeed, maxSpeed);

        // smooth
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.fixedDeltaTime * speedSmooth);

        // 👉 luôn bay về trước
        Vector3 velocity = transform.forward * currentSpeed;
        // 🚀 Lift lên/xuống
        if (upPressed)
        {
            velocity += transform.up * liftSpeed;
        }

        if (downPressed)
        {
            velocity -= transform.up * liftSpeed;
        }

        rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);

        // 🎯 Engine light theo tốc độ
        float speedPercent = currentSpeed / maxSpeed;

        float targetIntensity = Mathf.Lerp(minLightIntensity, maxLightIntensity, speedPercent);

        leftEngineLight.intensity = targetIntensity;
        rightEngineLight.intensity = targetIntensity;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            rb.linearVelocity = Vector3.zero;
        }
    }

    public float GetSpeedPercent()
    {
        return currentSpeed / maxSpeed;
    }
}
