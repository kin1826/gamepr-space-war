using UnityEngine;
using TMPro;
using System.Collections;

public class MainSceneManager : BaseSceneManager
{
    public static MainSceneManager Instance;

    [Header("UI Panels")]
    public GameObject gamePlayHUD_Panel;

    public GameObject story_Panel;

    [Header("Hint UI")]
    public TMP_Text hintText;

    public float blinkSpeed = 2f;

    private Coroutine blinkRoutine;

    protected override void Awake()
    {
        base.Awake();
    }

    IEnumerator Start()
    {
        while (FadeManager.Instance.isFading)
        {
            yield return null;
        }

        ShowStory();
    }

    public override void ShowStory()
    {
        // story_Panel.SetActive(true);
        story_Panel.GetComponent<UIPanelFader>().ShowPanel();
        ShowHint("Click [F] to continue...");

        gamePlayHUD_Panel.SetActive(false);

        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public override void ContinueGame()
    {
        story_Panel.GetComponent<UIPanelFader>().HidePanel();
        ShowHint("Use [WASD] to move, [Mouse] to look around, [Left Click] to shoot.");

        gamePlayHUD_Panel.SetActive(true);

        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public override void ShowHint(string text)
    {
        hintText.text = text;

        hintText.gameObject.SetActive(true);

        if (blinkRoutine != null)
            StopCoroutine(blinkRoutine);

        blinkRoutine = StartCoroutine(BlinkHint());
    }

    public override void HideHint()
    {
        if (blinkRoutine != null)
            StopCoroutine(blinkRoutine);

        hintText.gameObject.SetActive(false);
    }

    IEnumerator BlinkHint()
    {
        while (true)
        {
            Color c = hintText.color;

            c.a = Mathf.PingPong(
                Time.unscaledTime * blinkSpeed,
                1f
            );

            hintText.color = c;

            yield return null;
        }
    }
}