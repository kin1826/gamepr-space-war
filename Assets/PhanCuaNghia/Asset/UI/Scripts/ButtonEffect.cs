using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonEffect : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerDownHandler,
    IPointerUpHandler
{
    private Vector3 normalScale  = Vector3.one;
    private Vector3 hoverScale   = new Vector3(1.05f, 1.05f, 1.05f);
    private Vector3 pressedScale = new Vector3(0.95f, 0.95f, 0.95f);
    private float   lerpSpeed    = 16f;

    private Vector3 _targetScale;
    private bool    _isHovering;

    void Start()
    {
        normalScale  = transform.localScale;
        _targetScale = normalScale;
    }

    void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, _targetScale, Time.unscaledDeltaTime * lerpSpeed);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _isHovering  = true;
        _targetScale = hoverScale;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _isHovering  = false;
        _targetScale = normalScale;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _targetScale = pressedScale;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _targetScale = _isHovering ? hoverScale : normalScale;
    }
}
