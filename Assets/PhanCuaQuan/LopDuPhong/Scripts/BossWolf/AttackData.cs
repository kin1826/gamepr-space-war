// Assets/PhanCuaQuan/Scripts/BossWolf/WolfbossAttackData.cs
using UnityEngine;

[CreateAssetMenu(fileName = "AttackData", menuName = "Wolfboss/Attack Data")]
public class WolfbossAttackData : ScriptableObject
{
    [Header("Attack 1 — Vồ vuốt")]
    public int   atk1Damage        = 30;
    public float atk1Knockback     = 5f;
    public float atk1Duration      = 1.2f;
    public float atk1Range         = 3.5f;

    [Header("Attack 2 — Lao vào")]
    public int   atk2Damage        = 55;
    public float atk2Knockback     = 10f;
    public float atk2Duration      = 2.5f;
    public float atk2BackupDist    = 2.5f;
    public float atk2BackupTime    = 0.4f;
    public float atk2LungeSpeed    = 16f;
    public float atk2LungeRange    = 9f;
    public float atk2LandRadius    = 2.2f;

    [Header("Attack 3 — Gầm")]
    public float atk3Duration      = 1.8f;
    public float atk3BuffMultiplier = 1.5f; // Phase 2: damage * 1.5

    [Header("Phase 2 — Dưới 30% HP")]
    public float phase2SpeedMult   = 1.4f;  // di chuyển nhanh hơn
    public float phase2CooldownMult = 0.65f; // cooldown ngắn hơn
    public float phase2DamageMult  = 1.35f; // damage cao hơn

    [Header("Distance")]
    public float minAttackRange    = 4f;
    public float maxAttackRange    = 7f;
    public float detectionRange    = 15f;
    public float backoffSpeed      = 3f;
    public float chaseSpeed        = 4f;
    public float attackCooldown    = 2f;
}