using UnityEngine;

public class EnemyReward : MonoBehaviour
{
    [Header("Gold")]
    public int minGold = 1;
    public int maxGold = 3;

    // XP tự tính từ EnemyHealth.maxHealth / 10
    private EnemyHealth _health;

    void Awake()
    {
        _health = GetComponent<EnemyHealth>();
        if (_health) _health.onDeath += GiveReward;
    }

    void OnDestroy()
    {
        if (_health) _health.onDeath -= GiveReward;
    }

    private void GiveReward()
    {
        int gold = Random.Range(minGold, maxGold + 1);
        int xp   = _health ? Mathf.Max(1, Mathf.RoundToInt(_health.maxHealth / 10f)) : 1;

        SessionReward.Instance?.Add(gold, xp);
    }
}
