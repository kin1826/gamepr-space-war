using UnityEngine;

public class WolfbossAura : MonoBehaviour
{
    [Header("Aura VFX")]
    public ParticleSystem auraVFX; // kéo AuraVFX vào đây

    [Header("Spawn Settings")]
    public Vector3 auraOffset = new Vector3(0, 0.5f, 0); // chỉnh vị trí
    public Vector3 auraScale  = Vector3.one;              // chỉnh size

    private WolfbossHealth _health;

    void Start()
    {
        _health = GetComponent<WolfbossHealth>();

        if (auraVFX != null)
        {
            auraVFX.transform.localPosition = auraOffset;
            auraVFX.transform.localScale    = auraScale;
            auraVFX.Play();
        }
    }

    void Update()
    {
        // Tắt aura khi boss chết
        if (_health != null && _health.IsDead)
        {
            if (auraVFX != null && auraVFX.isPlaying)
                auraVFX.Stop();
        }
    }
}