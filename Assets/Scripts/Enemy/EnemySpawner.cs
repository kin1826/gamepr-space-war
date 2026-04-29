using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject enemyPrefab;
    public int enemyCount = 5;

    [Header("Spawn Area")]
    public float spawnRadius = 50f;
    public float minDistanceFromPlayer = 20f;

    public Transform player;

    void Start()
    {
        SpawnEnemies();
    }

    void SpawnEnemies()
    {
        for (int i = 0; i < enemyCount; i++)
        {
            Vector3 spawnPos = GetSpawnPosition();

            GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);

            // 🔗 gán player cho enemy
            EnemyController ec = enemy.GetComponent<EnemyController>();
            if (ec != null)
            {
                ec.player = player;
            }
        }
    }

    Vector3 GetSpawnPosition()
    {
        Vector3 pos;

        do
        {
            pos = transform.position + Random.insideUnitSphere * spawnRadius;
            pos.y = transform.position.y; // giữ cùng độ cao
        }
        while (Vector3.Distance(pos, player.position) < minDistanceFromPlayer);

        return pos;
    }
}