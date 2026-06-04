using UnityEngine;
using UnityEngine.EventSystems;

public class Button_Equipment : MonoBehaviour, IPointerClickHandler
{
    [Header("Panels")]
    public GameObject lobbyPanel;
    public GameObject equipmentPanel;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (lobbyPanel != null)
            lobbyPanel.SetActive(false);

        if (equipmentPanel != null)
            equipmentPanel.SetActive(true);
    }
}