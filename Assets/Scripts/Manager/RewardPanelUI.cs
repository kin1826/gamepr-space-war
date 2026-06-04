using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RewardPanelUI : MonoBehaviour
{
    [Header("Reward Input (tạm thời, sau lấy từ ControlRoom)")]
    public int earnedXP   = 50;
    public int earnedGold = 100;

    [Header("UI")]
    public TMP_Text levelText;
    public Slider   xpSlider;
    public TMP_Text xpText;
    public TMP_Text goldText;
    public TMP_Text diamondText;
    public GameObject claimButton;

    [Header("Anim")]
    public float countDuration = 1.5f;

    private bool _claimed = false;

    // Gọi khi start lobby — chỉ load và hiện data, không liên quan reward
    public void LoadDisplay()
    {
        PlayerData data = SaveManager.Load();
        RefreshUI(data, data.currentXP, data.gold);
    }

    // Gọi khi vào Lobby — chỉ hiện data hiện tại + số thưởng chờ, chưa lưu
    public void Preview()
    {
        _claimed = false;
        PlayerData data = SaveManager.Load();
        RefreshUI(data, data.currentXP, data.gold);
        if (claimButton) claimButton.SetActive(true);
    }

    // Gắn vào OnClick() của nút Nhận
    public void OnClaimClick()
    {
        if (_claimed) return;
        _claimed = true;

        if (claimButton) claimButton.SetActive(false);
        LobbyManager.Instance.HideReward(); // Gọi từ LobbyManager để bắt đầu quy trình nhận thưởng (có thể thêm hiệu ứng, delay, v.v.)
        StartCoroutine(ClaimRoutine());


    }

    private IEnumerator ClaimRoutine()
    {
        PlayerData data = SaveManager.Load();

        int oldXP   = data.currentXP;
        int oldGold = data.gold;

        data.currentXP += earnedXP;
        data.gold      += earnedGold;

        while (data.currentXP >= data.xpToNextLevel)
        {
            data.currentXP    -= data.xpToNextLevel;
            data.level        += 1;
            data.xpToNextLevel = Mathf.RoundToInt(data.xpToNextLevel * 1.3f);
        }

        SaveManager.Save(data);

        yield return StartCoroutine(AnimateValues(oldXP, oldGold, data));
    }

    private IEnumerator AnimateValues(int fromXP, int fromGold, PlayerData target)
    {
        float t = 0f;

        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / countDuration;
            float ease = Mathf.SmoothStep(0f, 1f, t);

            int displayXP   = Mathf.RoundToInt(Mathf.Lerp(fromXP,   target.currentXP, ease));
            int displayGold = Mathf.RoundToInt(Mathf.Lerp(fromGold, target.gold,       ease));

            RefreshUI(target, displayXP, displayGold);
            yield return null;
        }

        RefreshUI(target, target.currentXP, target.gold);
    }

    private void RefreshUI(PlayerData data, int displayXP, int displayGold)
    {
        if (levelText) levelText.text = "Level " + data.level;
        if (xpSlider)
        {
            xpSlider.maxValue = data.xpToNextLevel;
            xpSlider.value    = displayXP;
        }
        if (xpText)      xpText.text     = $"{displayXP} / {data.xpToNextLevel}";
        if (goldText)    goldText.text    = displayGold.ToString();
        if (diamondText) diamondText.text = data.diamond.ToString();
    }
}
