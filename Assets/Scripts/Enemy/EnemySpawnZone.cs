using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Gắn vào một empty GameObject để tạo vùng spawn quái.
/// Gọi StartWave() để bắt đầu 1 đợt. Khi hết quái → tự báo về BaseSceneManager.OnWaveCleared().
/// </summary>
public class EnemySpawnZone : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject enemyPrefab;

    [Min(1)]
    public int maxEnemies = 5;

    [Tooltip("Khoảng cách giữa 2 lần spawn (giây)")]
    public float spawnInterval = 0.5f;

    [Tooltip("Kéo player vào đây để gán cho AILocomotion của enemy sau khi spawn")]
    public Transform player;

    [Header("Zone Size")]
    [Tooltip("Kích thước vùng spawn (X, Y, Z). Y = 0 để spawn trên mặt phẳng 2D/ngang")]
    public Vector3 zoneSize = new Vector3(10f, 0f, 10f);

    [Header("Wave Control")]
    [Tooltip("false nếu dùng WaveRoom để điều khiển thứ tự")]
    public bool autoStart = true;

    /// <summary>WaveRoom subscribe vào đây để biết khi đợt này xong.</summary>
    [HideInInspector] public Action onZoneCleared;

    // ──────────────────────────────────────────
    // Runtime state
    // ──────────────────────────────────────────
    private readonly List<GameObject> _aliveEnemies = new List<GameObject>();
    private bool _waveActive;

    // ──────────────────────────────────────────
    // Public API
    // ──────────────────────────────────────────

    public void Start()
    {
        if (autoStart) StartWave();
    }

    /// <summary>
    /// Bắt đầu 1 đợt spawn. Nếu đợt trước chưa xong thì bỏ qua.
    /// </summary>
    public void StartWave()
    {
        if (_waveActive)
        {
            Debug.LogWarning($"[EnemySpawnZone] Wave đang chạy, bỏ qua StartWave() trên {gameObject.name}");
            return;
        }

        _waveActive = true;
        _aliveEnemies.Clear();
        StartCoroutine(SpawnRoutine());
    }

    /// <summary>
    /// Dừng đột ngột và huỷ hết quái còn sống (dùng khi cần reset).
    /// </summary>
    public void CancelWave()
    {
        StopAllCoroutines();
        foreach (var e in _aliveEnemies)
        {
            if (e != null) Destroy(e);
        }
        _aliveEnemies.Clear();
        _waveActive = false;
    }

    // ──────────────────────────────────────────
    // Spawn logic
    // ──────────────────────────────────────────

    private IEnumerator SpawnRoutine()
    {
        for (int i = 0; i < maxEnemies; i++)
        {
            SpawnOneEnemy();

            if (i < maxEnemies - 1)
                yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnOneEnemy()
    {
        if (enemyPrefab == null)
        {
            Debug.LogError($"[EnemySpawnZone] enemyPrefab chưa được gán trên {gameObject.name}!");
            return;
        }

        Vector3 pos = GetRandomPositionInZone();
        GameObject go = Instantiate(enemyPrefab, pos, Quaternion.identity);

        // Gán player cho AILocomotion
        var ai = go.GetComponentInChildren<AILocomotion>();
        if (ai) ai.playerTransform = player;

        // Subscribe sự kiện chết
        var health = go.GetComponentInChildren<EnemyHealth>();
        if (health != null)
        {
            // Dùng local variable để tránh closure bắt sai tham chiếu
            GameObject captured = go;
            health.onDeath += () => HandleEnemyDied(captured);
        }
        else
        {
            Debug.LogWarning($"[EnemySpawnZone] Prefab {enemyPrefab.name} thiếu EnemyHealth, không track được!");
        }

        _aliveEnemies.Add(go);
    }

    // ──────────────────────────────────────────
    // Death tracking
    // ──────────────────────────────────────────

    private void HandleEnemyDied(GameObject enemy)
    {
        _aliveEnemies.Remove(enemy);

        Debug.Log($"[EnemySpawnZone] Enemy chết. Còn lại: {_aliveEnemies.Count}/{maxEnemies}");

        if (_aliveEnemies.Count == 0)
        {
            _waveActive = false;
            NotifyWaveCleared();
        }
    }

    private void NotifyWaveCleared()
    {
        Debug.Log($"[EnemySpawnZone] Đã tiêu diệt hết đợt quái trên {gameObject.name}!");

        bool standalone = onZoneCleared == null;

        onZoneCleared?.Invoke();

        if (standalone)
        {
            if (Manager.Instance != null)
                Manager.Instance.OnWaveCleared();
            else
                Debug.LogWarning("[EnemySpawnZone] Không tìm thấy Manager.Instance!");
        }
    }

    // ──────────────────────────────────────────
    // Helpers
    // ──────────────────────────────────────────

    private Vector3 GetRandomPositionInZone()
    {
        Vector3 half = zoneSize * 0.5f;
        float x = UnityEngine.Random.Range(-half.x, half.x);
        float y = UnityEngine.Random.Range(-half.y, half.y);
        float z = UnityEngine.Random.Range(-half.z, half.z);
        return transform.position + new Vector3(x, y, z);
    }

    // ──────────────────────────────────────────
    // Editor Gizmos
    // ──────────────────────────────────────────

    private void OnDrawGizmos()
    {
        // Fill mờ
        Gizmos.color = new Color(1f, 0.35f, 0f, 0.15f);
        Gizmos.DrawCube(transform.position, zoneSize);

        // Viền rõ
        Gizmos.color = new Color(1f, 0.35f, 0f, 0.85f);
        Gizmos.DrawWireCube(transform.position, zoneSize);
    }
}
