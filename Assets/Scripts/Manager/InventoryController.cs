using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

// Gắn vào Canvas hoặc UI Manager đang active, không gắn trực tiếp vào panel balo.
public class InventoryController : MonoBehaviour
{
    public static bool IsInventoryOpen { get; private set; }

    [Header("Inventory UI")]
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private Transform itemsContainer;
    [SerializeField] private GameObject inventoryItemPrefab;

    private void Start()
    {
        SetInventoryVisible(false);

        if (InventorySystem.Instance != null)
            InventorySystem.Instance.InventoryChanged += RefreshIfVisible;
    }

    private void OnDestroy()
    {
        if (InventorySystem.Instance != null)
            InventorySystem.Instance.InventoryChanged -= RefreshIfVisible;
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.tabKey.wasPressedThisFrame)
            SetInventoryVisible(!IsInventoryVisible());
    }

    public void SetInventoryVisible(bool isVisible)
    {
        if (inventoryPanel == null)
        {
            Debug.LogWarning("[InventoryController] Chưa gán Inventory Panel trong Inspector.", this);
            return;
        }

        inventoryPanel.SetActive(isVisible);
        IsInventoryOpen = isVisible;

        if (isVisible)
            RefreshInventory();

        // Mở balo thì giải phóng và hiện chuột để thao tác UI.
        // Đóng balo thì khoá và ẩn chuột để tiếp tục điều khiển game.
        Cursor.lockState = isVisible ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isVisible;
    }

    public bool IsInventoryVisible()
    {
        return inventoryPanel != null && inventoryPanel.activeSelf;
    }

    private void RefreshInventory()
    {
        if (itemsContainer == null || inventoryItemPrefab == null)
        {
            Debug.LogWarning("[InventoryController] Chưa gán Items Container hoặc Inventory Item Prefab.", this);
            return;
        }

        for (int i = itemsContainer.childCount - 1; i >= 0; i--)
            Destroy(itemsContainer.GetChild(i).gameObject);

        if (InventorySystem.Instance == null) return;

        foreach (InventorySlot slot in InventorySystem.Instance.Slots)
        {
            GameObject itemUI = Instantiate(inventoryItemPrefab, itemsContainer);
            Image icon = FindIconImage(itemUI);
            if (icon != null)
            {
                icon.sprite = slot.item.icon;
                icon.preserveAspect = true;
            }

            TMP_Text quantityText = itemUI.GetComponentInChildren<TMP_Text>(true);
            if (quantityText != null)
                quantityText.text = slot.quantity.ToString();
        }
    }

    public void RefreshIfVisible()
    {
        if (IsInventoryVisible())
            RefreshInventory();
    }

    private static Image FindIconImage(GameObject itemUI)
    {
        foreach (Image image in itemUI.GetComponentsInChildren<Image>(true))
        {
            if (image.transform != itemUI.transform)
                return image;
        }

        return itemUI.GetComponent<Image>();
    }
}
