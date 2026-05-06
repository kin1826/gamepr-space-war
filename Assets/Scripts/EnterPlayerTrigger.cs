using UnityEngine;

public class EnterPlayerTrigger : MonoBehaviour
{
    public Transform spawnPoint; // chỗ player xuất hiện (optional)

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Ship")) return;

        GameManager.Instance.EnterPlayer(spawnPoint);
    }
}