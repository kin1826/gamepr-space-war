using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

public class CameraSwitch : MonoBehaviour
{
    public CinemachineCamera camFP;
    public CinemachineCamera camTP;

    public bool isShipCamera; // 🔥 thêm cái này


    private PlayerInput input;
    private bool isFP = true;

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
        camFP.Priority = 20;
        camTP.Priority = 10;
    }

    void Switch(InputAction.CallbackContext ctx)
    {

        // 🔥 CHẶN theo state
        if (isShipCamera && GameManager.Instance.currentState != GameManager.GameState.Ship)
            return;

        if (!isShipCamera && GameManager.Instance.currentState != GameManager.GameState.Player)
            return;
            
        isFP = !isFP;

        camFP.Priority = isFP ? 20 : 10;
        camTP.Priority = isFP ? 10 : 20;
    }
}