using UnityEngine;
using UnityEngine.EventSystems;

public class Button_Award : MonoBehaviour, IPointerClickHandler
{
    [Header("Panels")]
    public GameObject lobbyPanel;
    public GameObject eventPanel;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (lobbyPanel != null)
            lobbyPanel.SetActive(false);

        if (eventPanel != null)
            eventPanel.SetActive(true);
    }
}