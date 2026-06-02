using System;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] float timeToDestroy;
    [HideInInspector] public WeaponManager weapon;
    [HideInInspector] public Vector3 dir;

    void Start()
    {
        Destroy(this.gameObject, timeToDestroy);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"[Bullet] Va chạm với: {collision.gameObject.name} | Layer: {LayerMask.LayerToName(collision.gameObject.layer)}");
        var target = collision.gameObject.GetComponentInParent<IDamageable>();
        Debug.Log($"[Bullet] IDamageable tìm thấy: {(target != null ? target.GetType().Name : "NULL")}");
        if (target != null)
        {
            target.TakeDamage(weapon.damage);

            // Kickback chỉ áp dụng cho EnemyHealth (có ragdoll)
            var enemyHealth = target as EnemyHealth;
            if (enemyHealth != null && enemyHealth.isDead)
            {
                Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
                if (rb) rb.AddForce(dir * weapon.enemyKickbackForce, ForceMode.Impulse);
            }
        }
        Destroy(this.gameObject);
    }
}
