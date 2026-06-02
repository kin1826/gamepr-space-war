using UnityEngine;

public class SoldierTrigger : MonoBehaviour
{
    [Tooltip("Index của soidlerPanel trong list MapManager.soidlerPanels")]
    public int panelIndex = 0;

    [Tooltip("Index của camera trong list MapManager.machineCams")]
    public int camIndex = 0;

    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;

        if (other.CompareTag("Player"))
        {
            triggered = true;
            Manager.Instance.OpenSoilderPanel(panelIndex, camIndex);
        }
    }
}
