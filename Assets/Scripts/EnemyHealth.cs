using System;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    [Header("Stats")]
    public float maxHealth = 200f;
    [Tooltip("Thời gian chờ trước khi destroy (để ragdoll kịp chạy)")]
    public float destroyDelay = 3f;

    [Header("UI")]
    public Canvas hpCanvas;
    public Slider hpSlider;

    [HideInInspector] public bool isDead;
    [HideInInspector] public Action onDeath;

    private float _currentHealth;
    private Camera _cam;
    private RagdollManager _ragdoll;

    void Start()
    {
        _ragdoll       = GetComponent<RagdollManager>();
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

        // Canvas luôn quay mặt về phía camera
        hpCanvas.transform.forward = _cam.transform.forward;

        // Chỉ hiện khi enemy nằm trong tầm nhìn
        Vector3 dir = (transform.position - _cam.transform.position).normalized;
        bool visible = Vector3.Dot(_cam.transform.forward, dir) > 0.5f;
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
        isDead = true;
        if (hpCanvas) hpCanvas.gameObject.SetActive(false);
        if (_ragdoll) _ragdoll.TriggerRagdoll();
        onDeath?.Invoke();
        Destroy(gameObject, destroyDelay);
    }
}
