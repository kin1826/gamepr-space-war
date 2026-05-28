using UnityEngine;

public class MaterialChange : MonoBehaviour
{
    [Tooltip("Nếu để trống sẽ tự lấy Renderer trên object này")]
    public Renderer targetRenderer;

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
        targetRenderer.material = states[index];
    }
}
