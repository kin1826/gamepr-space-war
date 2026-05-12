using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class FadeManager : MonoBehaviour
{
    public static FadeManager Instance;

    public Image fadeImage;

    public float fadeSpeed = 1f;

    public bool isFading;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        // 🎬 fade in khi bắt đầu
        StartCoroutine(Fade(1, 0));
        fadeImage.gameObject.SetActive(true);
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(FadeAndLoad(sceneName));
    }

    IEnumerator FadeAndLoad(string sceneName)
    {
        isFading = true;
        // 🎬 fade out
        yield return StartCoroutine(Fade(0, 1));

        // 🚀 load scene
        SceneManager.LoadScene(sceneName);

        // ⏳ đợi scene load
        yield return null;

        // 🎬 fade in
        yield return StartCoroutine(Fade(1, 0));

        isFading = false;
    }

    IEnumerator Fade(float start, float end)
    {
        float t = 0;

        Color c = fadeImage.color;

        while (t < 1)
        {
            t += Time.deltaTime * fadeSpeed;

            c.a = Mathf.Lerp(start, end, t);

            fadeImage.color = c;

            yield return null;
        }
    }
}