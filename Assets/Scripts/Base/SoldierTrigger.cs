using UnityEngine;

public class SoldierTrigger : MonoBehaviour
{
    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;

        if (other.CompareTag("Player"))
        {
            triggered = true;

            BaseSceneManager.Instance.OpenSoilderPanel();
        }
    }
}