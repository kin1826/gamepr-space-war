using System;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float health;
    RagdollManager ragdollManager;
    [HideInInspector] public bool isDead;

    /// <summary>
    /// Được gọi 1 lần duy nhất khi enemy chết.
    /// EnemySpawnZone sẽ subscribe vào đây để track số lượng còn sống.
    /// </summary>
    [HideInInspector] public Action onDeath;

    private void Start()
    {
        ragdollManager = GetComponent<RagdollManager>();
    }

    public void TakeDamge(float damage)
    {
        if (!isDead && health > 0)
        {
            health -= damage;
            if (health <= 0) EnemyDeath();
            else Debug.Log("Hit");
        }
    }

    void EnemyDeath()
    {
        isDead = true;
        ragdollManager.TriggerRagdoll();
        Debug.Log("Death");
        onDeath?.Invoke();
    }
}
