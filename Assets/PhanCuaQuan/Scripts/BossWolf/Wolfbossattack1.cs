using System.Collections;
using UnityEngine;

public class WolfbossAttack1 : MonoBehaviour
{
    [Header("Damage")]
    public int   damage    = 30;
    public float knockback = 5f;

    [Header("Layer")]
    public LayerMask playerLayer;

    // ── Private ───────────────────────────────────────────────
    private Collider _hitbox;
    private bool     _hasHit;
    private float    _buffMult = 1f;

    void Awake()
    {
        _hitbox         = GetComponent<Collider>();
        _hitbox.enabled = false;
    }

    // ── Gọi từ Animation Event ────────────────────────────────
    public void EnableHitbox()
    {
        _hitbox.enabled = true;
        _hasHit         = false;
        Debug.Log("[Attack1] Hitbox ON");
    }

    public void DisableHitbox()
    {
        _hitbox.enabled = false;
        Debug.Log("[Attack1] Hitbox OFF");
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

    // ── Va chạm ───────────────────────────────────────────────
    void OnTriggerEnter(Collider other)
    {
        if (_hasHit) return;
        if ((playerLayer.value & (1 << other.gameObject.layer)) == 0) return;

        _hasHit         = true;
        _hitbox.enabled = false;

        int finalDmg = Mathf.RoundToInt(damage * _buffMult);
        Debug.Log($"[Attack1] Trúng {other.name}, dmg={finalDmg}");

        other.GetComponent<PlayerHealth>()?.TakeDamage(finalDmg);

        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 dir = (other.transform.position - transform.position).normalized;
            dir.y = 0.3f;
            rb.AddForce(dir * knockback, ForceMode.Impulse);
        }
    }
}