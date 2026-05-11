using UnityEngine;
using UnityEngine.Serialization;
using TMPro;

public class ObjectiveMarkerUI : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    public Transform player;

    [Header("UI")]
    public TMP_Text nameText;

    public TMP_Text distanceText;

    public RectTransform iconTransform;

    [Header("Settings")]
    [FormerlySerializedAs("minScale")]
    public float minIconScale = 0.8f;

    [FormerlySerializedAs("maxScale")]
    public float maxIconScale = 2f;

    [Header("Offscreen")]
    public float screenBorder = 80f;

    private Camera cam;

    private Canvas canvas;

    private RectTransform canvasRect;

    private RectTransform rect;

    void Start()
    {
        cam = Camera.main;

        rect = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();

        if (canvas != null)
        {
            canvasRect = canvas.GetComponent<RectTransform>();
        }

        if (iconTransform == null)
        {
            Transform icon = transform.Find("TargetIcon");
            if (icon != null)
            {
                iconTransform = icon.GetComponent<RectTransform>();
            }
        }
    }

    void LateUpdate()
    {
        if (cam == null)
        {
            cam = Camera.main;
        }

        if (target == null || player == null || cam == null || rect == null)
            return;

        float distance = Vector3.Distance(player.position, target.position);

        Vector3 markerWorldPosition = target.position;
        Vector3 screenPos = cam.WorldToScreenPoint(markerWorldPosition);
        bool targetInFront = screenPos.z > 0f;

        if (targetInFront)
        {
            screenPos.x = Mathf.Clamp(screenPos.x, screenBorder, Screen.width - screenBorder);
            screenPos.y = Mathf.Clamp(screenPos.y, screenBorder, Screen.height - screenBorder);
        }
        else
        {
            screenPos = GetBehindCameraScreenPosition(markerWorldPosition);
        }

        SetMarkerPosition(screenPos);
        SetMarkerText(distance);
        SetIconScale(distance);
    }

    private Vector3 GetBehindCameraScreenPosition(Vector3 markerWorldPosition)
    {
        Vector3 direction = markerWorldPosition - cam.transform.position;
        Vector3 cameraSpaceDirection = cam.transform.InverseTransformDirection(direction);

        Vector2 edgeDirection = new Vector2(-cameraSpaceDirection.x, -cameraSpaceDirection.y);
        if (edgeDirection.sqrMagnitude < 0.001f)
        {
            edgeDirection = Vector2.down;
        }

        edgeDirection.Normalize();

        Vector2 screenCenter = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
        float maxX = screenCenter.x - screenBorder;
        float maxY = screenCenter.y - screenBorder;
        float scale = Mathf.Min(
            Mathf.Abs(maxX / edgeDirection.x),
            Mathf.Abs(maxY / edgeDirection.y)
        );

        if (float.IsInfinity(scale))
        {
            scale = edgeDirection.x == 0f ? maxY : maxX;
        }

        Vector2 screenPoint = screenCenter + edgeDirection * scale;
        return new Vector3(screenPoint.x, screenPoint.y, 0f);
    }

    private void SetMarkerPosition(Vector3 screenPos)
    {
        if (canvas == null || canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            rect.position = screenPos;
            return;
        }

        Camera canvasCamera = canvas.worldCamera;

        if (canvasCamera != null && RectTransformUtility.ScreenPointToWorldPointInRectangle(
                canvasRect,
                screenPos,
                canvasCamera,
                out Vector3 worldPoint))
        {
            rect.position = worldPoint;
        }
    }

    private void SetMarkerText(float distance)
    {
        if (distanceText != null)
        {
            if (distance >= 1000f)
            {
                distanceText.text = (distance / 1000f).ToString("F1") + " km";
            }
            else
            {
                distanceText.text = Mathf.Round(distance) + " m";
            }
        }

        if (nameText != null)
        {
            nameText.text = target.name;
        }
    }

    private void SetIconScale(float distance)
    {
        if (iconTransform == null)
            return;

        float safeDistance = Mathf.Max(distance, 1f);
        float iconScale = Mathf.Clamp(3000f / safeDistance, minIconScale, maxIconScale);

        iconTransform.localScale = Vector3.one * iconScale;
    }
}
