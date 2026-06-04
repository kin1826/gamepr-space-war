using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class WolfbossHealth : MonoBehaviour, IDamageable
{
    [Header("Stats")]
    public int maxHealth = 500;

    [Header("Poise — Dark Souls style")]
    public float maxPoise        = 100f;
    public float poiseRegenRate  = 20f;
    public float poiseRegenDelay = 2f;

    [Header("Phase 2")]
    [Range(0f, 1f)]
    public float phase2Threshold = 0.3f;

    [Header("Animator")]
    public string paramHit  = "Hit";
    public string paramDie  = "Die";
    public string paramRage = "Rage";

    [Header("UI")]
    public Slider   hpSlider;
    public TMP_Text bossNameText;
    public TMP_Text healthText;
    public string   bossName = "Wolf Boss";

    [Header("Hit Blink Effect")]
    public float blinkIntensity = 3f;
    public float blinkDuration  = 0.15f;

    [Header("On Death")]
    public float destroyDelay = 5f;

    // ── Public ────────────────────────────────────────────────
    public int   CurrentHealth { get; private set; }
    public float CurrentPoise  { get; private set; }
    public bool  IsDead        { get; private set; }
    public bool  IsPhase2      { get; private set; }
    public float HealthPercent => (float)CurrentHealth / maxHealth;

    public event Action OnPhase2Enter;
    public event Action OnDeath;

    // ── Private ───────────────────────────────────────────────
    private Animator             _anim;
    private WolfbossAI           _ai;
    private float                _poiseRegenTimer;
    private bool                 _poiseBroken;
    private float                _blinkTimer;
    private SkinnedMeshRenderer  _smr;

    void Start()
    {
        CurrentHealth = maxHealth;
        CurrentPoise  = maxPoise;
        _anim = GetComponent<Animator>();
        _ai   = GetComponent<WolfbossAI>();

        // Lấy SkinnedMeshRenderer lớn nhất (body chính)
        var smrs = GetComponentsInChildren<SkinnedMeshRenderer>();
        if (smrs.Length > 0)
        {
            _smr = smrs[0];
            foreach (var s in smrs)
                if (s.bounds.size.magnitude > _smr.bounds.size.magnitude)
                    _smr = s;
        }

        // Bật keyword Emission để SetColor có hiệu lực trong URP
        if (_smr != null)
            _smr.material.EnableKeyword("_EMISSION");

        if (hpSlider)     { hpSlider.maxValue = maxHealth; hpSlider.value = maxHealth; }
        if (bossNameText) bossNameText.text = bossName;
        if (healthText)   healthText.text   = maxHealth.ToString();
    }

    void Update()
    {
        if (IsDead) return;
        RegeneratePoise();
        UpdateBlink();
    }

    void UpdateBlink()
    {
        if (_smr == null) return;
        float safeDuration = blinkDuration > 0f ? blinkDuration : 0.1f;
        float lerp         = Mathf.Clamp01(_blinkTimer / safeDuration);
        float intensity    = lerp * blinkIntensity;
        _blinkTimer        = Mathf.Max(0f, _blinkTimer - Time.deltaTime);

        // URP/Lit dùng _EmissionColor — không có _BlinkColor
        Color emissive = Color.white * intensity;
        _smr.material.SetColor("_EmissionColor", emissive);
    }

    // ── Nhận damage ───────────────────────────────────────────
    public void TakeDamage(float damage) => TakeDamage((int)damage);

    public void TakeDamage(int amount, float poiseDamage = 25f)
    {
        if (IsDead) return;

        CurrentHealth  = Mathf.Clamp(CurrentHealth - amount, 0, maxHealth);
        CurrentPoise  -= poiseDamage;
        _poiseRegenTimer = poiseRegenDelay;

        if (hpSlider)   hpSlider.value  = CurrentHealth;
        if (healthText) healthText.text = CurrentHealth.ToString();

        // Bật blink
        _blinkTimer = blinkDuration;

        Debug.Log($"[Health] HP:{CurrentHealth}/{maxHealth} | Poise:{CurrentPoise:F0}/{maxPoise}");

        if (CurrentHealth <= 0) { Die(); return; }

        if (!IsPhase2 && HealthPercent <= phase2Threshold)
            EnterPhase2();

        if (CurrentPoise <= 0f && !_poiseBroken)
            TriggerPoiseBroken();
        else if (CurrentPoise > 0f)
            _anim.SetTrigger(paramHit);
    }

    // ── Poise ─────────────────────────────────────────────────
    void RegeneratePoise()
    {
        if (_poiseRegenTimer > 0f) { _poiseRegenTimer -= Time.deltaTime; return; }
        if (CurrentPoise < maxPoise)
        {
            CurrentPoise = Mathf.Min(CurrentPoise + poiseRegenRate * Time.deltaTime, maxPoise);
            _poiseBroken = false;
        }
    }

    void TriggerPoiseBroken()
    {
        _poiseBroken = true;
        CurrentPoise = maxPoise * 0.5f;
        _anim.SetTrigger(paramHit);
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
            Debug.LogWarning("[WolfbossHealth] Không tìm thấy Manager.Instance!");
    }
}