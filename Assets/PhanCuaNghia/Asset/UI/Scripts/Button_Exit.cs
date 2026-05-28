using UnityEngine;
using UnityEngine.EventSystems;

public class Button_Exit : MonoBehaviour, IPointerClickHandler
{
    [Header("Panels")]
    public GameObject lobbyPanel;
    public GameObject eventPanel;

    // CLICK BUTTON
    public void OnPointerClick(PointerEventData eventData)
    {
        // Tắt event
        if (eventPanel != null)
        {
            eventPanel.SetActive(false);
        }

        // Bật lobby
        if (lobbyPanel != null)
        {
            lobbyPanel.SetActive(true);
        }
    }
}