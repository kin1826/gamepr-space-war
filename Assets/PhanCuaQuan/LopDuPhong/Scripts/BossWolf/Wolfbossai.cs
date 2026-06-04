using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(WolfbossHealth))]
public class WolfbossAI : MonoBehaviour
{
    [Header("References")]
    public Transform          playerTransform;
    public WolfbossAttackData data;

    [Header("Animator Parameters")]
    public string paramSpeed      = "Speed";
    public string paramBattleIdle = "BattleIdle";
    public string paramAttack1    = "Attack1";

    // ── Components ────────────────────────────────────────────
    private NavMeshAgent    _agent;
    private Animator        _anim;
    private WolfbossHealth  _health;
    private WolfbossAttack1 _atk1;
    private WolfbossAttack2 _atk2;
    private WolfbossAttack3 _atk3;

    // ── State ─────────────────────────────────────────────────
    private enum State { Idle, Chase, Attack, Stagger, Dead }
    private State _state = State.Idle;

    private float _attackTimer;
    private float _attackLock;
    private float _staggerTimer;
    private bool  _isPhase2;

    // Weighted random — Phase1: atk1 nhiều, Phase2: atk2 nhiều hơn
    private readonly int[] _weightsP1 = { 60, 25, 15 };
    private readonly int[] _weightsP2 = { 45, 45, 10 };

    // ─────────────────────────────────────────────────────────
    void Start()
    {
        _agent  = GetComponent<NavMeshAgent>();
        _anim   = GetComponent<Animator>();
        _health = GetComponent<WolfbossHealth>();
        _atk1   = GetComponentInChildren<WolfbossAttack1>();
        _atk2   = GetComponent<WolfbossAttack2>();
        _atk3   = GetComponent<WolfbossAttack3>();

        _health.OnPhase2Enter += HandlePhase2;

        _atk2?.Init(data);
        _atk3?.Init(data);

        _agent.stoppingDistance = data.minAttackRange * 0.9f;
        _agent.speed            = data.chaseSpeed;
    }

    void Update()
    {
        if (_health.IsDead) return;

        _anim.SetFloat(paramSpeed, _agent.velocity.magnitude, 0.1f, Time.deltaTime);

        switch (_state)
        {
            case State.Idle:    UpdateIdle();    break;
            case State.Chase:   UpdateChase();   break;
            case State.Attack:  UpdateAttack();  break;
            case State.Stagger: UpdateStagger(); break;
            case State.Dead:                     break;
        }
    }

    // ── IDLE ─────────────────────────────────────────────────
    void UpdateIdle()
    {
        if (playerTransform == null) return;
        if (Dist() <= data.detectionRange) EnterChase();
    }

    // ── CHASE ────────────────────────────────────────────────
    void UpdateChase()
    {
        float dist = Dist();

        if (dist <= data.maxAttackRange)          { EnterAttack(); return; }
        if (dist > data.detectionRange * 1.5f)    { EnterIdle();   return; }

        _agent.isStopped = false;
        _agent.speed     = data.chaseSpeed * (_isPhase2 ? data.phase2SpeedMult : 1f);
        _agent.SetDestination(playerTransform.position);
    }

    // ── ATTACK ───────────────────────────────────────────────
    void UpdateAttack()
    {
        // Đang lunge — không can thiệp
        if (_atk2 != null && _atk2.IsLunging) return;

        float dist = Dist();

        // Quá xa — chase lại
        if (dist > data.maxAttackRange + 3f) { EnterChase(); return; }

        FacePlayer();

        // ── Kiểm soát khoảng cách ────────────────────────────
        if (dist < data.minAttackRange)
        {
            // Player quá gần → lùi ra
            _agent.isStopped = false;
            Vector3 back     = (transform.position - playerTransform.position).normalized;
            back.y           = 0f;
            _agent.speed     = data.backoffSpeed;
            _agent.SetDestination(transform.position + back * 3f);
        }
        else if (dist > data.maxAttackRange)
        {
            // Player quá xa → tiến lại
            _agent.isStopped = false;
            _agent.speed     = data.chaseSpeed * 0.6f;
            _agent.SetDestination(playerTransform.position);
        }
        else
        {
            // Vùng lý tưởng → đứng yên đánh
            _agent.isStopped = true;
            _agent.velocity  = Vector3.zero;
        }

        // Lock animation
        if (_attackLock > 0f) { _attackLock -= Time.deltaTime; return; }

        // Đánh khi trong tầm
        if (dist >= data.minAttackRange && dist <= data.maxAttackRange)
        {
            float cooldown = data.attackCooldown * (_isPhase2 ? data.phase2CooldownMult : 1f);
            _attackTimer -= Time.deltaTime;
            if (_attackTimer <= 0f)
            {
                DoWeightedAttack();
                _attackTimer = cooldown;
            }
        }
    }

    // ── STAGGER ──────────────────────────────────────────────
    void UpdateStagger()
    {
        _staggerTimer -= Time.deltaTime;
        if (_staggerTimer <= 0f) EnterAttack();
    }

    // ── WEIGHTED RANDOM ATTACK ────────────────────────────────
    void DoWeightedAttack()
    {
        int[] weights = _isPhase2 ? _weightsP2 : _weightsP1;
        int total = 0;
        foreach (var w in weights) total += w;

        int roll = Random.Range(0, total);
        int cum  = 0;

        for (int i = 0; i < weights.Length; i++)
        {
            cum += weights[i];
            if (roll < cum)
            {
                switch (i)
                {
                    case 0: DoAttack1(); break;
                    case 1: DoAttack2(); break;
                    case 2: DoAttack3(); break;
                }
                return;
            }
        }
    }

    void DoAttack1()
    {
        Debug.Log("[AI] Attack1 — Vồ vuốt");
        _anim.SetTrigger(paramAttack1);
        _attackLock = data.atk1Duration;
    }

    void DoAttack2()
    {
        Debug.Log("[AI] Attack2 — Lao vào");
        _atk2?.StartLunge(playerTransform);
        _attackLock = data.atk2Duration;
    }

    void DoAttack3()
    {
        Debug.Log("[AI] Attack3 — Gầm");
        _atk3?.StartHowl();
        _attackLock = data.atk3Duration;
    }

    // ── PHASE 2 ───────────────────────────────────────────────
    void HandlePhase2()
    {
        _isPhase2   = true;
        _attackLock = 1.5f; // chờ animation Rage xong
        Debug.Log("[AI] Phase 2 — Boss Enraged!");
    }

    // ── CALLBACKS ─────────────────────────────────────────────
    public void OnPoiseBreak()
    {
        _state           = State.Stagger;
        _staggerTimer    = 1.2f;
        _agent.isStopped = true;
        _agent.velocity  = Vector3.zero;
        Debug.Log("[AI] Stagger!");
    }

    public void OnDead()
    {
        _state = State.Dead;
        // Dừng trước khi disable để tránh lỗi "Stop on inactive agent"
        if (_agent.enabled && _agent.isOnNavMesh)
        {
            _agent.isStopped = true;
            _agent.velocity  = Vector3.zero;
        }
        _agent.enabled = false;
        _anim.SetBool(paramBattleIdle, false);
        StopAllCoroutines();
    }

    // ── STATE TRANSITIONS ────────────────────────────────────
    void EnterIdle()
    {
        _state           = State.Idle;
        _agent.isStopped = true;
        _agent.velocity  = Vector3.zero;
        _anim.SetBool(paramBattleIdle, false);
    }

    void EnterChase()
    {
        _state           = State.Chase;
        _agent.isStopped = false;
        _anim.SetBool(paramBattleIdle, false);
    }

    void EnterAttack()
    {
        _state           = State.Attack;
        _agent.isStopped = true;
        _agent.velocity  = Vector3.zero;
        _anim.SetBool(paramBattleIdle, true);
        _attackTimer     = 0f;
        _attackLock      = 0f;
    }

    // ── HELPERS ───────────────────────────────────────────────
    void FacePlayer()
    {
        Vector3 dir = playerTransform.position - transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude > 0.01f)
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(dir),
                8f * Time.deltaTime);
    }

    float Dist() => Vector3.Distance(
        new Vector3(transform.position.x, 0, transform.position.z),
        new Vector3(playerTransform.position.x, 0, playerTransform.position.z));

    void OnDrawGizmosSelected()
    {
        if (data == null) return;
        Vector3 c = transform.position + Vector3.up;
        Gizmos.color = Color.yellow; Gizmos.DrawWireSphere(c, data.detectionRange);
        Gizmos.color = Color.green;  Gizmos.DrawWireSphere(c, data.maxAttackRange);
        Gizmos.color = Color.red;    Gizmos.DrawWireSphere(c, data.minAttackRange);
    }
}