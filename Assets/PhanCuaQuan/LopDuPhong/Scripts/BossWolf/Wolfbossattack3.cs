using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
public class WolfbossAttack3 : MonoBehaviour
{
    [Header("Animator")]
    public string paramAttack3 = "Attack3";

    [Header("VFX")]
    public GameObject howlVFX;
    public GameObject phase2VFX;

    [Header("SFX")]
    public AudioSource howlSFX;
    public AudioSource phase2HowlSFX;

    [Header("Spawn Settings")]
    public Transform howlPoint;
    public Vector3   howlScale = Vector3.one;

    [Header("AoE Damage")]
    public float aoeDamageRadius = 5f;
    public int   aoeDamage       = 20;

    // ── Public ────────────────────────────────────────────────
    public bool IsHowling { get; private set; }

    // ── Private ───────────────────────────────────────────────
    private WolfbossAttackData _data;
    private Animator           _anim;
    private WolfbossHealth     _health;
    private WolfbossAttack1    _atk1;
    private NavMeshAgent       _agent;

    void Awake()
    {
        _anim   = GetComponent<Animator>();
        _health = GetComponent<WolfbossHealth>();
        _atk1   = GetComponentInChildren<WolfbossAttack1>();
        _agent  = GetComponent<NavMeshAgent>();
    }

    public void Init(WolfbossAttackData data) => _data = data;

    // ── Gọi từ WolfbossAI ─────────────────────────────────────
    public void StartHowl()
    {
        if (IsHowling || _data == null) return;
        StartCoroutine(HowlSequence());
    }

    IEnumerator HowlSequence()
    {
        IsHowling = true;
        _anim.SetTrigger(paramAttack3);

        // ── Khóa di chuyển ────────────────────────────────────
        if (_agent != null)
        {
            _agent.isStopped = true;
            _agent.velocity  = Vector3.zero;
        }

        bool isPhase2 = _health != null && _health.IsPhase2;

        // ── SFX ───────────────────────────────────────────────
        if (isPhase2)
        {
            if (phase2HowlSFX != null) phase2HowlSFX.Play();
            else if (howlSFX != null)  howlSFX.Play();
        }
        else
        {
            if (howlSFX != null) howlSFX.Play();
        }

        // ── VFX ───────────────────────────────────────────────
        Transform  spawnAt   = howlPoint != null ? howlPoint : transform;
        GameObject vfxPrefab = (isPhase2 && phase2VFX != null) ? phase2VFX : howlVFX;
        GameObject spawnedFX = null;

        if (vfxPrefab != null)
        {
            spawnedFX = Instantiate(vfxPrefab, spawnAt.position, spawnAt.rotation);
            spawnedFX.transform.SetParent(transform);
            spawnedFX.transform.localScale = howlScale;
        }

        Debug.Log($"[Attack3] Boss GẦMM! Phase2={isPhase2}");

        // ── Phase 2 buff damage Attack1 ───────────────────────
        if (isPhase2 && _atk1 != null && _data != null)
        {
            _atk1.ApplyDamageBuff(_data.atk3BuffMultiplier, _data.atk3Duration);
            Debug.Log($"[Attack3] Phase2 buff x{_data.atk3BuffMultiplier}!");
        }

        // ── Chờ nửa animation rồi gây AoE damage ─────────────
        float halfDuration = (_data != null ? _data.atk3Duration : 1.8f) * 0.5f;
        yield return new WaitForSeconds(halfDuration);

        HowlAoeDamage(isPhase2);

        // ── Chờ nửa còn lại ───────────────────────────────────
        yield return new WaitForSeconds(halfDuration);

        // ── Mở khóa di chuyển ─────────────────────────────────
        if (_agent != null)
            _agent.isStopped = false;

        // ── Xóa VFX ───────────────────────────────────────────
        if (spawnedFX != null) Destroy(spawnedFX);

        IsHowling = false;
    }

    // ── AoE Damage ────────────────────────────────────────────
    void HowlAoeDamage(bool isPhase2)
    {
        float dmgMult  = isPhase2 && _data != null ? _data.phase2DamageMult : 1f;
        int   finalDmg = Mathf.RoundToInt(aoeDamage * dmgMult);

        Collider[] hits = Physics.OverlapSphere(transform.position, aoeDamageRadius);
        foreach (var col in hits)
        {
            if (!col.CompareTag("Player")) continue;

            PlayerHealth ph = col.GetComponent<PlayerHealth>()
                           ?? col.GetComponentInParent<PlayerHealth>()
                           ?? col.GetComponentInChildren<PlayerHealth>();
            if (ph == null) continue;

            ph.TakeDamage(finalDmg);

            Debug.Log($"[Attack3] AoE trúng {col.name}, dmg={finalDmg}");
        }
    }

    // ── Gizmo xem vùng AoE ───────────────────────────────────
    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.5f, 0, 1f, 0.3f);
        Gizmos.DrawSphere(transform.position, aoeDamageRadius);
    }
}