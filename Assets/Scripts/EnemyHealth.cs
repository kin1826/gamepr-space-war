using System;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    [Header("Stats")]
    public float maxHealth = 200f;
    [Tooltip("Thời gian chờ trước khi destroy (để animation death kịp chạy)")]
    public float destroyDelay = 3f;

    [Header("UI")]
    public Canvas hpCanvas;
    public Slider hpSlider;

    [Header("Death Animation")]
    [Tooltip("Tên trigger trong Animator — phải khớp với parameter trong Animator Controller")]
    public string deathTrigger = "Death";
    [Tooltip("Delay trước khi bật ragdoll (giây) — để animation death chạy trước)")]
    public float ragdollDelay  = 1.5f;

    [HideInInspector] public bool   isDead;
    [HideInInspector] public Action onDeath;

    private float          _currentHealth;
    private Camera         _cam;
    private RagdollManager _ragdoll;
    private Animator       _anim;
    private AILocomotion   _ai;

    void Start()
    {
        _ragdoll       = GetComponent<RagdollManager>();
        _anim          = GetComponent<Animator>();
        _ai            = GetComponent<AILocomotion>();
        _cam           = Camera.main;
        _currentHealth = maxHealth;

        if (hpSlider)
        {
            hpSlider.maxValue = maxHealth;
            hpSlider.value    = maxHealth;
        }

        if (hpCanvas) hpCanvas.gameObject.SetActive(false);
    }

    void LateUpdate()
    {
        if (hpCanvas == null || _cam == null) return;

        hpCanvas.transform.forward = _cam.transform.forward;

        Vector3 dir     = (transform.position - _cam.transform.position).normalized;
        bool    visible = Vector3.Dot(_cam.transform.forward, dir) > 0.5f;
        hpCanvas.gameObject.SetActive(visible && !isDead);
    }

    public void TakeDamge(float damage)
    {
        if (isDead) return;

        _currentHealth = Mathf.Clamp(_currentHealth - damage, 0f, maxHealth);

        if (hpSlider) hpSlider.value = _currentHealth;
        if (hpCanvas) hpCanvas.gameObject.SetActive(true);

        if (_currentHealth <= 0f) EnemyDeath();
    }

    void EnemyDeath()
    {
        if (isDead) return;
        isDead = true;

        // 1. Ẩn HP bar
        if (hpCanvas) hpCanvas.gameObject.SetActive(false);

        // 2. Dừng AI hoàn toàn
        _ai?.OnDead();

        // 3. Trigger animation Death ngay lập tức
        if (_anim != null)
        {
            _anim.SetTrigger(deathTrigger);
            // Tắt mọi trigger attack đang pending để tránh conflict
            _anim.ResetTrigger("Attack");
        }

        // 4. Bật ragdoll SAU khi animation death chạy xong
        if (_ragdoll != null)
            Invoke(nameof(TriggerRagdollDelayed), ragdollDelay);

        // 5. Fire event & destroy
        onDeath?.Invoke();
        Destroy(gameObject, destroyDelay);
    }

    void TriggerRagdollDelayed()
    {
        if (_ragdoll != null) _ragdoll.TriggerRagdoll();
    }
}