using System.Collections;
using UnityEngine;

public class WolfbossAttack1 : MonoBehaviour
{
    [Header("Damage")]
    public int   damage    = 30;
    public float knockback = 5f;

    [Header("Hit Radius — OverlapSphere")]
    [Tooltip("Bán kính kiểm tra damage khi vồ")]
    public float hitRadius = 1.2f;

    [Header("Layer")]
    public LayerMask playerLayer;

    [Header("VFX")]
    public GameObject slashVFX;

    [Header("SFX")]
    public AudioSource swooshSFX;
    public AudioSource hitSFX;

    private Collider _hitbox;
    private bool     _hasHit;
    private float    _buffMult = 1f;

    void Awake()
    {
        _hitbox         = GetComponent<Collider>();
        if (_hitbox != null) _hitbox.enabled = false;
    }

    // ── Gọi từ Animation Event ────────────────────────────────
    public void EnableHitbox()
    {
        _hasHit = false;
        if (_hitbox != null) _hitbox.enabled = true;
        if (swooshSFX != null) swooshSFX.Play();
        if (slashVFX != null)
        {
            GameObject fx = Instantiate(slashVFX, transform.position, transform.rotation);
            Destroy(fx, 1f);
        }
        Debug.Log("[Attack1] Hitbox ON");

        // OverlapSphere ngay lập tức — tương thích CharacterController
        CheckHitOverlap();
    }

    public void DisableHitbox()
    {
        if (_hitbox != null) _hitbox.enabled = false;
        Debug.Log("[Attack1] Hitbox OFF");
    }

    void CheckHitOverlap()
    {
        if (_hasHit) return;

        Collider[] hits = Physics.OverlapSphere(transform.position, hitRadius);
        foreach (var col in hits)
        {
            if (!col.CompareTag("Player")) continue;

            // Tìm PlayerHealth từ collider lên/xuống root
            PlayerHealth ph = col.GetComponent<PlayerHealth>()
                           ?? col.GetComponentInParent<PlayerHealth>()
                           ?? col.GetComponentInChildren<PlayerHealth>();
            if (ph == null) continue;

            _hasHit = true;
            if (_hitbox != null) _hitbox.enabled = false;

            int finalDmg = Mathf.RoundToInt(damage * _buffMult);
            ph.TakeDamage(finalDmg);
            if (hitSFX != null) hitSFX.Play();

            Debug.Log($"[Attack1] Trúng {col.name}, dmg={finalDmg}");

            Rigidbody rb = col.GetComponentInParent<Rigidbody>();
            if (rb != null)
            {
                Vector3 dir = (col.transform.position - transform.position).normalized;
                dir.y = 0.3f;
                rb.AddForce(dir * knockback, ForceMode.Impulse);
            }
            break;
        }
    }

    // Fallback OnTriggerEnter (nếu player có Rigidbody)
    void OnTriggerEnter(Collider other)
    {
        if (_hasHit || !other.CompareTag("Player")) return;
        PlayerHealth ph = other.GetComponent<PlayerHealth>()
                       ?? other.GetComponentInParent<PlayerHealth>()
                       ?? other.GetComponentInChildren<PlayerHealth>();
        if (ph == null) return;

        _hasHit = true;
        if (_hitbox != null) _hitbox.enabled = false;
        int finalDmg = Mathf.RoundToInt(damage * _buffMult);
        ph.TakeDamage(finalDmg);
        if (hitSFX != null) hitSFX.Play();
        Debug.Log($"[Attack1] Trúng {other.name}, dmg={finalDmg}");
    }

    // ── Buff damage từ Attack3 Phase 2 ────────────────────────
    public void ApplyDamageBuff(float mult, float duration)
    {
        StartCoroutine(BuffCoroutine(mult, duration));
    }

    IEnumerator BuffCoroutine(float mult, float duration)
    {
        _buffMult = mult;
        Debug.Log($"[Attack1] Damage buff x{mult} trong {duration}s");
        yield return new WaitForSeconds(duration);
        _buffMult = 1f;
        Debug.Log("[Attack1] Buff hết hạn");
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, hitRadius);
    }
}