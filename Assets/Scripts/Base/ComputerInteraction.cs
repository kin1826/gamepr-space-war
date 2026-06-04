using UnityEngine;
using UnityEngine.InputSystem;

public class ComputerInteraction : MonoBehaviour
{
    public DoorComController door;
    public MaterialChange materialChange;

    private bool playerInRange = false;
    private bool _lastWaveState;

    private InputSystem_Actions input;

    void Awake()
    {
        input = new InputSystem_Actions();
    }

    void OnEnable()
    {
        input.Enable();

        input.Player.Interact.performed += Interact;
    }

    void OnDisable()
    {
        input.Player.Interact.performed -= Interact;

        input.Disable();
    }

    void Update()
    {
        // Đổi material khi trạng thái wave thay đổi
        if (BaseManager.Instance != null)
        {
            bool current = BaseManager.Instance.isWaveCleared;
            if (current != _lastWaveState)
            {
                _lastWaveState = current;
                materialChange?.SetState(current ? 1 : 0);
            }
        }

        if (!playerInRange) return;

        bool waveCleared = BaseManager.Instance != null && BaseManager.Instance.isWaveCleared;

        if (waveCleared)
        {
            if (door.IsOpen())
                Manager.Instance.ShowHint("Press F to CLOSE door");
            else
                Manager.Instance.ShowHint("Press F to OPEN door");
        }
        else
        {
            Manager.Instance.ShowHint("Defeat all enemies to unlock the door");
        }

        
    }

    void Interact(InputAction.CallbackContext ctx)
    {
        if (!playerInRange) return;

        Manager.Instance.ShowHint("Interact");

        door.ToggleDoor();
        AudioManager.Instance.PlaySFX("DoorToggle"); // Phát âm thanh mở/đóng cửa (giả sử clip đầu tiên trong sfxClips là âm thanh này)

        Manager.Instance.SetDefaultHint(2); // Chuyển sang hint thứ 2 trong list mặc định (nếu có)
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            // Manager.Instance.ShowHint("Click F to open/close the door");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            
            Manager.Instance.ShowDefaultHint();
        }
    }
}