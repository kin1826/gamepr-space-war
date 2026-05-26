using UnityEngine;

public class Manager : MonoBehaviour
{
    public static Manager Instance;

    protected virtual void Awake()
    {
        Instance = this;
    }

    public virtual void ContinueGame()
    {

    }

    public virtual void ShowHint(string text)
    {

    }

    public virtual void HideHint()
    {

    }
    public virtual void ShowStory()
    {

    }

    public virtual void OpenSoilderPanel()
    {

    }

    public virtual void CloseSoilderPanel()
    {

    }

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

}