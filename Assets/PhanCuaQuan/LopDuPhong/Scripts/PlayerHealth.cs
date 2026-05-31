using UnityEngine;
using System;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [Header("Stats")]
    public int maxHealth = 200;

    [Header("Invincibility after hit")]
    [Tooltip("Thời gian bất tử sau khi bị hit (tránh nhận damage liên tục)")]
    public float invincibleDuration = 0.5f;

    // ── Public ────────────────────────────────────────────────
    public int   CurrentHealth { get; private set; }
    public bool  IsDead        { get; private set; }
    public float HealthPercent => (float)CurrentHealth / maxHealth;

    // Events — UI hoặc script khác lắng nghe
    public event Action<int, int> OnHealthChanged; // (current, max)
    public event Action           OnDeath;

    [Header("Animation")]
    [Tooltip("Tên trigger Death trong Animator. Để trống nếu không dùng.")]
    public string deathAnimParam = "Die";

    // ── Private ───────────────────────────────────────────────
    private float    _invincibleTimer;
    private Animator _anim;

    // ─────────────────────────────────────────────────────────
    void Start()
    {
        _anim         = GetComponentInChildren<Animator>();
        CurrentHealth = maxHealth;
        Manager.Instance?.OnPlayerHealthChanged(CurrentHealth, maxHealth);
    }

    void Update()
    {
        if (_invincibleTimer > 0f)
            _invincibleTimer -= Time.deltaTime;
    }

    // ── Nhận damage từ boss ───────────────────────────────────
    public void TakeDamage(float damage) => TakeDamage((int)damage);

    public void TakeDamage(int amount)
    {
        if (IsDead)                  return;
        if (_invincibleTimer > 0f)   return; // đang bất tử

        CurrentHealth     = Mathf.Clamp(CurrentHealth - amount, 0, maxHealth);
        _invincibleTimer  = invincibleDuration;

        Debug.Log($"[PlayerHealth] HP: {CurrentHealth}/{maxHealth} (-{amount})");

        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
        Manager.Instance?.OnPlayerHealthChanged(CurrentHealth, maxHealth);

        if (CurrentHealth <= 0) Die();
    }

    // ── Hồi máu ───────────────────────────────────────────────
    public void Heal(int amount)
    {
        if (IsDead) return;
        CurrentHealth = Mathf.Clamp(CurrentHealth + amount, 0, maxHealth);
        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
        Manager.Instance?.OnPlayerHealthChanged(CurrentHealth, maxHealth);
        Debug.Log($"[PlayerHealth] Heal +{amount} → {CurrentHealth}/{maxHealth}");
    }

    // ── Death ─────────────────────────────────────────────────
    void Die()
    {
        if (IsDead) return;
        IsDead = true;

        if (_anim != null && !string.IsNullOrEmpty(deathAnimParam))
            _anim.SetTrigger(deathAnimParam);

        OnDeath?.Invoke();
        Manager.Instance?.OnPlayerDied();
        Debug.Log("[PlayerHealth] Player đã chết!");
    }
}