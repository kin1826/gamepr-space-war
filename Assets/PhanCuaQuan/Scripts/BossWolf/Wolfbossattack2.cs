using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class WolfbossAttack2 : MonoBehaviour
{
    [Header("Animator")]
    public string paramAttack2 = "Attack2";

    [Header("Optional VFX")]
    public ParticleSystem landVFX;
    public AudioSource    landSFX;

    // ── Public ────────────────────────────────────────────────
    public bool IsLunging { get; private set; }

    // ── Private ───────────────────────────────────────────────
    private WolfbossAttackData _data;
    private NavMeshAgent       _agent;
    private Animator           _anim;
    private WolfbossHealth     _health;

    void Awake()
    {
        _agent  = GetComponent<NavMeshAgent>();
        _anim   = GetComponent<Animator>();
        _health = GetComponent<WolfbossHealth>();
    }

    public void Init(WolfbossAttackData data) => _data = data;

    // ── Gọi từ WolfbossAI ─────────────────────────────────────
    public void StartLunge(Transform player)
    {
        if (IsLunging || _data == null) return;
        StartCoroutine(LungeSequence(player));
    }

    IEnumerator LungeSequence(Transform player)
    {
        IsLunging = true;
        _anim.SetTrigger(paramAttack2);

        // ── Phase 1: Lùi lấy đà ──────────────────────────────
        _agent.isStopped = true;
        _agent.enabled   = false;

        Vector3 backDir = (transform.position - player.position).normalized;
        backDir.y = 0f;

        float elapsed = 0f;
        while (elapsed < _data.atk2BackupTime)
        {
            transform.position += backDir * (_data.atk2BackupDist / _data.atk2BackupTime) * Time.deltaTime;
            FaceTarget(player.position);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // ── Phase 2: Nhảy lao vào ─────────────────────────────
        yield return new WaitForSeconds(0.15f);

        Vector3 start    = transform.position;
        Vector3 lungeDir = player.position - start;
        lungeDir.y = 0f;

        if (lungeDir.magnitude > _data.atk2LungeRange)
            lungeDir = lungeDir.normalized * _data.atk2LungeRange;

        Vector3 end      = start + lungeDir;
        float   lungeTime = lungeDir.magnitude / _data.atk2LungeSpeed;
        elapsed = 0f;

        while (elapsed < lungeTime)
        {
            float t   = elapsed / lungeTime;
            float arc = Mathf.Sin(t * Mathf.PI) * 1.2f;
            transform.position = Vector3.Lerp(start, end, t) + Vector3.up * arc;
            FaceTarget(end);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = end;

        // ── Phase 3: Hit khi đáp xuống ───────────────────────
        if (landVFX != null) landVFX.Play();
        if (landSFX != null) landSFX.Play();
        CheckLandHit();

        yield return new WaitForSeconds(0.3f);

        // Bật lại NavMesh
        _agent.enabled = true;
        if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            _agent.Warp(hit.position);
        _agent.isStopped = false;

        IsLunging = false;
    }

    void CheckLandHit()
    {
        bool  isPhase2 = _health != null && _health.IsPhase2;
        float dmgMult  = isPhase2 ? _data.phase2DamageMult : 1f;

        Collider[] hits = Physics.OverlapSphere(transform.position, _data.atk2LandRadius);
        foreach (var col in hits)
        {
            var ph = col.GetComponent<PlayerHealth>();
            if (ph == null) continue;

            int finalDmg = Mathf.RoundToInt(_data.atk2Damage * dmgMult);
            ph.TakeDamage(finalDmg);

            Rigidbody rb = col.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 dir = (col.transform.position - transform.position).normalized;
                dir.y = 0.5f;
                rb.AddForce(dir * _data.atk2Knockback, ForceMode.Impulse);
            }

            Debug.Log($"[Attack2] Lunge trúng {col.name}, dmg={finalDmg}");
        }
    }

    void FaceTarget(Vector3 target)
    {
        Vector3 dir = target - transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude > 0.01f)
            transform.rotation = Quaternion.LookRotation(dir);
    }

    void OnDrawGizmosSelected()
    {
        if (_data == null) return;
        Gizmos.color = new Color(1, 0.3f, 0, 0.35f);
        Gizmos.DrawSphere(transform.position, _data.atk2LandRadius);
    }
}