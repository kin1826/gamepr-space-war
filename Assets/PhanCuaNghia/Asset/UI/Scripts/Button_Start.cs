using UnityEngine;
using UnityEngine.EventSystems;

public class Button_Start : MonoBehaviour, IPointerClickHandler
{
    [Header("Loading Manager")]
    public Loading_Manager loadingManager;

    // CLICK BUTTON
    public void OnPointerClick(PointerEventData eventData)
    {
        loadingManager.StartLoading();
    }
}