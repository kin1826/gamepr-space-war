using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// Gắn vào empty có BoxCollider (Is Trigger) đặt tại vị trí rương.
// Player vào vùng trigger -> hiện hint tương tác; bấm phím Interact -> mở nắp rương + phát âm thanh.
public class CrateInteract : MonoBehaviour
{
    [Header("Nắp rương")]
    [Tooltip("Kéo object nắp rương vào đây, script sẽ xoay trục X object này khi mở")]
    [SerializeField] private Transform lid;
    [SerializeField] private float openAngleX = -120f;
    [SerializeField] private float openDuration = 0.6f;

    [Header("Vật phẩm bên trong")]
    [Tooltip("Kéo các ItemData sẽ nhận được khi mở rương vào đây")]
    [SerializeField] private List<ItemData> lootItems = new List<ItemData>();

    [Header("Loot UI")]
    [Tooltip("Kéo object đang có LootPanelUI vào đây")]
    [SerializeField] private LootPanelUI lootPanel;

    [Header("Audio")]
    [SerializeField] private AudioClip openSfx;

    private bool playerInRange = false;
    private bool isOpened = false;
    private int nextLootIndex = 0;
    private Vector3 lidClosedEuler;

    private InputSystem_Actions input;

    private void Awake()
    {
        input = new InputSystem_Actions();

        if (lid != null)
            lidClosedEuler = lid.localEulerAngles;
    }

    private void OnEnable()
    {
        input.Enable();
        input.Player.Interact.performed += Interact;
    }

    private void OnDisable()
    {
        input.Player.Interact.performed -= Interact;
        input.Disable();
    }

    private void Update()
    {
        if (!playerInRange) return;

        if (!isOpened)
            Manager.Instance.ShowHint("Press F to open");
        else if (nextLootIndex < lootItems.Count)
            Manager.Instance.ShowHint("Press F to take item");
        else
            Manager.Instance.ShowHint("Chest is empty");
    }

    private void Interact(InputAction.CallbackContext ctx)
    {
        if (!playerInRange) return;

        if (!isOpened)
            OpenCrate();
        else
            TakeNextLoot();
    }

    private void OpenCrate()
    {
        isOpened = true;

        if (openSfx != null)
            AudioManager.Instance.PlaySFX(openSfx);

        Manager.Instance.ShowDefaultHint();

        if (lid != null)
            StartCoroutine(RotateLidOpen());

        ShowLootPanel();
    }

    private void TakeNextLoot()
    {
        if (nextLootIndex >= lootItems.Count) return;

        if (InventorySystem.Instance == null)
        {
            Debug.LogWarning("[CrateInteract] Không tìm thấy InventorySystem trong scene.", this);
            return;
        }

        ItemData item = lootItems[nextLootIndex];
        nextLootIndex++;

        if (item != null)
        {
            // Mỗi lần loot nhận đầy một stack của loại item này.
            int lootAmount = Mathf.Max(1, item.maxStackSize);
            InventorySystem.Instance.Add(item, lootAmount);
        }

        // Cập nhật panel để chỉ còn hiển thị các vật phẩm chưa lấy.
        if (playerInRange)
            ShowLootPanel();
    }

    private void ShowLootPanel()
    {
        if (lootPanel == null)
        {
            Debug.LogWarning("[CrateInteract] Chưa gán LootPanelUI trong Inspector.", this);
            return;
        }

        lootPanel.ShowLoot(lootItems.GetRange(nextLootIndex, lootItems.Count - nextLootIndex));
    }

    private IEnumerator RotateLidOpen()
    {
        Quaternion from = lid.localRotation;
        Quaternion to = Quaternion.Euler(openAngleX, lidClosedEuler.y, lidClosedEuler.z);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / openDuration;
            lid.localRotation = Quaternion.Slerp(from, to, t);
            yield return null;
        }

        lid.localRotation = to;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;

            // Rương đã mở từ trước: vào lại vùng trigger thì hiện danh sách loot.
            if (isOpened)
                ShowLootPanel();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            Manager.Instance.ShowDefaultHint();
            lootPanel?.HideAndClear();
        }
    }
}
