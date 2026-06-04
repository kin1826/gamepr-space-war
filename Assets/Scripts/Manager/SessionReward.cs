using UnityEngine;

public class SessionReward : MonoBehaviour
{
    public static SessionReward Instance;

    public int totalGold { get; private set; }
    public int totalXP   { get; private set; }

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Add(int gold, int xp)
    {
        totalGold += gold;
        totalXP   += xp;
    }

    public void Reset()
    {
        totalGold = 0;
        totalXP   = 0;
    }

    // Gọi sau khi Lobby đã đọc xong
    public static void Consume()
    {
        if (Instance == null) return;
        Destroy(Instance.gameObject);
        Instance = null;
    }
}
