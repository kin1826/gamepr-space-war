using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    [Header("Stats")]
    public float maxHealth = 200f;

    [Header("Death — Ragdoll Prefab")]
    public GameObject ragdollPrefab;
    public float ragdollLifetime = 4f;

    [Header("UI")]
    public Canvas hpCanvas;
    public Slider hpSlider;

    [Header("Hit Blink Effect")]
    public float blinkIntensity = 1f;   // 0→1, 1 = trắng hoàn toàn
    public float blinkDuration  = 0.12f;

    [HideInInspector] public bool   isDead;
    [HideInInspector] public Action onDeath;

    private float        _currentHealth;
    private Camera       _cam;
    private AILocomotion _ai;

    // Blink
    private Renderer[]   _renderers;
    private Color[]      _originalColors;
    private Coroutine    _blinkCoroutine;

    void Start()
    {
        _ai            = GetComponent<AILocomotion>();
        _cam           = Camera.main;
        _currentHealth = maxHealth;

        if (hpSlider) { hpSlider.maxValue = maxHealth; hpSlider.value = maxHealth; }
        if (hpCanvas) hpCanvas.gameObject.SetActive(false);

        // Cache tất cả renderer + màu gốc
        // Chỉ lấy renderer có shader support _Color (tránh Unlit/Texture)
        var allRenderers = GetComponentsInChildren<Renderer>();
        var validRenderers = new System.Collections.Generic.List<Renderer>();
        var validColors    = new System.Collections.Generic.List<Color>();

        foreach (var r in allRenderers)
        {
            if (r.material.HasProperty("_Color"))
            {
                validRenderers.Add(r);
                validColors.Add(r.material.color);
            }
        }

        _renderers      = validRenderers.ToArray();
        _originalColors = validColors.ToArray();
    }

    void LateUpdate()
    {
        if (hpCanvas == null || _cam == null) return;
        hpCanvas.transform.forward = _cam.transform.forward;
        Vector3 dir     = (transform.position - _cam.transform.position).normalized;
        bool    visible = Vector3.Dot(_cam.transform.forward, dir) > 0.5f;
        hpCanvas.gameObject.SetActive(visible && !isDead);
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;
        _currentHealth = Mathf.Clamp(_currentHealth - damage, 0f, maxHealth);
        if (hpSlider) hpSlider.value = _currentHealth;
        if (hpCanvas) hpCanvas.gameObject.SetActive(true);

        // Blink khi bị hit
        if (_blinkCoroutine != null) StopCoroutine(_blinkCoroutine);
        _blinkCoroutine = StartCoroutine(BlinkEffect());

        if (_currentHealth <= 0f) Die();
    }

    IEnumerator BlinkEffect()
    {
        // Flash trắng
        for (int i = 0; i < _renderers.Length; i++)
            _renderers[i].material.color = Color.Lerp(_originalColors[i], Color.white, blinkIntensity);

        // Fade về màu gốc
        float elapsed = 0f;
        while (elapsed < blinkDuration)
        {
            float t = elapsed / blinkDuration;
            for (int i = 0; i < _renderers.Length; i++)
                _renderers[i].material.color = Color.Lerp(
                    Color.Lerp(_originalColors[i], Color.white, blinkIntensity),
                    _originalColors[i], t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Reset về màu gốc
        for (int i = 0; i < _renderers.Length; i++)
            _renderers[i].material.color = _originalColors[i];

        _blinkCoroutine = null;
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        if (_blinkCoroutine != null) { StopCoroutine(_blinkCoroutine); _blinkCoroutine = null; }

        // Reset màu về gốc trước khi chết
        for (int i = 0; i < _renderers.Length; i++)
            _renderers[i].material.color = _originalColors[i];

        _ai?.OnDead();

        if (hpCanvas) hpCanvas.gameObject.SetActive(false);
        hpCanvas = null;

        if (ragdollPrefab != null)
        {
            GameObject ragdoll = Instantiate(ragdollPrefab, transform.position, transform.rotation);
            Destroy(ragdoll, ragdollLifetime);
        }

        onDeath?.Invoke();
        Destroy(gameObject);
    }
}