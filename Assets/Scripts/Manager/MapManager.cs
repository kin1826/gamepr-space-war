using UnityEngine;
using TMPro;
using System.Collections;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance;

    [Header("UI Panels")]
    public GameObject gamePlayHUD_Panel;

    public GameObject story_Panel;

    public GameObject soidler_Panel;

    [Header("Hint UI")]
    public TMP_Text hintText;

    public float blinkSpeed = 2f;

    private Coroutine blinkRoutine;

    public bool isFirstDoor = true;

    // [Header("Trigger")]

    private void Awake()
    {
        Instance = this;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        story_Panel.SetActive(false);
        gamePlayHUD_Panel.SetActive(false);
        soidler_Panel.SetActive(false);

        OpenStory();
    }

    public void OpenStory()
    {
        story_Panel.SetActive(true);
        ShowHint("Click [F] to continue...");

        gamePlayHUD_Panel.SetActive(false);

        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ContinueGame()
    {
        story_Panel.SetActive(false);
        soidler_Panel.SetActive(false);
        ShowHint("Use [WASD] to move, [Mouse] to look around, [Left Click] to shoot.");

        gamePlayHUD_Panel.SetActive(true);

        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void ShowHint(string text)
    {
        hintText.text = text;

        hintText.gameObject.SetActive(true);

        if (blinkRoutine != null)
            StopCoroutine(blinkRoutine);

        blinkRoutine = StartCoroutine(BlinkHint());
    }

    public void HideHint()
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

    public void OpenSoilderPanel()
    {
        soidler_Panel.SetActive(true);

        gamePlayHUD_Panel.SetActive(false);

        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = false;
    }

    public void CloseSoilderPanel()
    {
        soidler_Panel.SetActive(false);

        gamePlayHUD_Panel.SetActive(true);

        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
