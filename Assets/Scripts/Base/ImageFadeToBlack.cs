using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Gắn lên Image UI. Gọi FadeIn() để tối dần, FadeOut() để sáng lại.
/// </summary>
public class ImageFadeToBlack : MonoBehaviour
{
    public Image targetImage;
    public float fadeDuration = 1f;

    void Awake()
    {
        if (targetImage == null)
            targetImage = GetComponent<Image>();
    }

    public void FadeIn()  => StartCoroutine(Fade(0f, 1f));
    public void FadeOut() => StartCoroutine(Fade(1f, 0f));

    IEnumerator Fade(float from, float to)
    {
        float elapsed = 0f;
        Color c = targetImage.color;
        c.a = from;
        targetImage.color = c;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            c.a = Mathf.Lerp(from, to, elapsed / fadeDuration);
            targetImage.color = c;
            yield return null;
        }

        c.a = to;
        targetImage.color = c;
    }
}
