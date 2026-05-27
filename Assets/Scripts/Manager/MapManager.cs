using UnityEngine;
using TMPro;
using System.Collections;
using Unity.Cinemachine;

public class MapManager : Manager
{
    public static MapManager Instance;

    [Header("Camera")]
    public CinemachineCamera machineCam;

    [Header("UI Panels")]
    public GameObject gamePlayHUD_Panel;

    public GameObject story_Panel;

    public GameObject soidler_Panel;

    [Header("Ammo UI")]
    public TMP_Text clipSizeText;

    [Header("Hint UI")]
    public TMP_Text hintText;

    public float blinkSpeed = 2f;

    private Coroutine blinkRoutine;

    public bool isFirstDoor = true;

    // [Header("Trigger")]

    private void Awake()
    {
        base.Awake();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    // void Start()
    // {
        
    // }

    IEnumerator Start()
    {
        while (FadeManager.Instance.isFading)
        {
            yield return null;
        }

        story_Panel.SetActive(false);
        gamePlayHUD_Panel.SetActive(false);
        soidler_Panel.SetActive(false);

        HideHint();

        ShowStory();
    }

    public override void ShowStory()
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
        soidler_Panel.GetComponent<UIPanelFader>().HidePanel();
        HideHint();
        
        gamePlayHUD_Panel.SetActive(true);

        Time.timeScale = 1f;

        if (machineCam) machineCam.enabled = false;

        Cursor.lockState = CursorLockMode.None;
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

    [Header("Camera Transition")]
    public float camTransitionDelay = 2f;

    public override void OpenSoilderPanel()
    {
        StartCoroutine(OpenSoilderPanelRoutine());
    }

    IEnumerator OpenSoilderPanelRoutine()
    {
        // 1. Chuyển cam trước
        if (machineCam)
        {
            machineCam.enabled = true;
            machineCam.Priority = 100;
        }

        // 2. Chờ cam blend xong
        yield return new WaitForSecondsRealtime(camTransitionDelay);

        // 3. Sau đó mới hiện panel
        soidler_Panel.GetComponent<UIPanelFader>().ShowPanel();
        ShowHint("Click [F] to continue...");

        gamePlayHUD_Panel.SetActive(false);

        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = true;
    }

    public override void CloseSoilderPanel()
    {
        soidler_Panel.GetComponent<UIPanelFader>().HidePanel();

        gamePlayHUD_Panel.SetActive(true);

        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = false;
    }

    public override void OnAmmoChanged(int current, int extra)
    {
        if (clipSizeText) clipSizeText.text = current.ToString();
    }
}
