using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class Loading_Manager : MonoBehaviour
{
    [Header("UI")]
    public GameObject loadingPanel;
    public Image fillBar;
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

        // Reset loading bar
        fillBar.fillAmount = 0;

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
        fillBar.fillAmount = 0;

        while (fillBar.fillAmount < 1f)
        {
            // Tăng loading
            fillBar.fillAmount += Time.deltaTime * loadingSpeed;

            // Giới hạn tối đa
            fillBar.fillAmount = Mathf.Clamp01(fillBar.fillAmount);

            // Đổi sang %
            int percent = Mathf.RoundToInt(fillBar.fillAmount * 100);

            // Update text
            loadingText.text = "Loading " + percent + "%";

            yield return null;
        }

        // Hoàn thành
        loadingText.text = "Complete";

        yield return new WaitForSeconds(0.5f);

        // Chuyển scene
        SceneManager.LoadScene(sceneName);
    }
}