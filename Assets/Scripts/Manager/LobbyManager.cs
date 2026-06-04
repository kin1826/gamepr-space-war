using UnityEngine;

[DefaultExecutionOrder(-1000)]
public class LobbyManager : MonoBehaviour
{
    void Awake()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible   = true;

        
    }

    void Start()
    {
        // Có thể thêm hiệu ứng fade in ở đây nếu muốn
        AudioManager.Instance.PlayTrack(0);
    }
}
