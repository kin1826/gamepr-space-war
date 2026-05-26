using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Cinemachine;

public class AimStateManager : MonoBehaviour
{
    [Header("States")]
    public AimBaseState currentState;
    public HipFireState Hip = new HipFireState();
    public AimState Aim = new AimState();

    [Header("Mouse Settings")]
    [SerializeField] float mouseSense = 1f;

    float xAxis;
    float yAxis;

    [Header("Camera")]
    [SerializeField] Transform camFollowPos;

    [HideInInspector] public Animator anim;

    public CinemachineCamera vCam;

    [Header("FOV")]
    public float adsFov = 20f;

    [HideInInspector] public float hipFov;
    [HideInInspector] public float currentFov;

    public float fovSmoothSpeed = 10f;

    [Header("Aim")]
    public Transform aimPos;

    [SerializeField] float aimSmoothSpeed = 20f;
    [SerializeField] LayerMask aimMask;

    [Header("Shoulder Swap")]
    float xFollowPos;
    float yFollowPos;
    float ogYPos;

    [SerializeField] float shoulderSwapSpeed = 10f;

    void Start()
    {
        if (camFollowPos != null)
        {
            xFollowPos = camFollowPos.localPosition.x;

            ogYPos = camFollowPos.localPosition.y;
            yFollowPos = ogYPos;

            xAxis = camFollowPos.eulerAngles.y;
            yAxis = camFollowPos.eulerAngles.x;
        }

        // Lấy Cinemachine Camera
        vCam = FindFirstObjectByType<CinemachineCamera>();

        if(vCam != null)
{
    hipFov = vCam.Lens.FieldOfView;
    currentFov = hipFov;
}
else
{
    Debug.LogError("VCam chưa được gán!");

}

        anim = GetComponent<Animator>();

        SwitchState(Hip);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        MouseInput();
        CameraFov();
        AimPosition();

        if (currentState != null)
        {
            currentState.UpdateState(this);
        }
    }

    private void LateUpdate()
    {
        CameraRotation();
        MoveCamera();
    }

    void MouseInput()
    {
        xAxis += Input.GetAxisRaw("Mouse X") * mouseSense;

        yAxis -= Input.GetAxisRaw("Mouse Y") * mouseSense;

        yAxis = Mathf.Clamp(yAxis, -80f, 80f);
    }

    void CameraRotation()
    {
        if (camFollowPos == null) return;

        // Xoay camera
        camFollowPos.rotation = Quaternion.Euler(yAxis, xAxis, 0f);

        // Xoay player theo chuột ngang
        if (Input.GetKey(KeyCode.Mouse1))
    {
        transform.rotation = Quaternion.Euler(0f, xAxis, 0f);
    }
    }

    void CameraFov()
    {
        if (vCam == null) return;

        LensSettings lens = vCam.Lens;

        lens.FieldOfView = Mathf.Lerp(
            lens.FieldOfView,
            currentFov,
            fovSmoothSpeed * Time.deltaTime
        );

        vCam.Lens = lens;
    }

    void AimPosition()
    {
        if (Camera.main == null) return;

        Vector2 screenCentre = new Vector2(Screen.width / 2f, Screen.height / 2f);

        Ray ray = Camera.main.ScreenPointToRay(screenCentre);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, aimMask))
        {
            if (aimPos != null)
            {
                aimPos.position = Vector3.Lerp(
                    aimPos.position,
                    hit.point,
                    aimSmoothSpeed * Time.deltaTime
                );
            }
        }
    }

    public void SwitchState(AimBaseState state)
    {
        currentState = state;
        currentState.EnterState(this);
    }

    void MoveCamera()
    {
        if (camFollowPos == null) return;

        // Shoulder Swap
        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            xFollowPos = -xFollowPos;
        }

        Vector3 targetPos = new Vector3(
            xFollowPos,
            yFollowPos,
            camFollowPos.localPosition.z
        );

        camFollowPos.localPosition = Vector3.Lerp(
            camFollowPos.localPosition,
            targetPos,
            shoulderSwapSpeed * Time.deltaTime
        );
    }
}