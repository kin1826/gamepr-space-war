using UnityEngine;
using TMPro;
using System.Collections;

public class MainSceneManager : Manager
{
    public static MainSceneManager Instance;

    [Header("UI")]
    public GameObject storyPanel;

    [Header("Pause")]
    public GameObject pausePanel;

    [Header("Hint UI")]
    public TMP_Text hintText;
    public float blinkSpeed = 2f;

    private Coroutine blinkRoutine;

    protected override void Awake()
    {
        base.Awake();
        Instance = this;
    }

    IEnumerator Start()
    {
        while (FadeManager.Instance.isFading)
            yield return null;

        ShowStory(0);

        AudioManager.Instance.PlayTrack(0);
    }

    public override void ShowStory(int index = 0)
    {
        storyPanel.GetComponent<StoryDialogue>()?.LoadDialogueSet(index);
        storyPanel.GetComponent<UIPanelFader>().ShowPanel();
        ShowHint("Click [F] to continue...");

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible   = true;
    }

    public override void ContinueGame()
    {
        storyPanel.GetComponent<UIPanelFader>().HidePanel();
        ShowHint("Move to the mom ship");

        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible   = false;
    }

    public override void ShowHint(string text)
    {
        hintText.text = text;
        hintText.gameObject.SetActive(true);

        if (blinkRoutine != null) StopCoroutine(blinkRoutine);
        blinkRoutine = StartCoroutine(BlinkHint());
    }

    public override void HideHint()
    {
        if (blinkRoutine != null) StopCoroutine(blinkRoutine);
        hintText.gameObject.SetActive(false);
    }

    public override void OnPauseGame()
    {
        if (pausePanel) pausePanel.GetComponent<UIPanelFader>().ShowPanel();
    }

    public override void OnResumeGame()
    {
        if (pausePanel) pausePanel.GetComponent<UIPanelFader>().HidePanel();
    }

    IEnumerator BlinkHint()
    {
        while (true)
        {
            Color c = hintText.color;
            c.a = Mathf.PingPong(Time.unscaledTime * blinkSpeed, 1f);
            hintText.color = c;
            yield return null;
        }
    }
}
