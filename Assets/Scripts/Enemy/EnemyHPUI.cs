using UnityEngine;
using UnityEngine.UI;

public class EnemyHPUI : MonoBehaviour
{
    [Header("UI")]
    public Canvas hpCanvas;

    public Image hpFill;

    [Header("Health")]
    public float maxHP = 200f;

    private float currentHP;

    private Camera cam;

    void Start()
    {
        cam = Camera.main;

        currentHP = maxHP;

        UpdateHP();

        hpCanvas.gameObject.SetActive(false);
    }

    void LateUpdate()
    {
        if (cam == null)
            return;

        // 🚀 nhìn camera
        hpCanvas.transform.forward =
            cam.transform.forward;

        // 🚀 player nhìn enemy?
        Vector3 dir =
            (transform.position - cam.transform.position)
            .normalized;

        float dot =
            Vector3.Dot(
                cam.transform.forward,
                dir
            );

        // 🎯 nếu nằm trong góc nhìn
        bool visible = dot > 0.5f;

        hpCanvas.gameObject.SetActive(visible);
    }

    public void TakeDamage(float damage)
    {
        currentHP -= damage;

        currentHP =
            Mathf.Clamp(
                currentHP,
                0,
                maxHP
            );

        UpdateHP();

        // 🚀 hiện HP khi bị hit
        hpCanvas.gameObject.SetActive(true);

        if (currentHP <= 0)
        {
            Destroy(gameObject);
        }
    }

    void UpdateHP()
    {
        hpFill.fillAmount =
            currentHP / maxHP;
    }
}