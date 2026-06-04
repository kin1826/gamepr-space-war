using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class Loading_Manager_Event : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private Image loadingFill;
    [SerializeField] private TextMeshProUGUI loadingText;

    [Header("Panels")]
    [SerializeField] private GameObject currentPanel;
    [SerializeField] private GameObject eventPanel;

    [Header("Loading Speed")]
    [SerializeField] private float loadingSpeed = 1f;

    private bool isLoading = false;

    private void Start()
    {
        loadingPanel.SetActive(false);

        if (loadingFill != null)
            loadingFill.fillAmount = 0;

        if (loadingText != null)
            loadingText.text = "Loading 0%";

        // Ẩn Event lúc đầu
        eventPanel.SetActive(false);
    }

    public void StartLoading()
    {
        if (isLoading) return;

        isLoading = true;

        loadingPanel.SetActive(true);

        StartCoroutine(LoadingCoroutine());
    }

    private IEnumerator LoadingCoroutine()
    {
        loadingFill.fillAmount = 0;

        float progress = 0f;

        while (progress < 1f)
        {
            progress += Time.deltaTime * loadingSpeed;
            progress = Mathf.Clamp01(progress);

            loadingFill.fillAmount = progress;

            int percent = Mathf.RoundToInt(progress * 100);

            loadingText.text = $"Loading {percent}%";

            yield return null;
        }

        loadingText.text = "Complete";

        yield return new WaitForSeconds(0.3f);

        // Chuyển panel
        if (currentPanel != null)
            currentPanel.SetActive(false);

        if (eventPanel != null)
            eventPanel.SetActive(true);

        loadingPanel.SetActive(false);

        isLoading = false;
    }
}