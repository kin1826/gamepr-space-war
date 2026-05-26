using Unity.VisualScripting;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [Header("Fire Rate")]
    [SerializeField] float fireRate;  
    [SerializeField] bool semiAuto; 
    float fireRateTimer;

    [Header("Bullet Properties")]
    [SerializeField] GameObject bullet;
    [SerializeField] Transform barrelPos;
    [SerializeField] int bulletsPerShot;
    [SerializeField] float bulletVelocity;
    public float damage = 20;
    AimStateManager aim;

    [SerializeField] AudioClip gunShot;
    [HideInInspector] public AudioSource audioSource;
    [HideInInspector] public WeaponAmmo ammo;
    WeaponBloom bloom;
    ActionStateManager actions;
    WeaponRecoil recoil;

    Light muzzleFlashLight;
    ParticleSystem muzzleFlashParticles;
    float lightIntensity;
    [SerializeField] float lightReturnSpeed = 20;

    public float enemyKickbackForce = 100;

    public Transform leftHandTarget, leftHandHint;
    WeaponClassManager weaponClass;

    void Start()
    {
        aim = GetComponentInParent<AimStateManager>();
        bloom = GetComponent<WeaponBloom>();
        actions = GetComponentInParent<ActionStateManager>();
        
        // Kiểm tra an toàn cho Muzzle Flash Light
        muzzleFlashLight = GetComponentInChildren<Light>();
        if (muzzleFlashLight != null)
        {
            lightIntensity = muzzleFlashLight.intensity;
            muzzleFlashLight.intensity = 0;
        }
        else
        {
            Debug.LogWarning($"[WeaponManager] Thiếu thành phần Light ở các GameObject con của {gameObject.name}");
        }

        // Kiểm tra an toàn cho Muzzle Flash Particles
        muzzleFlashParticles = GetComponentInChildren<ParticleSystem>();
        if (muzzleFlashParticles == null)
        {
            Debug.LogWarning($"[WeaponManager] Thiếu thành phần ParticleSystem ở các GameObject con của {gameObject.name}");
        }

        fireRateTimer = fireRate;
    }

    private void OnEnable()
    {
        if (weaponClass == null)
        {
            weaponClass = GetComponentInParent<WeaponClassManager>();
            ammo = GetComponent<WeaponAmmo>();
            recoil = GetComponent<WeaponRecoil>();
            audioSource = GetComponent<AudioSource>();  
        }

        // Chỉ kích hoạt nếu tìm thấy WeaponClassManager ở công cụ cha
        if (weaponClass != null)
        {
            weaponClass.SetCurrentWeapon(this);
            if (recoil != null)
            {
                recoil.recoilFollowPos = weaponClass.recoiFollowPos; 
            }
        }
        else
        {
            Debug.LogError($"[WeaponManager] Không tìm thấy WeaponClassManager ở các GameObject cha của {gameObject.name}");
        }
    }

    void Update()
    {
        if (ShouldFire()) Fire();
        
        // Chỉ Lerp ánh sáng nếu đèn tồn tại
        if (muzzleFlashLight != null)
        {
            muzzleFlashLight.intensity = Mathf.Lerp(muzzleFlashLight.intensity, 0, lightReturnSpeed * Time.deltaTime);
        }
    }

    bool ShouldFire()
    {
        // Kiểm tra an toàn trước khi truy cập các biến để tránh lỗi Null
        if (ammo == null || actions == null) return false;

        fireRateTimer += Time.deltaTime;
        if (fireRateTimer < fireRate) return false;
        if (ammo.currentAmmo == 0) return false;
        if (actions.currentState == actions.Reload) return false;
        if (semiAuto && Input.GetKeyDown(KeyCode.Mouse0)) return true;
        if (!semiAuto && Input.GetKey(KeyCode.Mouse0)) return true;
        
        return false;
    }

    void Fire()
    {
        fireRateTimer = 0;
        
        if (aim != null && barrelPos != null) barrelPos.LookAt(aim.aimPos);
        if (bloom != null && barrelPos != null) barrelPos.localEulerAngles = bloom.BloomAngle(barrelPos);
        if (audioSource != null && gunShot != null) audioSource.PlayOneShot(gunShot);
        if (recoil != null) recoil.TriggerRecoil();
        
        TriggerMuzzleFlash();
        
        if (ammo != null) ammo.currentAmmo--;

        if (bullet != null && barrelPos != null)
        {
            for (int i = 0; i < bulletsPerShot; i++)
            {
                GameObject currentBullet = Instantiate(bullet, barrelPos.position, barrelPos.rotation);
                Bullet bulletScript = currentBullet.GetComponent<Bullet>();
                if (bulletScript != null)
                {
                    bulletScript.weapon = this; 
                    bulletScript.dir = barrelPos.transform.forward;
                }

                Rigidbody rb = currentBullet.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.AddForce(barrelPos.forward * bulletVelocity, ForceMode.Impulse);
                }
            }
        }
    }

    void TriggerMuzzleFlash()
    {
        if (muzzleFlashParticles != null) muzzleFlashParticles.Play();
        if (muzzleFlashLight != null) muzzleFlashLight.intensity = lightIntensity;
    }
}