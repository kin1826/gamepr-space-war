// WolfbossHealth.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class WolfbossHealth : MonoBehaviour, IDamageable
{
    [Header("Stats")]
    public int maxHealth = 500;

    [Header("Poise — Dark Souls style")]
    [Tooltip("Poise tối đa, mỗi lần bị hit trừ poisedamage của đòn")]
    public float maxPoise        = 100f;
    [Tooltip("Poise hồi phục mỗi giây khi không bị hit")]
    public float poiseRegenRate  = 20f;
    [Tooltip("Delay trước khi poise bắt đầu hồi")]
    public float poiseRegenDelay = 2f;

    [Header("Phase 2")]
    [Tooltip("Dưới % này đổi sang Phase 2")]
    [Range(0f, 1f)]
    public float phase2Threshold = 0.3f;

    [Header("Animator")]
    public string paramHit  = "Hit";
    public string paramDie  = "Die";
    public string paramRage = "Rage"; // trigger chuyển Phase 2

    [Header("UI")]
    public Slider     hpSlider;
    public TMP_Text   bossNameText;
    public TMP_Text   healthText;
    [Tooltip("Tên hiển thị trên UI")]
    public string     bossName = "Wolf Boss";

    [Header("On Death")]
    public float destroyDelay = 5f;

    // ── Public ────────────────────────────────────────────────
    public int   CurrentHealth  { get; private set; }
    public float CurrentPoise   { get; private set; }
    public bool  IsDead         { get; private set; }
    public bool  IsPhase2       { get; private set; }
    public float HealthPercent  => (float)CurrentHealth / maxHealth;

    // Events
    public event Action OnPhase2Enter;
    public event Action OnDeath;

    // ── Private ───────────────────────────────────────────────
    private Animator   _anim;
    private WolfbossAI _ai;
    private float      _poiseRegenTimer;
    private bool       _poiseBroken;

    void Start()
    {
        CurrentHealth = maxHealth;
        CurrentPoise  = maxPoise;
        _anim = GetComponent<Animator>();
        _ai   = GetComponent<WolfbossAI>();

        if (hpSlider)     { hpSlider.maxValue = maxHealth; hpSlider.value = maxHealth; }
        if (bossNameText) bossNameText.text = bossName;
        if (healthText)   healthText.text   = maxHealth.ToString();
    }

    void Update()
    {
        if (IsDead) return;
        RegeneratePoise();
    }


    // ── Nhận damage ───────────────────────────────────────────
    public void TakeDamage(float damage) => TakeDamage((int)damage);

    public void TakeDamage(int amount, float poiseDamage = 25f)
    {
        if (IsDead) return;

        CurrentHealth -= amount;
        CurrentHealth  = Mathf.Clamp(CurrentHealth, 0, maxHealth);

        // Poise
        CurrentPoise     -= poiseDamage;
        _poiseRegenTimer  = poiseRegenDelay;

        if (hpSlider)   hpSlider.value    = CurrentHealth;
        if (healthText) healthText.text   = CurrentHealth.ToString();

        Debug.Log($"[Health] HP:{CurrentHealth}/{maxHealth} | Poise:{CurrentPoise:F0}/{maxPoise}");

        if (CurrentHealth <= 0) { Die(); return; }

        // Check Phase 2
        if (!IsPhase2 && HealthPercent <= phase2Threshold)
            EnterPhase2();

        // Poise broken — stagger
        if (CurrentPoise <= 0f && !_poiseBroken)
            TriggerPoiseBroken();
        else if (CurrentPoise > 0f)
            _anim.SetTrigger(paramHit); // hit nhỏ, không stagger
    }

    // ── Poise ─────────────────────────────────────────────────
    void RegeneratePoise()
    {
        if (_poiseRegenTimer > 0f)
        {
            _poiseRegenTimer -= Time.deltaTime;
            return;
        }

        if (CurrentPoise < maxPoise)
        {
            CurrentPoise  = Mathf.Min(CurrentPoise + poiseRegenRate * Time.deltaTime, maxPoise);
            _poiseBroken  = false;
        }
    }

    void TriggerPoiseBroken()
    {
        _poiseBroken = true;
        CurrentPoise = maxPoise * 0.5f; // hồi 50% poise ngay sau stagger
        _anim.SetTrigger(paramHit);     // animation stagger (dùng chung Hit)
        _ai?.OnPoiseBreak();
        Debug.Log("[Health] Poise BROKEN — Stagger!");
    }

    // ── Phase 2 ───────────────────────────────────────────────
    void EnterPhase2()
    {
        IsPhase2 = true;
        _anim.SetTrigger(paramRage);
        OnPhase2Enter?.Invoke();
        Debug.Log("[Health] ⚡ PHASE 2 — Boss Enraged!");
    }

    // ── Death ─────────────────────────────────────────────────
    void Die()
    {
        if (IsDead) return;
        IsDead = true;
        _anim.SetTrigger(paramDie);
        _ai?.OnDead();
        OnDeath?.Invoke();
        Debug.Log("[Health] Boss đã chết!");
        if (destroyDelay > 0f) Destroy(gameObject, destroyDelay);

        if (Manager.Instance != null)
            Manager.Instance.OnWaveCleared();
        else
            Debug.LogWarning("[EnemySpawnZone] Không tìm thấy Manager.Instance!");
    }
}