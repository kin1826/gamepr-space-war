using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Gắn lên bất kỳ GameObject nào trong scene.
/// Lắng nghe input Pause (ESC) và gọi Manager.TogglePause().
/// </summary>
public class PauseController : MonoBehaviour
{
    private InputSystem_Actions _input;

    void Awake()
    {
        _input = new InputSystem_Actions();
    }

    void OnEnable()
    {
        _input.Enable();
        _input.UI.Cancel.performed += OnPause;
    }

    void OnDisable()
    {
        _input.UI.Cancel.performed -= OnPause;
        _input.Disable();
    }

    void OnPause(InputAction.CallbackContext ctx)
    {
        Manager.Instance?.TogglePause();
    }
}
