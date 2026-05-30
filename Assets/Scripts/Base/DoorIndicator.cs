using UnityEngine;

/// <summary>
/// Gắn lên object đèn cửa. Quản lý cả material lẫn light cùng lúc.
/// </summary>
public class DoorIndicator : MonoBehaviour
{
    public MaterialChange materialChange;
    public Light pointLight;

    [Header("Colors")]
    public Color lockedColor   = Color.red;
    public Color unlockedColor = Color.green;

    [Header("Material States")]
    [Tooltip("Index material khi khoá")]
    public int lockedStateIndex   = 0;
    [Tooltip("Index material khi mở")]
    public int unlockedStateIndex = 1;

    void Start()
    {
        SetState(false);
    }

    public void SetState(bool unlocked)
    {
        if (materialChange) materialChange.SetState(unlocked ? unlockedStateIndex : lockedStateIndex);
        if (pointLight)     pointLight.color = unlocked ? unlockedColor : lockedColor;
    }
}
