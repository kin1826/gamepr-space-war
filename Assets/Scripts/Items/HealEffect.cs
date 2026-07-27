using UnityEngine;

[CreateAssetMenu(fileName = "HealEffect", menuName = "Item Effects/Heal")]
public class HealEffect : ItemEffect
{
    [Min(1)] public int healAmount = 40;

    public override bool TryApply(GameObject user)
    {
        if (user == null) return false;

        PlayerHealth health = user.GetComponentInParent<PlayerHealth>();
        if (health == null)
            health = user.GetComponentInChildren<PlayerHealth>();

        if (health == null)
        {
            Debug.LogWarning("[HealEffect] Không tìm thấy PlayerHealth trên player.", user);
            return false;
        }

        return health.TryHeal(healAmount);
    }
}
