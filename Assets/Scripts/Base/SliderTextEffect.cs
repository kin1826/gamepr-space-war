using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SliderTextEffect : MonoBehaviour
{
    [Header("Slider")]
    public Slider   slider;
    public float    sliderDuration = 2f;

    [Header("Text")]
    public TMP_Text label;
    public float    zoomInScale  = 3f;
    public float    zoomDuration = 0.4f;

    public void Play()
    {
        gameObject.SetActive(true);
        StartCoroutine(RunEffect());
    }

    public void Play(string text)
    {
        if (label) label.text = text;
        Play();
    }

    private IEnumerator RunEffect()
    {
        // ── Chạy slider ──────────────────────────────────────────────
        if (slider)
        {
            slider.value = 0f;
            float t = 0f;
            while (t < 1f)
            {
                t           += Time.unscaledDeltaTime / sliderDuration;
                slider.value = Mathf.Lerp(0f, slider.maxValue, t);
                yield return null;
            }
            slider.value = slider.maxValue;
        }

        // ── Zoom text ─────────────────────────────────────────────────
        if (label)
        {
            label.gameObject.SetActive(true);
            yield return StartCoroutine(ZoomText());
        }
    }

    private IEnumerator ZoomText()
    {
        // Zoom in: small → big
        float t = 0f;
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / zoomDuration;
            float s = Mathf.Lerp(0f, zoomInScale, EaseOut(t));
            label.transform.localScale = Vector3.one * s;
            yield return null;
        }

        // Zoom out: big → normal
        t = 0f;
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / zoomDuration;
            float s = Mathf.Lerp(zoomInScale, 1f, EaseOut(t));
            label.transform.localScale = Vector3.one * s;
            yield return null;
        }

        label.transform.localScale = Vector3.one;
    }

    private float EaseOut(float t) => 1f - Mathf.Pow(1f - Mathf.Clamp01(t), 3f);
}
