using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    public DoorController door;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Ship") || other.CompareTag("Enemy"))
        {
            door.ToggleDoor();
        }
    }
}