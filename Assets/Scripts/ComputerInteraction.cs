using UnityEngine;
using UnityEngine.InputSystem;

public class ComputerInteraction : MonoBehaviour
{
    public DoorComController door;

    private bool playerInRange = false;

    void Update()
    {
        if (!playerInRange) return;

        if (door.IsOpen())
            Debug.Log("Press F to CLOSE door");
        else
            Debug.Log("Press F to OPEN door");
    }

    public void OnInteract(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        if (!playerInRange) return;

        Debug.Log("Click Interact button");

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