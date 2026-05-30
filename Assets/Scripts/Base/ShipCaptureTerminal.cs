using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class ShipCaptureTerminal : MonoBehaviour
{
    [Header("Door Indicators")]
    public List<DoorIndicator> indicators = new List<DoorIndicator>();

    [Header("On Capture")]
    [Tooltip("Index panel story hiện sau khi chiếm")]
    public int panelIndex = 0;
    [Tooltip("Index camera lia tới khi chiếm")]
    public int camIndex   = 0;
    [Tooltip("Directional light bật khi chiếm thành công")]
    public Light captureLight;

    private bool _playerInRange  = false;
    private bool _captured       = false;
    private InputSystem_Actions  _input;

    void Awake()
    {
        _input = new InputSystem_Actions();
    }

    void OnEnable()
    {
        _input.Enable();
        _input.Player.Interact.performed += OnInteract;
    }

    void OnDisable()
    {
        _input.Player.Interact.performed -= OnInteract;
        _input.Disable();
    }

    void Update()
    {
        if (!_playerInRange || _captured) return;

        bool waveCleared = ControlRoomManager.Instance != null && ControlRoomManager.Instance.isWaveCleared;

        Manager.Instance.ShowHint(waveCleared
            ? "Nhấn [F] để chiếm lại con tàu"
            : "Tiêu diệt hết kẻ thù trước!");
    }

    void OnInteract(InputAction.CallbackContext ctx)
    {
        if (!_playerInRange || _captured) return;

        bool waveCleared = ControlRoomManager.Instance != null && ControlRoomManager.Instance.isWaveCleared;
        if (!waveCleared) return;

        _captured = true;

        foreach (var indicator in indicators)
            if (indicator) indicator.SetState(true);

        if (captureLight) captureLight.enabled = true;

        Manager.Instance.OpenSoilderPanel(panelIndex, camIndex);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            _playerInRange = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _playerInRange = false;
            Manager.Instance.ShowDefaultHint();
        }
    }
}
