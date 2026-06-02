using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine.SceneManagement;

public class MapManager : Manager
{
    public static MapManager Instance;

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

    // ── Death / Respawn ────────────────────────────────────────────
    public override void OnPlayerDied()
    {
        StartCoroutine(DeathRoutine());
    }

    IEnumerator DeathRoutine()
    {
        // 1. Lia cam
        DisableAllCams();
        if (deathCamIndex >= 0 && deathCamIndex < machineCams.Count && machineCams[deathCamIndex])
        {
            machineCams[deathCamIndex].enabled  = true;
            machineCams[deathCamIndex].Priority = 100;
        }

        yield return new WaitForSecondsRealtime(camTransitionDelay);

        // 2. Hiện panel + ẩn HUD
        if (deathPanel)
        {
            deathPanel.GetComponent<UIPanelFader>().ShowPanel();
            deathPanel.GetComponentInChildren<ImageFadeToBlack>()?.FadeIn();
        }

        if (respawningText) respawningText.gameObject.SetActive(true);

        // 3. Animate "Respawning..."
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

        // 4. Reload scene
        FadeManager.Instance.LoadScene(SceneManager.GetActiveScene().name);
    }

    // ── Helper ─────────────────────────────────────────────────────
    void DisableAllCams()
    {
        foreach (var c in machineCams)
            if (c) c.enabled = false;
        _activeCamIndex = -1;
    }
}
