using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class AILocomotion : MonoBehaviour
{
    [Header("References")]
    public Transform playerTransform;

    [Header("Detection")]
    public float detectionRange = 20f;
    public float attackRange    = 2f;
    public float fieldOfView    = 120f;

    [Header("Patrol")]
    public Transform[] patrolPoints;
    public float randomPatrolRadius = 8f;
    public float patrolWaitTime     = 2f;

    [Header("Movement Speed")]
    public float patrolSpeed = 1.5f;
    public float chaseSpeed  = 3f;

    [Header("Attack")]
    public float attackCooldown = 1.5f;

    [Header("Animator Parameters")]
    public string speedParam  = "Speed";
    public string attackParam = "Attack";

    // ── private ──────────────────────────────────────────────────
    private NavMeshAgent _agent;
    private Animator     _anim;
    private Vector3      _spawnPos;

    private enum State { Patrol, Chase, Attack }
    private State _state;

    private int   _patrolIndex;
    private float _waitTimer;
    private bool  _waiting;
    private float _attackTimer;

    // ─────────────────────────────────────────────────────────────
    void Start()
    {
        _agent    = GetComponent<NavMeshAgent>();
        _anim     = GetComponent<Animator>();
        _spawnPos = transform.position;

        // Stopping distance phải nhỏ hơn attackRange
        _agent.stoppingDistance = attackRange * 0.8f;

        EnterPatrol();
    }

    void Update()
    {
        if (playerTransform == null) return;

        // Cập nhật animation tốc độ liên tục
        _anim.SetFloat(speedParam, _agent.velocity.magnitude, 0.1f, Time.deltaTime);

        switch (_state)
        {
            case State.Patrol: UpdatePatrol(); break;
            case State.Chase:  UpdateChase();  break;
            case State.Attack: UpdateAttack(); break;
        }
    }

    // ── PATROL ───────────────────────────────────────────────────
    void UpdatePatrol()
    {
        if (CanDetectPlayer())
        {
            EnterChase();
            return;
        }

        _agent.speed = patrolSpeed;

        if (_waiting)
        {
            _waitTimer -= Time.deltaTime;
            if (_waitTimer <= 0f) { _waiting = false; GoNextPatrol(); }
            return;
        }

        if (!_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance + 0.1f)
        {
            _waiting   = true;
            _waitTimer = patrolWaitTime;
        }
    }

    void GoNextPatrol()
    {
        if (patrolPoints != null && patrolPoints.Length > 0)
        {
            _agent.SetDestination(patrolPoints[_patrolIndex].position);
            _patrolIndex = (_patrolIndex + 1) % patrolPoints.Length;
        }
        else
        {
            Vector3 rnd = Random.insideUnitSphere * randomPatrolRadius + _spawnPos;
            if (NavMesh.SamplePosition(rnd, out NavMeshHit hit, randomPatrolRadius, NavMesh.AllAreas))
                _agent.SetDestination(hit.position);
        }
    }

    void EnterPatrol()
    {
        _state           = State.Patrol;
        _agent.isStopped = false;
        _agent.speed     = patrolSpeed;
        GoNextPatrol();
    }

    // ── CHASE ────────────────────────────────────────────────────
    void UpdateChase()
    {
        float dist = Dist();

        if (dist <= attackRange)
        {
            EnterAttack();
            return;
        }

        if (dist > detectionRange * 1.5f)
        {
            EnterPatrol();
            return;
        }

        _agent.isStopped = false;
        _agent.speed     = chaseSpeed;
        _agent.SetDestination(playerTransform.position);
    }

    void EnterChase()
    {
        _state           = State.Chase;
        _agent.isStopped = false;
        _agent.speed     = chaseSpeed;
    }

    // ── ATTACK ───────────────────────────────────────────────────
    void UpdateAttack()
    {
        float dist = Dist();

        // Player chạy ra ngoài → chase lại
        if (dist > attackRange + 0.5f)
        {
            EnterChase();
            return;
        }

        // Dừng và quay mặt về phía player
        _agent.isStopped = true;
        FacePlayer();

        // Đánh theo cooldown
        _attackTimer -= Time.deltaTime;
        if (_attackTimer <= 0f)
        {
            _attackTimer = attackCooldown;
            _anim.SetTrigger(attackParam);
        }
    }

    void EnterAttack()
    {
        _state           = State.Attack;
        _agent.isStopped = true;
        _attackTimer     = 0f; // đánh ngay lần đầu
    }

    // ── HELPERS ──────────────────────────────────────────────────
    float Dist() => Vector3.Distance(transform.position, playerTransform.position);

    void FacePlayer()
    {
        Vector3 dir = playerTransform.position - transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude > 0.01f)
            transform.rotation = Quaternion.Slerp(transform.rotation,
                                 Quaternion.LookRotation(dir), 10f * Time.deltaTime);
    }

    // Phát hiện player: khoảng cách + góc nhìn
    // Nếu rất gần (< attackRange*2) thì phát hiện 360° luôn
    bool CanDetectPlayer()
    {
        float dist = Dist();
        if (dist > detectionRange) return false;
        if (dist < attackRange * 2f) return true;

        float angle = Vector3.Angle(transform.forward,
                      playerTransform.position - transform.position);
        return angle <= fieldOfView * 0.5f;
    }

    // ── GIZMOS ───────────────────────────────────────────────────
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        if (patrolPoints == null) return;
        Gizmos.color = Color.green;
        for (int i = 0; i < patrolPoints.Length; i++)
        {
            if (patrolPoints[i] == null) continue;
            Gizmos.DrawSphere(patrolPoints[i].position, 0.3f);
            int next = (i + 1) % patrolPoints.Length;
            if (patrolPoints[next] != null)
                Gizmos.DrawLine(patrolPoints[i].position, patrolPoints[next].position);
        }
    }
}
