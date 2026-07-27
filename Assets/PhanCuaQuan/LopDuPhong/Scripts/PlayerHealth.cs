using UnityEngine;
using System;
using StarterAssets;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [Header("Stats")]
    public int maxHealth = 200;

    [Header("Invincibility after hit")]
    [Tooltip("Thời gian bất tử sau khi bị hit")]
    public float invincibleDuration = 0.5f;

    // Public
    public int CurrentHealth { get; private set; }
    public bool IsDead { get; private set; }
    public float HealthPercent => (float)CurrentHealth / maxHealth;

    // Events
    public event Action<int, int> OnHealthChanged;
    public event Action OnDeath;

    // Private
    private float _invincibleTimer;
    private Animator anim;

    private void Start()
    {
        CurrentHealth = maxHealth;

        // Tìm Animator ở player hoặc object con
        anim = GetComponentInChildren<Animator>();

        Manager.Instance?.OnPlayerHealthChanged(CurrentHealth, maxHealth);
    }

    private void Update()
    {
        if (_invincibleTimer > 0f)
        {
            _invincibleTimer -= Time.deltaTime;
        }
    }

    // ==================================================
    // DAMAGE
    // ==================================================

    public void TakeDamage(float damage)
    {
        TakeDamage((int)damage);
    }

    public void TakeDamage(int amount)
    {
        if (IsDead) return;
        if (_invincibleTimer > 0f) return;

        CurrentHealth = Mathf.Clamp(
            CurrentHealth - amount,
            0,
            maxHealth
        );

        _invincibleTimer = invincibleDuration;

        Debug.Log($"[PlayerHealth] HP: {CurrentHealth}/{maxHealth} (-{amount})");

        // Nếu chết thì ưu tiên Death
        if (CurrentHealth <= 0)
        {
            Die();
            return;
        }

        // Hit Reaction
        if (anim != null)
        {
            anim.SetTrigger("Hit");
        }

        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
        Manager.Instance?.OnPlayerHealthChanged(CurrentHealth, maxHealth);
    }

    // ==================================================
    // HEAL
    // ==================================================

    // Trả về false khi player đã đầy máu hoặc đã chết để consumable không bị trừ sai.
    public bool TryHeal(int amount)
    {
        if (IsDead || amount <= 0 || CurrentHealth >= maxHealth)
            return false;

        Heal(amount);
        return true;
    }

    public void Heal(int amount)
    {
        if (IsDead) return;

        CurrentHealth = Mathf.Clamp(
            CurrentHealth + amount,
            0,
            maxHealth
        );

        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
        Manager.Instance?.OnPlayerHealthChanged(CurrentHealth, maxHealth);

        Debug.Log($"[PlayerHealth] Heal +{amount} → {CurrentHealth}/{maxHealth}");
    }

    // ==================================================
    // DEATH
    // ==================================================

    private void Die()
    {
        if (IsDead) return;

        IsDead = true;

        Debug.Log("DEATH TRIGGERED");

        if (anim != null)
        {
            anim.ResetTrigger("Hit");
            anim.SetBool("Dead", true);
        }

        OnDeath?.Invoke();

        Manager.Instance?.OnPlayerDied();

        // Khóa di chuyển
        ThirdPersonController movement = GetComponent<ThirdPersonController>();

        if (movement != null)
        {
            movement.enabled = false;
        }

        // Khóa Aim
        AimStateManager aim = GetComponent<AimStateManager>();

        if (aim != null)
        {
            aim.enabled = false;
        }
    }
}
