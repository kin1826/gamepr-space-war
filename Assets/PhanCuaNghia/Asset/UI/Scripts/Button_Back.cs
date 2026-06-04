using UnityEngine;
using UnityEngine.EventSystems;

public class Button_Back : MonoBehaviour, IPointerClickHandler
{
    [Header("Panels")]
    public GameObject lobbyPanel;
    public GameObject equipmentPanel;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (equipmentPanel != null)
            equipmentPanel.SetActive(false);

        if (lobbyPanel != null)
            lobbyPanel.SetActive(true);
    }
}