// WolfbossHealth.cs
using UnityEngine;
using UnityEngine.UI;
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
    public Canvas hpCanvas;
    public Slider hpSlider;

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
    private Camera     _cam;
    private float      _poiseRegenTimer;
    private bool       _poiseBroken;

    void Start()
    {
        CurrentHealth = maxHealth;
        CurrentPoise  = maxPoise;
        _anim = GetComponent<Animator>();
        _ai   = GetComponent<WolfbossAI>();
        _cam  = Camera.main;

        if (hpSlider) { hpSlider.maxValue = maxHealth; hpSlider.value = maxHealth; }
        if (hpCanvas) hpCanvas.gameObject.SetActive(false);
    }

    void Update()
    {
        if (IsDead) return;
        RegeneratePoise();
    }

    void LateUpdate()
    {
        if (hpCanvas == null || _cam == null) return;
        hpCanvas.transform.forward = _cam.transform.forward;
        Vector3 dir = (transform.position - _cam.transform.position).normalized;
        bool visible = Vector3.Dot(_cam.transform.forward, dir) > 0.5f;
        hpCanvas.gameObject.SetActive(visible && !IsDead);
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

        if (hpSlider) hpSlider.value = CurrentHealth;
        if (hpCanvas) hpCanvas.gameObject.SetActive(true);

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
        if (hpCanvas) hpCanvas.gameObject.SetActive(false);
        hpCanvas = null;
        _anim.SetTrigger(paramDie);
        _ai?.OnDead();
        OnDeath?.Invoke();
        Debug.Log("[Health] Boss đã chết!");
        if (destroyDelay > 0f) Destroy(gameObject, destroyDelay);
    }
}