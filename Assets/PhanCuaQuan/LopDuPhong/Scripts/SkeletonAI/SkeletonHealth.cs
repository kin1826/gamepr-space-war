using UnityEngine;
using System;

public class SkeletonHealth : MonoBehaviour
{
    [Header("Stats")]
    public int maxHealth = 100;

    [Header("Animator")]
    public string paramHit = "TakeDamage";
    public string paramDie = "Death";

    [Header("On Death")]
    public float destroyDelay = 3f;

    public int   CurrentHealth { get; private set; }
    public bool  IsDead        { get; private set; }
    public float HealthPercent => (float)CurrentHealth / maxHealth;

    public event Action OnDeath;

    private Animator     _anim;
    private SkeletonAI   _ai;

    void Start()
    {
        CurrentHealth = maxHealth;
        _anim = GetComponent<Animator>();
        _ai   = GetComponent<SkeletonAI>();
    }

    public void TakeDamage(int amount)
    {
        if (IsDead) return;

        CurrentHealth = Mathf.Clamp(CurrentHealth - amount, 0, maxHealth);
        Debug.Log($"[SkeletonHealth] HP: {CurrentHealth}/{maxHealth}");

        if (CurrentHealth <= 0) Die();
        else _anim.SetTrigger(paramHit);
    }

    void Die()
    {
        if (IsDead) return;
        IsDead = true;
        _anim.SetTrigger(paramDie);
        _ai?.OnDead();
        OnDeath?.Invoke();
        Debug.Log("[SkeletonHealth] Skeleton chết!");
        if (destroyDelay > 0f) Destroy(gameObject, destroyDelay);
    }
}