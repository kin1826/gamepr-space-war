using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ControlRoomManager : Manager
{
    public static ControlRoomManager Instance;

    [Header("UI Panels")]
    public GameObject gamePlayHUD_Panel;
    public GameObject story_Panel;

    [Header("Ammo UI")]
    public TMP_Text clipSizeText;

    [Header("Health UI")]
    public Slider healthSlider;
    public TMP_Text healthText;

    [Header("Hint UI")]
    public TMP_Text hintText;
    public float blinkSpeed = 2f;

    private Coroutine blinkRoutine;

    private void Awake()
    {
        base.Awake();
    }

    IEnumerator Start()
    {
        while (FadeManager.Instance.isFading)
            yield return null;

        story_Panel.SetActive(false);
        gamePlayHUD_Panel.SetActive(false);

        HideHint();
        ShowStory();
    }

    public override void ShowStory(int index = 0)
    {
        story_Panel.GetComponent<UIPanelFader>().ShowPanel();
        ShowHint("Click [F] to continue...");

        gamePlayHUD_Panel.SetActive(false);

        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = true;
    }

    public override void ContinueGame()
    {
        story_Panel.GetComponent<UIPanelFader>().HidePanel();
        InitDefaultHint();

        gamePlayHUD_Panel.SetActive(true);

        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = false;
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

    public override void OnAmmoChanged(int current, int extra)
    {
        if (clipSizeText) clipSizeText.text = current.ToString();
    }

    public override void OnPlayerHealthChanged(int current, int max)
    {
        if (healthSlider)
        {
            healthSlider.maxValue = max;
            healthSlider.value    = current;
        }
        if (healthText) healthText.text = current.ToString();
    }
}
