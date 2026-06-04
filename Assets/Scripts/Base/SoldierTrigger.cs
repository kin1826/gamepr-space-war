using UnityEngine;

public class SoldierTrigger : MonoBehaviour
{
    [Tooltip("Index của soidlerPanel trong list MapManager.soidlerPanels")]
    public int panelIndex = 0;

    [Tooltip("Index của camera trong list MapManager.machineCams")]
    public int camIndex = 0;
    public bool isChangeMusic = false;
    public int musicTrackIndex = 0;

    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;

        if (other.CompareTag("Player"))
        {
            triggered = true;
            Manager.Instance.OpenSoilderPanel(panelIndex, camIndex);
            if (isChangeMusic)
            {
                AudioManager.Instance.SwitchTrack(musicTrackIndex); // Phát nhạc mới (giả sử clip thứ hai trong tracks là nhạc này)
            }
        }
    }
}
