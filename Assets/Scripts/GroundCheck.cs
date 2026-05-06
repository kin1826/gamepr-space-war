using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    public PlayerControl player;

    private int groundCount = 0;

    private void OnTriggerEnter(Collider other)
    {

        Debug.Log("ENTER: " + other.name);
        if (other.CompareTag("Ground"))
        {
            groundCount++;
            player.SetGrounded(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            groundCount--;

            if (groundCount <= 0)
            {
                groundCount = 0;
                player.SetGrounded(false);
            }
        }
    }
}