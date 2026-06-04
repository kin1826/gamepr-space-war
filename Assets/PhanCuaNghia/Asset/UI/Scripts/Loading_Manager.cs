using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class Loading_Manager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private Image loadingFill;
    [SerializeField] private TextMeshProUGUI loadingText;

    [Header("Scene")]
    [SerializeField] private string sceneName = "MainScene";
    
    [Header("Loading Speed")]
    [SerializeField] private float loadingSpeed = 0.5f;

    private bool isLoading = false;

    private void Start()
    {
        if (loadingPanel != null)
        {
            loadingPanel.SetActive(false);
        }
        else
        {
            Debug.LogError("Loading Panel chưa được gán!");
        }

        if (loadingFill != null)
        {
            loadingFill.fillAmount = 0;
        }

        if (loadingText != null)
        {
            loadingText.text = "Loading 0%";
        }
        else
        {
            Debug.LogError("Loading Text chưa được gán!");
        }
    }

    public void StartLoading()
    {
        if (isLoading) return;

        isLoading = true;

        if (loadingPanel != null)
        {
            loadingPanel.SetActive(true);
        }

        StartCoroutine(LoadingCoroutine());
    }

    private IEnumerator LoadingCoroutine()
    {
        if (loadingFill != null)
        {
            loadingFill.fillAmount = 0;
        }

        float progress = 0f;

        while (progress < 1f)
        {
            progress += Time.deltaTime * loadingSpeed;
            progress = Mathf.Clamp01(progress);

            if (loadingFill != null)
            {
                loadingFill.fillAmount = progress;
            }

            int percent = Mathf.RoundToInt(progress * 100);

            if (loadingText != null)
            {
                loadingText.text = $"Loading {percent}%";
            }

            yield return null;
        }

        if (loadingText != null)
        {
            loadingText.text = "Complete";
        }

        yield return new WaitForSeconds(0.5f);

        // Nếu có FadeManager
        if (FadeManager.Instance != null)
        {
            FadeManager.Instance.LoadScene(sceneName);
        }
        else
        {
            Debug.LogWarning("Không tìm thấy FadeManager. Load Scene trực tiếp.");
            SceneManager.LoadScene(sceneName);
        }
    }
}