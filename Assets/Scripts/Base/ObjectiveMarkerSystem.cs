using UnityEngine;
using TMPro;

[System.Serializable]
public class Objective
{
    public string objectiveName;

    public Transform target;
}

public class ObjectiveMarkerSystem : MonoBehaviour
{
    [Header("Objectives")]
    public Objective[] objectives;

    private int currentObjectiveIndex = 0;

    [Header("Player")]
    public Transform player;

    [Header("On Screen")]
    public RectTransform onScreenMarker;

    public TMP_Text onScreenName;

    public TMP_Text onScreenDistance;

    public RectTransform onScreenIcon;

    [Header("Off Screen")]
    public RectTransform offScreenMarker;

    public TMP_Text offScreenDistance;

    public RectTransform offScreenArrow;

    [Header("Settings")]
    public Vector3 worldOffset =
        new Vector3(0, 10, 0);

    public float screenBorder = 100f;

    public float centerRadius = 250f;

    private Camera cam;

    private Objective CurrentObjective
    {
        get
        {
            if (
                objectives == null ||
                objectives.Length == 0
            )
            {
                return null;
            }

            return objectives[currentObjectiveIndex];
        }
    }

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        if (
            CurrentObjective == null ||
            CurrentObjective.target == null ||
            player == null
        )
        {
            return;
        }

        // 🚀 distance
        float distance =
            Vector3.Distance(
                player.position,
                CurrentObjective.target.position
            );

        // 🚀 world -> screen
        Vector3 screenPos =
            cam.WorldToScreenPoint(
                CurrentObjective.target.position +
                worldOffset
            );

        // 🚀 target visible?
        bool isVisible =
            screenPos.z > 0 &&
            screenPos.x > 0 &&
            screenPos.x < Screen.width &&
            screenPos.y > 0 &&
            screenPos.y < Screen.height;

        // 🚀 distance text
        string distanceString;

        if (distance >= 1000f)
        {
            distanceString =
                (distance / 1000f).ToString("F1")
                + " km";
        }
        else
        {
            distanceString =
                Mathf.Round(distance) + " m";
        }

        // =====================================================
        // 🟢 ONSCREEN
        // =====================================================

        if (isVisible)
        {
            onScreenMarker.gameObject.SetActive(true);
            offScreenMarker.gameObject.SetActive(false);

            // 🚀 center screen
            Vector2 screenCenter =
                new Vector2(
                    Screen.width / 2f,
                    Screen.height / 2f
                );

            // 🚀 direction from center
            Vector2 dirFromCenter =
                ((Vector2)screenPos - screenCenter);

            // 🚀 tránh marker nằm giữa màn hình
            if (dirFromCenter.magnitude < centerRadius)
            {
                // 🚀 nếu đúng giữa màn hình
                if (dirFromCenter.magnitude < 0.01f)
                {
                    dirFromCenter =
                        new Vector2(0.5f, 1f);
                }

                dirFromCenter =
                    dirFromCenter.normalized *
                    centerRadius;

                screenPos =
                    screenCenter + dirFromCenter;
            }

            // 🚀 move marker
            onScreenMarker.position =
                screenPos;

            // 🚀 text
            onScreenName.text =
                CurrentObjective.objectiveName;

            onScreenDistance.text =
                distanceString;

            // 🚀 scale icon only
            float scale =
                Mathf.Clamp(
                    3000f / distance,
                    0.8f,
                    2f
                );

            onScreenIcon.localScale =
                Vector3.one * scale;
        }

        // =====================================================
        // 🔴 OFFSCREEN
        // =====================================================

        else
        {
            onScreenMarker.gameObject.SetActive(false);
            offScreenMarker.gameObject.SetActive(true);

            // 🚀 nếu target sau camera
            if (screenPos.z < 0)
            {
                screenPos *= -1;
            }

            // 🚀 center screen
            Vector3 screenCenter =
                new Vector3(
                    Screen.width / 2f,
                    Screen.height / 2f,
                    0
                );

            // 🚀 direction
            Vector3 direction =
                (screenPos - screenCenter)
                .normalized;

            // 🚀 edge position
            Vector3 edgePosition =
                screenCenter +
                direction *
                (Screen.height / 2f -
                screenBorder);

            edgePosition.x = Mathf.Clamp(
                edgePosition.x,
                screenBorder,
                Screen.width - screenBorder
            );

            edgePosition.y = Mathf.Clamp(
                edgePosition.y,
                screenBorder,
                Screen.height - screenBorder
            );

            // 🚀 move marker
            offScreenMarker.position =
                edgePosition;

            // 🚀 rotate arrow
            float angle =
                Mathf.Atan2(
                    direction.y,
                    direction.x
                ) * Mathf.Rad2Deg;

            offScreenArrow.rotation =
                Quaternion.Euler(
                    0,
                    0,
                    angle - 90f
                );

            // 🚀 text
            offScreenDistance.text =
                distanceString;
        }
    }

    // =====================================================
    // 🚀 NEXT OBJECTIVE
    // =====================================================

    public void NextObjective()
    {
        if (
            currentObjectiveIndex <
            objectives.Length - 1
        )
        {
            currentObjectiveIndex++;
        }
    }

    // =====================================================
    // 🚀 SET OBJECTIVE DIRECTLY
    // =====================================================

    public void SetObjective(int index)
    {
        if (
            index >= 0 &&
            index < objectives.Length
        )
        {
            currentObjectiveIndex = index;
        }
    }

    public void EndObjective()
    {
        gameObject.SetActive(false);
    }

    public void ContinueObjective()
    {
        gameObject.SetActive(true);
    }
}