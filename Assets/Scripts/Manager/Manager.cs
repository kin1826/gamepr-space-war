using UnityEngine;
using System.Collections.Generic;

public class Manager : MonoBehaviour
{
    public static Manager Instance;

    [Header("Default Hints")]
    [Tooltip("Danh sách hint mặc định. Gọi SetDefaultHint(index) để chuyển đổi giữa chúng.")]
    public List<string> defaultHints = new List<string>();

    private int _defaultHintIndex = 0;
    private string CurrentDefaultHint => defaultHints.Count > 0 ? defaultHints[_defaultHintIndex] : null;

    // Stack lưu các hint đang active theo thứ tự vào
    private readonly List<string> _activeHints = new List<string>();

    protected virtual void Awake()
    {
        Instance = this;
    }

    // ── Default hint API ───────────────────────────────────────────

    /// <summary>
    /// Chuyển sang default hint theo index trong list.
    /// Nếu stack trigger đang rỗng thì hiện ngay lập tức.
    /// </summary>
    public void SetDefaultHint(int index)
    {
        if (index < 0 || index >= defaultHints.Count) return;
        _defaultHintIndex = index;

        if (_activeHints.Count == 0)
            ShowOrHideDefault();
    }

    public void NextDefaultHint()
    {
        if (defaultHints.Count == 0) return;

        _defaultHintIndex = (_defaultHintIndex + 1) % defaultHints.Count;

        if (_activeHints.Count == 0)
            ShowOrHideDefault();
    }

    // ── Stack API — dùng bởi HintTrigger ──────────────────────────
    public void RegisterHint(string text)
    {
        _activeHints.Add(text);
        ShowHint(_activeHints[_activeHints.Count - 1]);
    }

    public void UnregisterHint(string text)
    {
        _activeHints.Remove(text);

        if (_activeHints.Count > 0)
            ShowHint(_activeHints[_activeHints.Count - 1]);
        else
            ShowOrHideDefault();
    }

    // Gọi trong Start() / ContinueGame() của manager con
    protected void InitDefaultHint()
    {
        _defaultHintIndex = 0;
        ShowOrHideDefault();
    }

    public void ShowDefaultHint() => ShowOrHideDefault();

    private void ShowOrHideDefault()
    {
        if (!string.IsNullOrEmpty(CurrentDefaultHint))
            ShowHint(CurrentDefaultHint);
        else
            HideHint();
    }

    // ── Virtual methods ────────────────────────────────────────────
    public virtual void ContinueGame()
    {

    }

    public virtual void ShowHint(string text)
    {

    }

    public virtual void HideHint()
    {

    }
    public virtual void ShowStory(int index = 0) { }

    public virtual void OpenSoilderPanel(int panelIndex = 0, int camIndex = 0) { }

    public virtual void CloseSoilderPanel() { }

    /// <summary>
    /// Được gọi bởi EnemySpawnZone khi tiêu diệt hết toàn bộ quái trong 1 đợt.
    /// Override ở class con để xử lý logic tiếp theo (mở cửa, chuyển cảnh, spawn đợt mới, v.v.)
    /// </summary>
    public virtual void OnWaveCleared()
    {
        Debug.Log("[Manager] OnWaveCleared — override ở class con để xử lý tiếp.");
    }

    /// <summary>
    /// Được gọi mỗi khi đạn thay đổi (bắn, reload, khởi tạo).
    /// Override ở class con để cập nhật UI hiển thị đạn.
    /// </summary>
    public virtual void OnAmmoChanged(int current, int extra) { }

    /// <summary>
    /// Được gọi mỗi khi máu player thay đổi.
    /// Override ở class con để cập nhật Slider hiển thị máu.
    /// </summary>
    public virtual void OnPlayerHealthChanged(int current, int max) { }

    /// <summary>
    /// Được gọi khi player chết. Override ở class con để xử lý respawn UI.
    /// </summary>
    public virtual void OnPlayerDied() { }

}