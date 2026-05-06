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
            Debug.Log("Press F to CLOSE door");
        else
            Debug.Log("Press F to OPEN door");
    }

    void Interact(InputAction.CallbackContext ctx)
    {
        if (!playerInRange) return;

        Debug.Log("Interact");

        door.ToggleDoor();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}