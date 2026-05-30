using System;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    [Header("Stats")]
    public float maxHealth   = 200f;

    [Header("Death — Ragdoll Prefab")]
    [Tooltip("Kéo prefab ragdoll vào đây")]
    public GameObject ragdollPrefab;
    [Tooltip("Thời gian tồn tại của ragdoll trước khi tự xóa (giây)")]
    public float ragdollLifetime = 4f;

    [Header("UI")]
    public Canvas hpCanvas;
    public Slider hpSlider;

    [HideInInspector] public bool   isDead;
    [HideInInspector] public Action onDeath;

    private float        _currentHealth;
    private Camera       _cam;
    private AILocomotion _ai;

    void Start()
    {
        _ai            = GetComponent<AILocomotion>();
        _cam           = Camera.main;
        _currentHealth = maxHealth;

        if (hpSlider) { hpSlider.maxValue = maxHealth; hpSlider.value = maxHealth; }
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
        if (_currentHealth <= 0f) Die();
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        // 1. Dừng AI
        _ai?.OnDead();

        // 2. Ẩn HP bar vĩnh viễn — tắt hẳn không cho LateUpdate bật lại
        if (hpCanvas) hpCanvas.gameObject.SetActive(false);
        hpCanvas = null; // null để LateUpdate bỏ qua hoàn toàn

        // 3. Spawn ragdoll tại đúng vị trí + rotation của AI lúc chết
        if (ragdollPrefab != null)
        {
            GameObject ragdoll = Instantiate(
                ragdollPrefab,
                transform.position,
                transform.rotation);

            // Tự xóa ragdoll sau ragdollLifetime giây
            Destroy(ragdoll, ragdollLifetime);
        }

        // 4. Fire event
        onDeath?.Invoke();

        // 5. Xóa AI gốc ngay lập tức
        Destroy(gameObject);
    }
}