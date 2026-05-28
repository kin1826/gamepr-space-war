using UnityEngine;
using UnityEngine.EventSystems;

public class Button_Award : MonoBehaviour, IPointerClickHandler
{
    [Header("Panels")]
    public GameObject lobbyPanel;
    public GameObject eventPanel;

    // CLICK BUTTON
    public void OnPointerClick(PointerEventData eventData)
    {
        // Tắt Lobby
        if (lobbyPanel != null)
        {
            lobbyPanel.SetActive(false);
        }

        // Bật Event
        if (eventPanel != null)
        {
            eventPanel.SetActive(true);
        }
    }
}