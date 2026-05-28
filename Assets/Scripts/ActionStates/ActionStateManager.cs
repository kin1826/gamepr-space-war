using UnityEngine;
using UnityEngine.Animations.Rigging;

public class ActionStateManager : MonoBehaviour
{
    [HideInInspector] public ActionBaseState currentState;
    public ReloadState Reload = new ReloadState();
    public DefaultState Default = new DefaultState();

    [HideInInspector] public WeaponManager currentWeapon;
    [HideInInspector] public WeaponAmmo ammo;
    AudioSource audioSource;

    [HideInInspector] public Animator anim;

    public MultiAimConstraint rHandAim;
    public TwoBoneIKConstraint lHandIK;

    void Start()
    {
        SwitchState(Default);
        anim = GetComponent<Animator>();

        // ĐÃ XÓA đoạn lấy ammo và audioSource trực tiếp từ currentWeapon ở đây.
        // Các biến này sẽ tự động được gán thông qua hàm SetWeapon() bên dưới khi đổi súng.
    }

    void Update()
    {
        if (currentState != null)
        {
            currentState.UpdateState(this);
        }
    }

    public void SwitchState(ActionBaseState state)
    {
        currentState = state;
        if (currentState != null)
        {
            currentState.EnterState(this);
        }
    }

    public void WeaponReloaded()
    {
        if (ammo != null) ammo.Reload();
        SwitchState(Default);
    }

    public void MagOut()
    {
        if (audioSource != null && ammo != null && ammo.magOutSound != null)
        {
            audioSource.PlayOneShot(ammo.magOutSound);
        }
    }

    public void MagIn()
    {
        if (audioSource != null && ammo != null && ammo.magInSound != null)
        {
            audioSource.PlayOneShot(ammo.magInSound);
        }
    }

    public void ReleaseSlide()
    {
        if (audioSource != null && ammo != null && ammo.releaseSlideSound != null)
        {
            audioSource.PlayOneShot(ammo.releaseSlideSound);
        }
    }

    // Hàm quan trọng để thiết lập thông số mỗi khi người chơi cầm vũ khí lên
    public void SetWeapon(WeaponManager weapon)
    {
        if (weapon == null) return;

        currentWeapon = weapon;
        audioSource = weapon.audioSource;
        ammo = weapon.ammo;
    }
}