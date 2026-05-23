using UnityEngine;

public class SkeletonKnife : MonoBehaviour
{
    public int   damage   = 20;
    public float speed    = 12f;
    public float lifetime = 3f;

    private Vector3 _direction;
    private bool    _hasHit;
    private bool    _launched; // ← thêm flag này

    public void Launch(Vector3 direction, int dmg, float spd, float life)
    {
        _direction = direction.normalized;
        damage     = dmg;
        speed      = spd;
        _launched  = true; // ← set true khi launch
        Destroy(gameObject, life);
    }

    void Update()
    {
        if (!_launched) return; // ← chưa launch thì không làm gì

        transform.position += _direction * speed * Time.deltaTime;

        // Tránh lỗi Look rotation viewing vector is zero
        if (_direction.sqrMagnitude > 0.01f)
            transform.rotation = Quaternion.LookRotation(_direction);
    }

    void OnTriggerEnter(Collider other)
    {
        if (_hasHit) return;

        var ph = other.GetComponent<PlayerHealth>();
        if (ph != null)
        {
            _hasHit = true;
            ph.TakeDamage(damage);
            Debug.Log($"[Knife] Trúng {other.name}, dmg={damage}");
            Destroy(gameObject);
        }
        else if (!other.isTrigger)
        {
            Destroy(gameObject);
        }
    }
}