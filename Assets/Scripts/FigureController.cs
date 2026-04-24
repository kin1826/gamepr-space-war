using UnityEngine;
using UnityEngine.InputSystem;

public class FigureController : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 3f;
    public float runSpeed = 6f;
    public float gravity = -9.8f;
    public float jumpForce = 5f;

    [Header("Camera")]
    public Transform cameraTransform;

    [Header("Animator")]
    public Animator animator;

    private CharacterController controller;

    private Vector2 moveInput;
    private float yVelocity;
    private bool isGrounded;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // 🟢 check ground
        isGrounded = controller.isGrounded;

        if (isGrounded && yVelocity < 0)
            yVelocity = -2f;

        // 🟢 lấy hướng theo camera
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        forward.y = 0;
        right.y = 0;

        forward.Normalize();
        right.Normalize();

        Vector3 move = forward * moveInput.y + right * moveInput.x;

        // 🟢 chạy hay đi
        bool isRunning = moveInput.y > 0.9f;

        float speed = isRunning ? runSpeed : walkSpeed;

        controller.Move(move * speed * Time.deltaTime);

        // 🟢 gravity
        yVelocity += gravity * Time.deltaTime;
        controller.Move(Vector3.up * yVelocity * Time.deltaTime);

        // 🟢 xoay theo hướng di chuyển
        if (move.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }

        // 🎬 Animator
        animator.SetBool("isRun", isRunning && move.magnitude > 0.1f);
    }

    // 🎮 INPUT SYSTEM

    public void OnMove(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && isGrounded)
        {
            yVelocity = Mathf.Sqrt(jumpForce * -2f * gravity);
            animator.SetTrigger("isJump");
        }
    }

    public void OnAttack(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            animator.SetTrigger("isAttack");
        }
    }
}