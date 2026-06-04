using UnityEngine;
using UnityEngine.EventSystems;

public class Button_Exit : MonoBehaviour, IPointerClickHandler
{
    [Header("Panels")]
    public GameObject lobbyPanel;
    public GameObject eventPanel;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventPanel != null)
            eventPanel.SetActive(false);

        if (lobbyPanel != null)
            lobbyPanel.SetActive(true);
    }
}