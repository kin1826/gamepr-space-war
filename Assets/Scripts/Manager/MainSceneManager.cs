using UnityEngine;

public class MainSceneManager : MonoBehaviour
{
    public GameObject gameplayHUD;

    public GameObject storyCanvas;

    void Start()
    {
        OpenStory();
    }

    public void OpenStory()
    {
        storyCanvas.SetActive(true);

        gameplayHUD.SetActive(false);

        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ContinueGame()
    {
        storyCanvas.SetActive(false);

        gameplayHUD.SetActive(true);

        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}