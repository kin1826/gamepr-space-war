using UnityEngine;

// Gắn script này vào item nằm trên mặt đất: item sẽ xoay quanh trục đứng
// và bồng bềnh lên xuống để người chơi dễ nhận ra.
public class ItemFloat : MonoBehaviour
{
    [Header("Model")]
    [Tooltip("Kéo object chứa mesh của item vào đây (object con nằm trong empty này). Để trống sẽ dùng chính object gắn script.")]
    [SerializeField] private Transform model;

    [Header("Xoay quanh trục đứng")]
    [SerializeField] private float rotationSpeed = 90f; // độ/giây

    [Header("Bồng bềnh lên xuống")]
    [SerializeField] private float bobHeight = 0.25f;   // biên độ (m)
    [SerializeField] private float bobSpeed = 1.5f;      // chu kỳ/giây

    [Header("Emission nhấp nháy nhịp thở")]
    [Tooltip("Để trống sẽ tự lấy tất cả Renderer trong model (box, đạn, ...)")]
    [SerializeField] private Renderer[] targetRenderers;
    [SerializeField] private Color emissionColor = Color.cyan;
    [SerializeField] private float minEmissionIntensity = 0.2f;
    [SerializeField] private float maxEmissionIntensity = 3f;
    [SerializeField] private float pulseSpeed = 2f;

    private static readonly int EmissionColorId = Shader.PropertyToID("_EmissionColor");

    private Vector3 startLocalPos;
    private float bobOffset;
    private MaterialPropertyBlock propBlock;

    private void Awake()
    {
        if (model == null)
            model = transform;

        startLocalPos = model.localPosition;
        // Lệch pha ngẫu nhiên để nhiều item không nhấp nhô/nhấp nháy cùng nhịp
        bobOffset = Random.Range(0f, Mathf.PI * 2f);

        if (targetRenderers == null || targetRenderers.Length == 0)
            targetRenderers = model.GetComponentsInChildren<Renderer>();

        propBlock = new MaterialPropertyBlock();
    }

    private void Update()
    {
        model.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.Self);

        float y = Mathf.Sin(Time.time * bobSpeed + bobOffset) * bobHeight;
        model.localPosition = startLocalPos + Vector3.up * y;

        UpdateEmissionPulse();
    }

    private void UpdateEmissionPulse()
    {
        if (targetRenderers == null || targetRenderers.Length == 0) return;

        // 0..1 theo sóng sin -> nhịp thở sáng/mờ mượt, không giật cục
        float t = (Mathf.Sin(Time.time * pulseSpeed + bobOffset) + 1f) * 0.5f;
        float intensity = Mathf.Lerp(minEmissionIntensity, maxEmissionIntensity, t);
        Color color = emissionColor * intensity;

        foreach (Renderer r in targetRenderers)
        {
            if (r == null) continue;

            r.GetPropertyBlock(propBlock);
            propBlock.SetColor(EmissionColorId, color);
            r.SetPropertyBlock(propBlock);
        }
    }
}
