using UnityEngine;
using System.Collections;

public class UIPanelFader : MonoBehaviour
{
    public CanvasGroup canvasGroup;

    public float fadeSpeed = 4f;

    Coroutine currentRoutine;

    void Awake()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();
    }

    public void ShowPanel()
    {
        gameObject.SetActive(true);

        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(Fade(0, 1));
    }

    public void HidePanel()
    {
        if (!gameObject.activeInHierarchy)
            return;

        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(Fade(1, 0));
    }

    IEnumerator Fade(float start, float end)
    {
        float t = 0;

        canvasGroup.alpha = start;

        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;

        while (t < 1)
        {
            t += Time.unscaledDeltaTime * fadeSpeed;

            canvasGroup.alpha =
                Mathf.Lerp(start, end, t);

            yield return null;
        }

        canvasGroup.alpha = end;

        // 🎯 nếu hide xong
        if (end == 0)
        {
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;

            gameObject.SetActive(false);
        }
    }
}