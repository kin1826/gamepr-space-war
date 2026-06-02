using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class Loading_Manager : MonoBehaviour
{
    [Header("UI")]
    public GameObject loadingPanel;
    public Slider loadingSlider;
    public TextMeshProUGUI loadingText;

    [Header("Scene")]
    public string sceneName = "MainScene";

    [Header("Loading Speed")]
    public float loadingSpeed = 0.5f;

    private bool isLoading = false;

    void Start()
    {
        // Ẩn panel lúc đầu
        loadingPanel.SetActive(false);

        if (loadingSlider) loadingSlider.value = 0;

        // Reset text
        loadingText.text = "Loading 0%";
    }

    public void StartLoading()
    {
        // Tránh spam click
        if (isLoading) return;

        isLoading = true;

        // Bật panel trước
        loadingPanel.SetActive(true);

        // Chạy coroutine
        StartCoroutine(LoadingCoroutine());
    }

    IEnumerator LoadingCoroutine()
    {
        if (loadingSlider) loadingSlider.value = 0;

        float progress = 0f;

        while (progress < 1f)
        {
            progress += Time.deltaTime * loadingSpeed;
            progress  = Mathf.Clamp01(progress);

            if (loadingSlider) loadingSlider.value = progress;

            int percent = Mathf.RoundToInt(progress * 100);

            // Update text
            loadingText.text = "Loading " + percent + "%";

            yield return null;
        }

        // Hoàn thành
        loadingText.text = "Complete";

        yield return new WaitForSeconds(0.5f);

        // Chuyển scene
        FadeManager.Instance.LoadScene(sceneName);
        // SceneManager.LoadScene(sceneName);
    }
}