using UnityEngine;
using UnityEngine.InputSystem;

// Gắn vào Player. Hiện tại chỉ dùng quick slot 4 cho item hồi máu.
public class QuickSlotController : MonoBehaviour
{
    [Header("Quick Slots")]
    [SerializeField] private ItemData slot4Item;

    private void Update()
    {
        if (InventoryController.IsInventoryOpen || Keyboard.current == null)
            return;

        if (Keyboard.current.digit4Key.wasPressedThisFrame)
            UseSlot4();
    }

    public void UseSlot4()
    {
        if (slot4Item == null)
        {
            Debug.LogWarning("[QuickSlotController] Chưa gán item cho slot 4.", this);
            return;
        }

        if (InventorySystem.Instance == null)
        {
            Debug.LogWarning("[QuickSlotController] Không tìm thấy InventorySystem trong scene.", this);
            return;
        }

        InventorySystem.Instance.TryUseItem(slot4Item, gameObject);
    }
}
