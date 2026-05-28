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
        EnemyHealth enemyHealth = collision.gameObject.GetComponentInParent<EnemyHealth>();
        if (enemyHealth != null)
        {
            bool wasDead = enemyHealth.isDead;
            enemyHealth.TakeDamge(weapon.damage);

            // Áp lực kickback khi phát đạn này là phát kết liễu
            if (!wasDead && enemyHealth.isDead)
            {
                Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
                if (rb) rb.AddForce(dir * weapon.enemyKickbackForce, ForceMode.Impulse);
            }
        }
        Destroy(this.gameObject);
    }
}
