using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

public class CameraSwitch : MonoBehaviour
{
    public CinemachineCamera camOutside;
    public CinemachineCamera camInside;

    private InputSystem_Actions inputActions;
    private bool isOutside = true;

    void Awake()
    {
        inputActions = new InputSystem_Actions();
    }

    void OnEnable()
    {
        inputActions.Enable();
        inputActions.Player.SwitchCamera.performed += OnSwitch;
    }

    void OnDisable()
    {
        inputActions.Player.SwitchCamera.performed -= OnSwitch;
        inputActions.Disable();
    }

    void Start()
    {
        camOutside.Priority = 10;
        camInside.Priority = 0;
    }

    void OnSwitch(InputAction.CallbackContext ctx)
    {
        if (isOutside)
        {
            camOutside.Priority = 0;
            camInside.Priority = 10;
        }
        else
        {
            camOutside.Priority = 10;
            camInside.Priority = 0;
        }

        isOutside = !isOutside;
    }
}