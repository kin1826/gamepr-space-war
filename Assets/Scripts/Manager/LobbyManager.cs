using UnityEngine;

[DefaultExecutionOrder(-1000)]
public class LobbyManager : MonoBehaviour
{
    void Awake()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible   = true;
    }
}
