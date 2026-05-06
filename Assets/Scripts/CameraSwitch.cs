using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

public class CameraSwitch : MonoBehaviour
{
    public CinemachineCamera camFP;
    public CinemachineCamera camTP;

    public bool isShipCamera; // 🔥 thêm cái này


    private PlayerInput input;
    private bool isFP = false;

    void Awake()
    {
        input = GetComponent<PlayerInput>();
    }

    void OnEnable()
    {
        input.actions["SwitchCamera"].performed += Switch;
    }

    void OnDisable()
    {
        input.actions["SwitchCamera"].performed -= Switch;
    }

    void Start()
    {
        camFP.Priority = 10;
        camTP.Priority = 20;
    }

    void Switch(InputAction.CallbackContext ctx)
    {

        // 🔥 CHẶN theo state
        if (isShipCamera && GameManager.Instance.currentState != GameManager.GameState.Ship)
            return;

        if (!isShipCamera && GameManager.Instance.currentState != GameManager.GameState.Player)
            return;
            
        isFP = !isFP;

        camFP.Priority = isFP ? 10 : 20;
        camTP.Priority = isFP ? 20 : 10;
    }
}