using System.Collections;
using UnityEngine;

/// <summary>
/// Xử lý 3 attack cận chiến: Slash01, Slash02, Stab
/// Gắn lên Skeleton root GameObject
/// </summary>
public class SkeletonAttackMelee : MonoBehaviour
{
    [Header("Hitbox")]
    public Transform  hitPoint;     // điểm kiểm tra va chạm
    public float      hitRadius = 0.5f;
    public LayerMask  playerLayer;

    // ── Private ───────────────────────────────────────────────
    private SkeletonAttackData _data;
    private SkeletonHealth     _health;
    private bool               _hasHit;

    public bool IsAttacking { get; private set; }

    void Awake()
    {
        _health = GetComponent<SkeletonHealth>();
    }

    public void Init(SkeletonAttackData data) => _data = data;

    // ── Gọi từ SkeletonAI ─────────────────────────────────────
    public IEnumerator DoSlash1()
    {
        yield return DoMeleeAttack("Slash01", _data.slash1Damage, _data.slash1Duration);
    }

    public IEnumerator DoSlash2()
    {
        yield return DoMeleeAttack("Slash02", _data.slash2Damage, _data.slash2Duration);
    }

    public IEnumerator DoStab()
    {
        yield return DoMeleeAttack("Stab", _data.stabDamage, _data.stabDuration);
    }

    IEnumerator DoMeleeAttack(string trigger, int damage, float duration)
    {
        IsAttacking = true;
        _hasHit     = false;

        GetComponent<Animator>().SetTrigger(trigger);

        // Kiểm tra hit ở giữa animation
        yield return new WaitForSeconds(duration * 0.4f);
        CheckHit(damage);

        yield return new WaitForSeconds(duration * 0.6f);
        IsAttacking = false;
    }

    void CheckHit(int damage)
    {
        if (_hasHit) return;

        Transform point = hitPoint != null ? hitPoint : transform;
        Collider[] hits = Physics.OverlapSphere(point.position, hitRadius, playerLayer);

        foreach (var col in hits)
        {
            var ph = col.GetComponent<PlayerHealth>();
            if (ph == null) continue;

            _hasHit = true;
            bool isBuffed = GetComponent<SkeletonAI>()?.IsScreamBuffed ?? false;
            int  finalDmg = isBuffed ? Mathf.RoundToInt(damage * 1.3f) : damage;

            ph.TakeDamage(finalDmg);
            Debug.Log($"[Melee] Trúng {col.name}, dmg={finalDmg}");
            break;
        }
    }

    void OnDrawGizmosSelected()
    {
        if (hitPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(hitPoint.position, hitRadius);
    }
}