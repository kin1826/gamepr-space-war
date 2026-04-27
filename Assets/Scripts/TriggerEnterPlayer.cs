using UnityEngine;

public class TriggerEnterPlayer : MonoBehaviour
{
    public GameManager gameManager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameManager.EnterPlayer();
        }
    }
}