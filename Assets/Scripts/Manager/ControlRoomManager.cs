using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;

public class ControlRoomManager : Manager
{
    public static ControlRoomManager Instance;

    [Header("Cameras")]
    public List<CinemachineCamera> machineCams = new List<CinemachineCamera>();

    [Header("Camera Transition")]
    public float camTransitionDelay = 2f;

    [Header("UI Panels")]
    public GameObject gamePlayHUD_Panel;
    public GameObject storyPanel;
    public GameObject soidlerPanel;

    [Header("Ammo UI")]
    public TMP_Text clipSizeText;

    [Header("Health UI")]
    public Slider healthSlider;
    public TMP_Text healthText;

    [Header("Hint UI")]
    public TMP_Text hintText;
    public float blinkSpeed = 2f;

    public bool isFirstDoor   = true;
    public bool isWaveCleared = false;

    private Coroutine blinkRoutine;
    private int _activeCamIndex = -1;

    private bool isDoneStory = false;

    private void Awake()
    {
        base.Awake();
        Instance = this;
    }

    IEnumerator Start()
    {
        while (FadeManager.Instance.isFading)
            yield return null;

        storyPanel.SetActive(false);
        soidlerPanel.SetActive(false);
        foreach (var c in machineCams) if (c) c.enabled = false;

        gamePlayHUD_Panel.SetActive(false);

        HideHint();
        ShowStory(0);
    }

    // ── Story ──────────────────────────────────────────────────────
    public override void ShowStory(int index = 0)
    {
        isDoneStory = false;
        storyPanel.GetComponent<StoryDialogue>()?.LoadDialogueSet(index);
        storyPanel.GetComponent<UIPanelFader>().ShowPanel();
        ShowHint("Click [F] to continue...");

        gamePlayHUD_Panel.SetActive(false);

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible   = true;
    }

    public override void ContinueGame()
    {
    
        storyPanel.GetComponent<UIPanelFader>().HidePanel();
        soidlerPanel.GetComponent<UIPanelFader>().HidePanel();

        gamePlayHUD_Panel.SetActive(true);

        Time.timeScale = 1f;

        DisableAllCams();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible   = false;

        NextDefaultHint();

        if (!isDoneStory) 
        {
            OpenSoilderPanel(0, 1);
            isDoneStory = true;
        }
    }

    // ── Soldier Panel ──────────────────────────────────────────────
    public override void OpenSoilderPanel(int panelIndex = 0, int camIndex = 0)
    {
        StartCoroutine(OpenSoilderPanelRoutine(panelIndex, camIndex));
    }

    IEnumerator OpenSoilderPanelRoutine(int panelIndex, int camIndex)
    {
        DisableAllCams();
        if (camIndex >= 0 && camIndex < machineCams.Count && machineCams[camIndex])
        {
            _activeCamIndex = camIndex;
            machineCams[camIndex].enabled  = true;
            machineCams[camIndex].Priority = 100;
        }

        yield return new WaitForSecondsRealtime(camTransitionDelay);

        soidlerPanel.GetComponent<StoryDialogue>()?.LoadDialogueSet(panelIndex);
        soidlerPanel.GetComponent<UIPanelFader>().ShowPanel();

        ShowHint("Click [F] to continue...");
        gamePlayHUD_Panel.SetActive(false);

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible   = true;
    }

    public override void CloseSoilderPanel()
    {
        soidlerPanel.GetComponent<UIPanelFader>().HidePanel();

        gamePlayHUD_Panel.SetActive(true);

        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible   = false;

        ShowDefaultHint();
    }

    // ── Hint ───────────────────────────────────────────────────────
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

    // ── Ammo / Health ──────────────────────────────────────────────
    public override void OnAmmoChanged(int current, int extra)
    {
        if (clipSizeText) clipSizeText.text = current.ToString();
    }

    public override void OnPlayerHealthChanged(int current, int max)
    {
        if (healthSlider) { healthSlider.maxValue = max; healthSlider.value = current; }
        if (healthText)   healthText.text = current.ToString();
    }

    // ── Wave ───────────────────────────────────────────────────────
    public override void OnWaveCleared()
    {
        isWaveCleared = true;
        NextDefaultHint();
    }

    // ── Helper ─────────────────────────────────────────────────────
    void DisableAllCams()
    {
        foreach (var c in machineCams)
            if (c) c.enabled = false;
        _activeCamIndex = -1;
    }
}
