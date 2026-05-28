using UnityEngine;

[CreateAssetMenu(fileName = "SkeletonData", menuName = "Skeleton/Attack Data")]
public class SkeletonAttackData : ScriptableObject
{
    [Header("Stats")]
    public int   maxHealth       = 100;
    public float moveSpeed       = 3f;
    public float detectionRange  = 10f;

    [Header("Melee")]
    public float meleeRange      = 2f;
    public int   slash1Damage    = 15;
    public int   slash2Damage    = 20;
    public int   stabDamage      = 25;
    public float meleeCooldown   = 1.5f;
    public float slash1Duration  = 0.8f;
    public float slash2Duration  = 1.0f;
    public float stabDuration    = 0.9f;

    [Header("Range — Throw Knife")]
    public float throwRange      = 8f;
    public float throwMinRange   = 3f; // gần quá thì dùng melee
    public int   throwDamage     = 20;
    public float throwCooldown   = 3f;
    public float throwDuration   = 1.2f;
    public float knifeSpeed      = 12f;
    public float knifeLifetime   = 3f;

    [Header("Scream — Buff")]
    public float screamDuration  = 1.5f;
    public float screamSpeedBuff = 1.5f; // tăng tốc sau khi scream
    public float screamBuffTime  = 5f;   // buff kéo dài bao lâu

    [Header("Underground Spawn")]
    public float riseTime        = 1.2f; // thời gian nổi lên
    public float spawnDelay      = 0.5f; // delay trước khi bắt đầu nổi
}