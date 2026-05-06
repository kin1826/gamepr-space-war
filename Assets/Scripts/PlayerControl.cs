using UnityEngine;
using UnityEngine.InputSystem; // Bắt buộc có dòng này để fix lỗi Input

public class PlayerControl : MonoBehaviour
{
    [Header("Components")]
    public CharacterController controller;
    public Animator anim;

    [Header("Movement Settings")]
    public float walkSpeed = 2.5f;
    public float runSpeed = 6.0f;
    public float gravity = -19.62f; // Tăng trọng lực để nhân vật bớt lơ lửng
    public float jumpHeight = 1.5f;

    private Vector3 velocity;
    private bool isGrounded;

    void Awake()
    {
        // Tự động tìm thành phần nếu bạn lỡ quên chưa kéo thả
        if (controller == null) controller = GetComponent<CharacterController>();
        if (anim == null) anim = GetComponent<Animator>();
    }

    void Update()
    {
        // 1. Kiểm tra chạm đất
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Ghì nhân vật xuống sàn để tránh lơ lửng
        }

        // 2. Lấy đầu vào di chuyển (Input System New)
        float horizontal = 0;
        float vertical = 0;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed) vertical = 1;
            if (Keyboard.current.sKey.isPressed) vertical = -1;
            if (Keyboard.current.aKey.isPressed) horizontal = -1;
            if (Keyboard.current.dKey.isPressed) horizontal = 1;
        }

        bool isRunning = Keyboard.current.leftShiftKey.isPressed;
        bool isAiming = Mouse.current.rightButton.isPressed;

        // 3. Tính toán tốc độ
        float currentSpeed = isRunning ? runSpeed : walkSpeed;
        Vector3 move = transform.right * horizontal + transform.forward * vertical;
        controller.Move(move * currentSpeed * Time.deltaTime);

        // 4. Xử lý Nhảy
        if (Keyboard.current.spaceKey.wasPressedThisFrame && isGrounded)
        {
            // Kích hoạt Trigger trong Animator (Phải khớp tên JumpTrigger)
            anim.SetTrigger("JumpTrigger");
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // 5. Áp dụng Trọng lực
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // 6. Cập nhật Animator
        // Dùng Float để điều khiển Blend Tree Locomotion
        anim.SetFloat("Horizontal", horizontal, 0.1f, Time.deltaTime);
        
        // Tính toán Speed cho Animator (0: Idle, 1: Walk, 2: Run)
        float animSpeed = vertical;
        if (isRunning && vertical > 0) animSpeed *= 2f; 
        anim.SetFloat("Speed", animSpeed, 0.1f, Time.deltaTime);

        // Cập nhật trạng thái Ngắm bắn
        anim.SetBool("IsAiming", isAiming);

        // Thêm vào trong hàm Update()
       if (Keyboard.current.rKey.wasPressedThisFrame)
        {  
    // Kích hoạt Trigger có tên là Reload trong Animator
          anim.ResetTrigger("ReloadTrigger");
          anim.SetTrigger("ReloadTrigger"); 
        }
    }
}