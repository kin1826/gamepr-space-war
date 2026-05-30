using UnityEngine;

public class MaterialChange : MonoBehaviour
{
    [Tooltip("Nếu để trống sẽ tự lấy Renderer trên object này")]
    public Renderer targetRenderer;

    [Tooltip("Slot material cần đổi (0, 1, 2, 3...)")]
    public int slotIndex = 0;

    [Tooltip("Danh sách material theo thứ tự. Gọi SetState(index) để đổi.")]
    public Material[] states;

    void Awake()
    {
        if (targetRenderer == null)
            targetRenderer = GetComponent<Renderer>();
    }

    public void SetState(int index)
    {
        if (targetRenderer == null || index < 0 || index >= states.Length) return;

        // Lấy bản copy của mảng materials hiện tại
        Material[] mats = targetRenderer.materials;

        if (slotIndex < 0 || slotIndex >= mats.Length) return;

        mats[slotIndex] = states[index];
        targetRenderer.materials = mats;
    }
}
