using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonEffect : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerDownHandler,
    IPointerUpHandler
{
    public Vector3 normalScale = Vector3.one;
    public Vector3 hoverScale = new Vector3(1.1f, 1.1f, 1.1f);
    public Vector3 pressedScale = new Vector3(0.9f, 0.9f, 0.9f);

    private bool isHovering;

    void Start()
    {
        normalScale = transform.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
        transform.localScale = hoverScale;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        transform.localScale = normalScale;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        transform.localScale = pressedScale;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        transform.localScale = isHovering ? hoverScale : normalScale;
    }
}