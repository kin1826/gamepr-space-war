using System;
using NUnit.Framework.Interfaces;
using UnityEngine;

public class MovementStateManager : MonoBehaviour
{
    #region 
    public float currentMoveSpeed;
    public float walkSpeed = 3 , walkBackSpeed = 2;
    public float runSpeed = 7, runBackSpeed = 5;
    public float crouchSpeed = 2, crouchBackSpeed = 1;
    public float airSpeed = 1.5f;
    [HideInInspector] public Vector3 dir;
    public float hzInput, vInput;
    CharacterController controller;
    #endregion 

    [SerializeField] float groundYOffset;
    [SerializeField] LayerMask groundMask;
    Vector3 spherePos; 

    [SerializeField] float gravity = -9.81f;
    [SerializeField] float jumpForce = 10;
    [HideInInspector] public bool jumped;
    Vector3 velocity;

    public MovementBaseState previousState;
    public MovementBaseState currentState;
    public IdleState Idle = new IdleState();
    public CrouchState Crouch = new CrouchState();
    public WalkState Walk = new WalkState();
    public RunState Run = new RunState();
    public JumpState Jump = new JumpState();

    [HideInInspector] public Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        SwitchState(Idle);
    }

    // Update is called once per frame
    void Update()
    {
        GetDirectionAndMove();
        Gravity();
        Falling();
        
        anim.SetFloat("hzInput", hzInput);
        anim.SetFloat("vInput", vInput);

        currentState.UpdateState(this);
    }

    public void SwitchState(MovementBaseState state)
    {
        currentState = state;
        currentState.EnterState(this);
    }

    void GetDirectionAndMove()
    {
        hzInput = Input.GetAxis("Horizontal");
        vInput = Input.GetAxis("Vertical");
        Vector3 airDir = Vector3.zero; 
        if(!IsGrounded()) airDir = transform.forward * vInput + transform.right * hzInput;
        else dir = transform.forward * vInput + transform.right * hzInput;

        controller.Move((dir.normalized * currentMoveSpeed + airDir.normalized * airSpeed) * Time.deltaTime);
    }

    public bool IsGrounded()
    {
        spherePos = new Vector3(transform.position.x, transform.position.y - groundYOffset, transform.position.z);
        if (Physics.CheckSphere(spherePos, controller.radius - 0.05f, groundMask)) return true;
        return false;
    }

    void Gravity()
    {
        if(!IsGrounded()) velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(spherePos, controller.radius - 0.05f);
    }

    void Falling() => anim.SetBool("Falling", !IsGrounded());
    
    public void JumpForce() => velocity.y += jumpForce;

    public void Jumped() => jumped = true;
}
