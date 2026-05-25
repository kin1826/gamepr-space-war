using UnityEngine;

public class BaseSceneManager : MonoBehaviour
{
    public static BaseSceneManager Instance;

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
        Debug.Log("[BaseSceneManager] OnWaveCleared — override ở class con để xử lý tiếp.");
    }

}