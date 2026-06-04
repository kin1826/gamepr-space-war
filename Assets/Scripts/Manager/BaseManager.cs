using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine.SceneManagement;

public class BaseManager : Manager
{
    public static BaseManager Instance;

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

    public bool isWaveCleared = false;

    [Header("Tutorial Hint Panel")]
    public GameObject hintPanel;

    [Header("Pause")]
    public GameObject pausePanel;

    [Header("Death / Respawn")]
    public int        deathCamIndex = 0;
    public GameObject deathPanel;
    public TMP_Text   respawningText;
    public float      respawnDelay  = 3f;

    private Coroutine blinkRoutine;
    private int _activeCamIndex = -1;

    private void Awake()
    {
        base.Awake();
        Instance = this;
    }

    IEnumerator Start()
    {
        while (FadeManager.Instance.isFading)
            yield return null;

        if (storyPanel)   storyPanel.SetActive(false);
        if (soidlerPanel) soidlerPanel.SetActive(false);
        if (pausePanel)   pausePanel.SetActive(false);
        if (deathPanel)   deathPanel.SetActive(false);
        if (hintPanel)    hintPanel.SetActive(false);
        foreach (var c in machineCams) if (c) c.enabled = false;

        gamePlayHUD_Panel.SetActive(true);

        InitDefaultHint();

        AudioManager.Instance.PlayTrack(0);
    }

    void Update()
    {
        if (hintPanel && hintPanel.activeSelf && UnityEngine.InputSystem.Keyboard.current.fKey.wasPressedThisFrame)
            hintPanel.GetComponent<UIPanelFader>()?.HidePanel();
    }

    // ── Story ──────────────────────────────────────────────────────
    public override void ShowStory(int index = 0)
    {
        if (!storyPanel) return;

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
        if (storyPanel)   storyPanel.GetComponent<UIPanelFader>().HidePanel();
        if (soidlerPanel) soidlerPanel.GetComponent<UIPanelFader>().HidePanel();

        gamePlayHUD_Panel.SetActive(true);

        Time.timeScale = 1f;

        DisableAllCams();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible   = false;

        InitDefaultHint();

        if (hintPanel) hintPanel.GetComponent<UIPanelFader>()?.ShowPanel();
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

        if (soidlerPanel)
        {
            soidlerPanel.GetComponent<StoryDialogue>()?.LoadDialogueSet(panelIndex);
            soidlerPanel.GetComponent<UIPanelFader>().ShowPanel();
        }

        ShowHint("Click [F] to continue...");
        gamePlayHUD_Panel.SetActive(false);

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible   = true;
    }

    public override void CloseSoilderPanel()
    {
        if (soidlerPanel) soidlerPanel.GetComponent<UIPanelFader>().HidePanel();

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

    // ── Pause ──────────────────────────────────────────────────────
    public override void OnPauseGame()
    {
        if (pausePanel) pausePanel.GetComponent<UIPanelFader>().ShowPanel();
    }

    public override void OnResumeGame()
    {
        if (pausePanel) pausePanel.GetComponent<UIPanelFader>().HidePanel();
    }

    // ── Wave ───────────────────────────────────────────────────────
    public override void OnWaveCleared()
    {
        if (isWaveCleared) return;
        isWaveCleared = true;
        NextDefaultHint();

        AudioManager.Instance.SwitchTrack(1);
    }

    // ── Helper ─────────────────────────────────────────────────────
    public override void OnPlayerDied()
    {
        StartCoroutine(DeathRoutine());
    }

    IEnumerator DeathRoutine()
    {
        DisableAllCams();
        if (deathCamIndex >= 0 && deathCamIndex < machineCams.Count && machineCams[deathCamIndex])
        {
            machineCams[deathCamIndex].enabled  = true;
            machineCams[deathCamIndex].Priority = 100;
        }

        yield return new WaitForSecondsRealtime(camTransitionDelay);

        if (deathPanel)
        {
            deathPanel.GetComponent<UIPanelFader>().ShowPanel();
            deathPanel.GetComponentInChildren<ImageFadeToBlack>()?.FadeIn();
        }
        gamePlayHUD_Panel.SetActive(false);
        HideHint();

        if (respawningText) respawningText.gameObject.SetActive(true);

        float elapsed = 0f;
        int   dots    = 0;
        while (elapsed < respawnDelay)
        {
            if (respawningText)
                respawningText.text = "Respawning" + new string('.', dots % 4);
            dots++;
            elapsed += 0.5f;
            yield return new WaitForSecondsRealtime(0.5f);
        }

        FadeManager.Instance.LoadScene(SceneManager.GetActiveScene().name);
    }

    void DisableAllCams()
    {
        foreach (var c in machineCams)
            if (c) c.enabled = false;
        _activeCamIndex = -1;
    }
}
