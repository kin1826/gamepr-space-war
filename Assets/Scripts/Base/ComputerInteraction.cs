using UnityEngine;
using UnityEngine.InputSystem;

public class ComputerInteraction : MonoBehaviour
{
    public DoorComController door;

    private bool playerInRange = false;

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
        if (!playerInRange) return;

        if (door.IsOpen())
            Manager.Instance.ShowHint("Press F to CLOSE door");
        else
            Manager.Instance.ShowHint("Press F to OPEN door");
    }

    void Interact(InputAction.CallbackContext ctx)
    {
        if (!playerInRange) return;

        Manager.Instance.ShowHint("Interact");

        door.ToggleDoor();
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
            Manager.Instance.HideHint();
        }
    }
}