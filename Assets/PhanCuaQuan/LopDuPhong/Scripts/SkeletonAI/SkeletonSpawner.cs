using System.Collections;
using UnityEngine;

/// <summary>
/// Gắn lên Skeleton GameObject.
/// Skeleton ẩn dưới đất, nổi lên khi player bước vào trigger zone.
/// </summary>
public class SkeletonSpawner : MonoBehaviour
{
    [Header("Trigger")]
    public float triggerRadius = 5f; // bán kính phát hiện player

    [Header("References")]
    public SkeletonAttackData data;
    public LayerMask          playerLayer;

    [Header("Spawn Position")]
    [Tooltip("Vị trí dưới đất — Y âm")]
    public float undergroundY = -2f;

    // ── Private ───────────────────────────────────────────────
    private Animator     _anim;
    private SkeletonAI   _ai;
    private bool         _hasSpawned;
    private Vector3      _spawnPos; // vị trí trên mặt đất

    public bool IsReady { get; private set; } // AI chỉ chạy sau khi spawn xong

    void Start()
    {
        _anim     = GetComponent<Animator>();
        _ai       = GetComponent<SkeletonAI>();
        _spawnPos = transform.position;

        // Ẩn xuống đất ngay từ đầu
        Vector3 hidePos = _spawnPos;
        hidePos.y = undergroundY;
        transform.position = hidePos;
    }

    void Update()
    {
        if (_hasSpawned) return;

        // Kiểm tra player đi vào vùng trigger
        Collider[] hits = Physics.OverlapSphere(
            new Vector3(_spawnPos.x, _spawnPos.y, _spawnPos.z),
            triggerRadius, playerLayer);

        if (hits.Length > 0)
            StartCoroutine(RiseSequence());
    }

    IEnumerator RiseSequence()
    {
        _hasSpawned = true;

        yield return new WaitForSeconds(data != null ? data.spawnDelay : 0.5f);

        // Trigger animation Underground
        _anim.SetTrigger("Underground");

        // Nổi lên từ từ
        float riseTime = data != null ? data.riseTime : 1.2f;
        float elapsed  = 0f;
        Vector3 startPos = transform.position;

        while (elapsed < riseTime)
        {
            float t = elapsed / riseTime;
            transform.position = Vector3.Lerp(startPos, _spawnPos, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = _spawnPos;
        _anim.SetTrigger("Spawn");
        yield return new WaitForSeconds(1f);

        // Thêm — warp NavMesh lên đúng vị trí
        var agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agent != null)
        {
        agent.enabled = false;
        yield return null; // chờ 1 frame
        agent.enabled = true;
    
        if (UnityEngine.AI.NavMesh.SamplePosition(
        _spawnPos, out UnityEngine.AI.NavMeshHit hit, 2f, 
        UnityEngine.AI.NavMesh.AllAreas))
        {
        agent.Warp(hit.position);
    }
}

IsReady = true;
Debug.Log("[Spawner] Skeleton đã nổi lên + NavMesh ready!");
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(
            new Vector3(transform.position.x, transform.position.y, transform.position.z),
            triggerRadius);
    }
}