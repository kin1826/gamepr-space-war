using System.Collections;
using UnityEngine;

public class SkeletonAttackRange : MonoBehaviour
{
    [Header("Knife Prefab")]
    public GameObject knifePrefab;

    [Header("Throw Point")]
    public Transform throwPoint;

    private SkeletonAttackData _data;
    private Animator           _anim;

    public bool IsThrowing { get; private set; }

    void Awake()
    {
        _anim = GetComponent<Animator>();
    }

    public void Init(SkeletonAttackData data) => _data = data;

    public IEnumerator DoThrow(Transform target)
    {
        IsThrowing = true;
    _anim.SetTrigger("ThrowProjectile");

    yield return new WaitForSeconds(_data.throwDuration * 0.5f);

    if (knifePrefab != null && target != null)
    {
        Vector3 spawnPos  = throwPoint != null
            ? throwPoint.position
            : transform.position + Vector3.up * 1.4f;

        Vector3 targetPos = target.position + Vector3.up * 1.0f;
        Vector3 dir       = (targetPos - spawnPos).normalized;

        // Chỉ dùng hướng bay, rotation của model tự xử lý trong prefab
        GameObject knifeObj = Instantiate(knifePrefab, spawnPos, Quaternion.LookRotation(dir));
        var knife = knifeObj.GetComponent<SkeletonKnife>();
        if (knife != null)
            knife.Launch(dir, _data.throwDamage, _data.knifeSpeed, _data.knifeLifetime);
        else
            Debug.LogError("[Range] Knife Prefab thiếu SkeletonKnife script!");
        }

        yield return new WaitForSeconds(_data.throwDuration * 0.5f);
        IsThrowing = false;
    }
}