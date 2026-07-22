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

    [Header("Audio")]
    [SerializeField] private AudioClip openSfx;

    private bool playerInRange = false;
    private bool isOpened = false;
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
        if (!playerInRange || isOpened) return;

        // Lưu ý: action Interact hiện đang bind phím F trong InputSystem_Actions, không phải E
        Manager.Instance.ShowHint("Press F to open");
    }

    private void Interact(InputAction.CallbackContext ctx)
    {
        if (!playerInRange || isOpened) return;

        OpenCrate();
    }

    private void OpenCrate()
    {
        isOpened = true;

        if (openSfx != null)
            AudioManager.Instance.PlaySFX(openSfx);

        Manager.Instance.ShowDefaultHint();

        if (lid != null)
            StartCoroutine(RotateLidOpen());

        GiveLoot();
    }

    private void GiveLoot()
    {
        foreach (ItemData item in lootItems)
        {
            if (item == null) continue;

            // TODO: chưa có hệ thống inventory/HUD nhận item -> tạm log ra để test.
            // Khi có inventory, thay dòng dưới bằng ví dụ: Inventory.Instance.Add(item);
            Debug.Log($"[CrateInteract] Nhận vật phẩm: {item.itemName} ({item.id})");
        }
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
            playerInRange = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            Manager.Instance.ShowDefaultHint();
        }
    }
}
