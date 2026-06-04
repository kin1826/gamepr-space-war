using UnityEngine;

[DefaultExecutionOrder(-1000)]
public class LobbyManager : MonoBehaviour
{
    public static LobbyManager Instance;

    [Header("Reward")]
    public GameObject      rewardPanel;
    public RewardPanelUI   rewardPanelUI;
    public SliderTextEffect goldRewardEffect;
    public SliderTextEffect expRewardEffect;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible   = true;
    }

    void Start()
    {
        if (rewardPanel) rewardPanel.SetActive(false);
        if (rewardPanelUI) rewardPanelUI.LoadDisplay();

        AudioManager.Instance.PlayTrack(0);

        if (PlayerPrefs.GetInt("ShowReward", 0) == 1)
        {
            PlayerPrefs.SetInt("ShowReward", 0);
            PlayerPrefs.Save();

            ShowReward();
        }
    }

    void ShowReward()
    {
        if (rewardPanelUI && SessionReward.Instance != null)
        {
            rewardPanelUI.earnedGold = SessionReward.Instance.totalGold;
            rewardPanelUI.earnedXP   = SessionReward.Instance.totalXP;
            SessionReward.Consume();
        }

        if (rewardPanel)      rewardPanel.GetComponent<UIPanelFader>()?.ShowPanel();
        if (rewardPanelUI)    rewardPanelUI.Preview();
        if (goldRewardEffect) goldRewardEffect.Play("+ " + rewardPanelUI.earnedGold + " Gold");
        if (expRewardEffect)  expRewardEffect.Play("+ "  + rewardPanelUI.earnedXP   + " XP");
    }

    [Header("SFX")]
    public string clickSFX = "Click";

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            AudioManager.Instance?.PlaySFX(clickSFX);
    }

    public void HideReward()
    {
        if (rewardPanel) rewardPanel.GetComponent<UIPanelFader>()?.HidePanel();
    }
}
