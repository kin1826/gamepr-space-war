using UnityEngine;

public class WolfbossAnimEvent : MonoBehaviour
{
    [Header("Gắn ClawHitbox vào đây")]
    public WolfbossAttack1 clawAttack;

    // Animation Event gọi các hàm này
    public void EnableHitbox()  => clawAttack.EnableHitbox();
    public void DisableHitbox() => clawAttack.DisableHitbox();
}