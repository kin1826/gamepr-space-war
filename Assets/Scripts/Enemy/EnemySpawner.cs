using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject enemyPrefab;
    public int enemyCount = 5;
    public float spawnInterval = 3f;

    [Header("Spawn Area")]
    public float spawnRadius = 50f;
    public float minDistanceFromPlayer = 20f;
    public float minDistanceBetweenEnemies = 5f;

    public Transform player;

    private readonly List<Vector3> spawnedPositions = new List<Vector3>();

    void Start()
    {
        StartCoroutine(SpawnEnemies());
    }

    IEnumerator SpawnEnemies()
    {
        for (int i = 0; i < enemyCount; i++)
        {
            Vector3 spawnPos = GetSpawnPosition();
            spawnedPositions.Add(spawnPos);

            GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);

            EnemyController ec = enemy.GetComponent<EnemyController>();
            if (ec != null)
            {
                ec.player = player;
            }

            if (i < enemyCount - 1)
                yield return new WaitForSeconds(spawnInterval);
        }
    }

    Vector3 GetSpawnPosition()
    {
        Vector3 pos;
        int attempts = 0;

        do
        {
            pos = transform.position + Random.insideUnitSphere * spawnRadius;
            pos.y = transform.position.y;
            attempts++;
        }
        while (!IsValidSpawnPosition(pos) && attempts < 50);

        return pos;
    }

    bool IsValidSpawnPosition(Vector3 pos)
    {
        if (player != null && Vector3.Distance(pos, player.position) < minDistanceFromPlayer)
            return false;

        foreach (Vector3 spawnedPos in spawnedPositions)
        {
            if (Vector3.Distance(pos, spawnedPos) < minDistanceBetweenEnemies)
                return false;
        }

        return true;
    }
}
