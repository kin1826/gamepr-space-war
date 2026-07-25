using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Quản lý panel hiển thị các vật phẩm lấy được từ rương.
// Gắn script này vào Canvas/UI manager đang active, sau đó kéo panel, Content
// và Assets/Prefab/Loot_item.prefab vào các field tương ứng trong Inspector.
public class LootPanelUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject panelRoot;
    [SerializeField] private Transform itemsContainer;
    [SerializeField] private GameObject lootItemPrefab;

    private void Start()
    {
        // LootPanelUI nên đặt trên Canvas/UI Manager đang active.
        // Panel loot luôn bắt đầu ở trạng thái ẩn.
        HideAndClear();
    }

    public void ShowLoot(IReadOnlyList<LootEntry> lootItems)
    {
        if (panelRoot == null || itemsContainer == null || lootItemPrefab == null)
        {
            Debug.LogError("[LootPanelUI] Hãy gán Panel Root, Items Container và Loot Item Prefab trong Inspector.", this);
            return;
        }

        ClearItems();

        if (lootItems != null)
        {
            foreach (LootEntry lootEntry in lootItems)
            {
                ItemData item = lootEntry.item;
                if (item == null) continue;

                GameObject lootItemUI = Instantiate(lootItemPrefab, itemsContainer);
                Image iconImage = FindIconImage(lootItemUI);

                if (iconImage == null)
                {
                    Debug.LogWarning($"[LootPanelUI] Prefab '{lootItemPrefab.name}' không có Image để hiện '{item.itemName}'.", lootItemUI);
                    continue;
                }

                iconImage.sprite = item.icon;
                iconImage.preserveAspect = true;

                TMP_Text quantityText = lootItemUI.GetComponentInChildren<TMP_Text>(true);
                if (quantityText != null)
                    quantityText.text = lootEntry.quantity.ToString();
            }
        }

        panelRoot.SetActive(true);
    }

    // Dùng khi player rời trigger hoặc gắn vào OnClick của nút Close.
    public void HideAndClear()
    {
        if (itemsContainer != null)
            ClearItems();

        if (panelRoot != null)
            panelRoot.SetActive(false);
    }

    private void ClearItems()
    {
        for (int i = itemsContainer.childCount - 1; i >= 0; i--)
            Destroy(itemsContainer.GetChild(i).gameObject);
    }

    private static Image FindIconImage(GameObject lootItemUI)
    {
        Image[] images = lootItemUI.GetComponentsInChildren<Image>(true);

        // Loot_item.prefab có Image ở root làm nền; ưu tiên Image con làm icon.
        foreach (Image image in images)
        {
            if (image.transform != lootItemUI.transform)
                return image;
        }

        return lootItemUI.GetComponent<Image>();
    }
}
