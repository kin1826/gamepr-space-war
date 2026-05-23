using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SkeletonHealth))]
public class SkeletonAI : MonoBehaviour
{
    [Header("References")]
    public Transform          playerTransform;
    public SkeletonAttackData data;

    [Header("Animator Parameters")]
    public string paramSpeed = "Speed";

    // ── Components ────────────────────────────────────────────
    private NavMeshAgent         _agent;
    private Animator             _anim;
    private SkeletonHealth       _health;
    private SkeletonSpawner      _spawner;
    private SkeletonAttackMelee  _melee;
    private SkeletonAttackRange  _range;

    // ── State ─────────────────────────────────────────────────
    private enum State { Waiting, Chase, Attack, Scream, Dead }
    private State _state = State.Waiting;

    private float _attackTimer;
    private float _throwTimer;
    private bool  _isAttacking;

    // Scream buff
    public  bool  IsScreamBuffed  { get; private set; }
    private float _screamBuffTimer;
    private bool  _hasFirstScreamed;

    private readonly int[] _meleeWeights = { 40, 30, 20, 10 };

    void Start()
    {
        _agent   = GetComponent<NavMeshAgent>();
        _anim    = GetComponent<Animator>();
        _health  = GetComponent<SkeletonHealth>();
        _spawner = GetComponent<SkeletonSpawner>();
        _melee   = GetComponent<SkeletonAttackMelee>();
        _range   = GetComponent<SkeletonAttackRange>();

        _melee?.Init(data);
        _range?.Init(data);

        _agent.speed            = data.moveSpeed;
        _agent.stoppingDistance = data.meleeRange * 0.85f;
        _agent.updateRotation   = false;
        _agent.updateUpAxis     = false;
    }

    void Update()
    {
        if (_health.IsDead) return;

        if (_spawner != null && !_spawner.IsReady) return;
        if (_state == State.Waiting) _state = State.Chase;

        if (IsScreamBuffed)
        {
            _screamBuffTimer -= Time.deltaTime;
            if (_screamBuffTimer <= 0f) IsScreamBuffed = false;
        }

        // Normalize speed về 0-1 cho Blend Tree
        float normalizedSpeed = _agent.velocity.magnitude / data.moveSpeed;
        _anim.SetFloat(paramSpeed, normalizedSpeed, 0.1f, Time.deltaTime);

        switch (_state)
        {
            case State.Chase:  UpdateChase();  break;
            case State.Attack: UpdateAttack(); break;
        }
    }

    // ── CHASE ────────────────────────────────────────────────
    void UpdateChase()
    {
        if (playerTransform == null) return;

        float dist = Dist();

        // Vào tầm ném dao
        if (dist <= data.throwRange && dist > data.meleeRange)
        {
            _agent.isStopped = true;
            _agent.velocity  = Vector3.zero;
            FacePlayer();
            TryThrow();
            return;
        }

        // Vào tầm cận chiến
        if (dist <= data.meleeRange)
        {
            EnterAttack();
            return;
        }

        // Chase — xoay theo hướng di chuyển
        _agent.isStopped = false;
        _agent.speed     = data.moveSpeed * (IsScreamBuffed ? data.screamSpeedBuff : 1f);
        _agent.SetDestination(playerTransform.position);
        FaceVelocity(); // ← xoay theo velocity thay vì FacePlayer
    }

    // ── ATTACK ───────────────────────────────────────────────
    void UpdateAttack()
    {
        if (_isAttacking) return;

        float dist = Dist();

        if (dist > data.meleeRange + 1f) { EnterChase(); return; }

        FacePlayer();
        _agent.isStopped = true;

        _attackTimer -= Time.deltaTime;
        if (_attackTimer <= 0f)
        {
            StartCoroutine(DoWeightedMelee());
            _attackTimer = data.meleeCooldown;
        }
    }

    // ── WEIGHTED MELEE ────────────────────────────────────────
    IEnumerator DoWeightedMelee()
    {
        _isAttacking = true;

        int total = 0;
        foreach (var w in _meleeWeights) total += w;
        int roll = Random.Range(0, total);
        int cum  = 0;

        for (int i = 0; i < _meleeWeights.Length; i++)
        {
            cum += _meleeWeights[i];
            if (roll < cum)
            {
                switch (i)
                {
                    case 0: yield return _melee.DoSlash1(); break;
                    case 1: yield return _melee.DoSlash2(); break;
                    case 2: yield return _melee.DoStab();   break;
                    case 3: yield return DoScream();        break;
                }
                break;
            }
        }

        _isAttacking = false;
    }

    // ── THROW ─────────────────────────────────────────────────
    void TryThrow()
    {
        if (_range == null || _range.IsThrowing) return;

        _throwTimer -= Time.deltaTime;
        if (_throwTimer <= 0f)
        {
            StartCoroutine(_range.DoThrow(playerTransform));
            _throwTimer = data.throwCooldown;
        }
    }

    // ── SCREAM ────────────────────────────────────────────────
    IEnumerator DoScream()
    {
        _state           = State.Scream;
        _agent.isStopped = true;
        _agent.velocity  = Vector3.zero;

        _anim.SetTrigger("Scream");
        Debug.Log("[AI] Skeleton SCREAM — buff speed!");

        yield return new WaitForSeconds(data.screamDuration);

        IsScreamBuffed   = true;
        _screamBuffTimer = data.screamBuffTime;

        _state           = State.Chase;
        _agent.isStopped = false;
    }

    // ── CALLBACKS ─────────────────────────────────────────────
    public void OnDead()
    {
        _state           = State.Dead;
        _agent.isStopped = true;
        _agent.velocity  = Vector3.zero;
        _agent.enabled   = false;
        StopAllCoroutines();
    }

    // ── STATE TRANSITIONS ────────────────────────────────────
    void EnterChase()
    {
        _state           = State.Chase;
        _agent.isStopped = false;

        if (!_hasFirstScreamed)
        {
            _hasFirstScreamed = true;
            StartCoroutine(FirstScream());
        }
    }

    IEnumerator FirstScream()
    {
        _state           = State.Scream;
        _agent.isStopped = true;
        _anim.SetTrigger("Scream");
        Debug.Log("[AI] Skeleton phát hiện player — SCREAM!");
        yield return new WaitForSeconds(data.screamDuration);
        _state           = State.Chase;
        _agent.isStopped = false;
    }

    void EnterAttack()
    {
        _state           = State.Attack;
        _agent.isStopped = true;
        _attackTimer     = 0f;
    }

    // ── HELPERS ───────────────────────────────────────────────
    void FacePlayer()
    {
        if (playerTransform == null) return;
        Vector3 dir = playerTransform.position - transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.01f) return;
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            Quaternion.LookRotation(dir),
            8f * Time.deltaTime);
    }

    // Xoay theo hướng NavMesh đang di chuyển
    void FaceVelocity()
    {
        Vector3 vel = _agent.velocity;
        vel.y = 0f;
        if (vel.sqrMagnitude < 0.01f) return;
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            Quaternion.LookRotation(vel),
            10f * Time.deltaTime);
    }

    float Dist() => Vector3.Distance(
        new Vector3(transform.position.x, 0, transform.position.z),
        new Vector3(playerTransform.position.x, 0, playerTransform.position.z));

    void OnDrawGizmosSelected()
    {
        if (data == null) return;
        Vector3 c = transform.position + Vector3.up;
        Gizmos.color = Color.yellow; Gizmos.DrawWireSphere(c, data.detectionRange);
        Gizmos.color = Color.red;    Gizmos.DrawWireSphere(c, data.meleeRange);
        Gizmos.color = Color.blue;   Gizmos.DrawWireSphere(c, data.throwRange);
        Gizmos.color = Color.cyan;
        if (TryGetComponent<SkeletonSpawner>(out var s))
            Gizmos.DrawWireSphere(c, s.triggerRadius);
    }
}