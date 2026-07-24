using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class InventorySlot
{
    public ItemData item;
    public int quantity;
}

// Giữ dữ liệu balo trong thời gian chơi. Gắn cùng object với InventoryController.
public class InventorySystem : MonoBehaviour
{
    public static InventorySystem Instance { get; private set; }

    [SerializeField] private List<InventorySlot> slots = new List<InventorySlot>();
    public IReadOnlyList<InventorySlot> Slots => slots;

    public event Action InventoryChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void Add(ItemData item, int amount = 1)
    {
        if (item == null || amount <= 0) return;

        int maxStackSize = Mathf.Max(1, item.maxStackSize);

        // Ưu tiên lấp đầy các stack cùng loại đang còn chỗ.
        foreach (InventorySlot slot in slots)
        {
            if (slot.item != item || slot.quantity >= maxStackSize) continue;

            int added = Mathf.Min(amount, maxStackSize - slot.quantity);
            slot.quantity += added;
            amount -= added;

            if (amount == 0)
            {
                InventoryChanged?.Invoke();
                return;
            }
        }

        // Balo vô hạn: tạo stack mới đến khi nhận hết số lượng.
        while (amount > 0)
        {
            int stackAmount = Mathf.Min(amount, maxStackSize);
            slots.Add(new InventorySlot { item = item, quantity = stackAmount });
            amount -= stackAmount;
        }

        InventoryChanged?.Invoke();
    }
}
