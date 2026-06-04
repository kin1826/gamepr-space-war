using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonController : MonoBehaviour
{
    [Header("SceneButton")]
    public string sceneName;

    public void OnHomeClick()
    {
        CleanupPersistentState();
        OnResumeClick();

        if (FadeManager.Instance != null)
            FadeManager.Instance.LoadScene(sceneName);
        else
            SceneManager.LoadScene(sceneName);
    }

    public void OnReplayClick()
    {
        OnResumeClick();
        string current = SceneManager.GetActiveScene().name;
        CleanupPersistentState();

        if (FadeManager.Instance != null)
            FadeManager.Instance.LoadScene(current);
        else
            SceneManager.LoadScene(current);
    }

    private void CleanupPersistentState()
    {
        // Cursor.lockState = CursorLockMode.None;
        // Cursor.visible   = true;

        if (GameManager.Instance != null)
            Destroy(GameManager.Instance.gameObject);
    }

    public void OnResumeClick()
    {
        Manager.Instance?.TogglePause();
    }
}
