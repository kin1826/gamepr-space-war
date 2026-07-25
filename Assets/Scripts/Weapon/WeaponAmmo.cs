using UnityEngine;

public class WeaponAmmo : MonoBehaviour
{
    [Header("Inventory Ammo")]
    [Tooltip("Loại ItemData bị trừ trong balo mỗi khi súng bắn. Để trống sẽ dùng cơ chế ammo cũ.")]
    [SerializeField] private ItemData inventoryAmmoItem;

    public int clipSize;
    public int extraAmmo;
    [HideInInspector] public int currentAmmo;

    public AudioClip magInSound;
    public AudioClip magOutSound;
    public AudioClip releaseSlideSound;

    void Start()
    {
        // Player bắt đầu với một băng đầy, còn balo ban đầu có thể không có đạn dự trữ.
        currentAmmo = clipSize;
        SyncReserveAmmoFromInventory();

        if (InventorySystem.Instance != null)
            InventorySystem.Instance.InventoryChanged += OnInventoryChanged;

        NotifyAmmoChanged();
    }

    private void OnDestroy()
    {
        if (InventorySystem.Instance != null)
            InventorySystem.Instance.InventoryChanged -= OnInventoryChanged;
    }

    public void UseAmmo()
    {
        if (inventoryAmmoItem != null)
        {
            if (currentAmmo <= 0) return;

            // Đạn trong băng giảm khi bắn; đạn dự trữ chỉ giảm lúc reload.
            currentAmmo--;
            NotifyAmmoChanged();
            return;
        }

        currentAmmo--;
        NotifyAmmoChanged();
    }

    public bool HasAmmo()
    {
        return inventoryAmmoItem == null
            ? currentAmmo > 0
            : currentAmmo > 0;
    }

    public void Reload()
    {
        if (inventoryAmmoItem != null)
        {
            if (InventorySystem.Instance == null) return;

            int ammoNeeded = clipSize - currentAmmo;
            int reserveAmmo = GetRemainingAmmo();
            int ammoToLoad = Mathf.Min(ammoNeeded, reserveAmmo);

            if (ammoToLoad <= 0) return;

            if (InventorySystem.Instance.TryRemove(inventoryAmmoItem, ammoToLoad))
                currentAmmo += ammoToLoad;

            SyncReserveAmmoFromInventory();
            NotifyAmmoChanged();
            return;
        }

        if(extraAmmo >= clipSize)
        {
            int ammoToReload = clipSize - currentAmmo;
            extraAmmo -= ammoToReload;
            currentAmmo += ammoToReload;
        }
        else if(extraAmmo + currentAmmo > clipSize)
        {
            int leftOverAmmo = extraAmmo + currentAmmo - clipSize;
            extraAmmo = leftOverAmmo;
            currentAmmo = clipSize;
        }
        else
        {
            currentAmmo += extraAmmo;
            extraAmmo = 0;
        }

        NotifyAmmoChanged();
    }

    void NotifyAmmoChanged()
    {
        Manager.Instance?.OnAmmoChanged(currentAmmo, extraAmmo);
    }

    // Đạn dự trữ luôn là tổng số item đạn còn trong balo.
    public int GetRemainingAmmo()
    {
        if (inventoryAmmoItem == null)
            return extraAmmo;

        return InventorySystem.Instance == null
            ? 0
            : InventorySystem.Instance.GetQuantity(inventoryAmmoItem);
    }

    private void OnInventoryChanged()
    {
        if (inventoryAmmoItem == null) return;

        SyncReserveAmmoFromInventory();
        NotifyAmmoChanged();
    }

    private void SyncReserveAmmoFromInventory()
    {
        if (inventoryAmmoItem != null)
            extraAmmo = GetRemainingAmmo();
    }
}
