using UnityEngine;

/// <summary>
/// Quản lý toàn bộ sound của Skeleton.
/// Gắn lên root Skeleton GameObject.
///
/// Setup:
///   1. Add 2 AudioSource lên Skeleton:
///      - audioSFX   : Play One Shot, không loop (cho die, slice)
///      - audioFootstep: Loop, volume nhỏ hơn (cho footstep)
///   2. Kéo các AudioClip vào Inspector
///   3. Thêm Animation Event vào clip:
///      - Slash01/02/Stab → frame vung kiếm → PlaySwordSlice()
///      - Walk/Run → mỗi bước chân → PlayFootstep()
///      - Death → frame đầu → PlayDie()
/// </summary>
public class SkeletonSound : MonoBehaviour
{
    [Header("Audio Sources")]
    [Tooltip("Dùng cho SFX 1 lần: die, sword slice")]
    public AudioSource audioSFX;
    [Tooltip("Dùng riêng cho footstep để dễ kiểm soát volume")]
    public AudioSource audioFootstep;

    [Header("Clips")]
    public AudioClip clipDie;
    public AudioClip clipFootstep;
    public AudioClip clipSwordSlice;

    [Header("Volume")]
    [Range(0f, 1f)] public float volumeDie       = 1f;
    [Range(0f, 1f)] public float volumeFootstep  = 0.5f;
    [Range(0f, 1f)] public float volumeSwordSlice = 0.8f;

    [Header("Footstep")]
    [Tooltip("Thời gian tối thiểu giữa 2 footstep (tránh spam)")]
    public float footstepCooldown = 0.25f;

    private float _footstepTimer;
    private bool  _isDead;

    void Update()
    {
        if (_footstepTimer > 0f)
            _footstepTimer -= Time.deltaTime;
    }

    // ── Gọi từ Animation Event ────────────────────────────────

    /// Thêm vào clip: Walk, Run — mỗi bước chân
    public void PlayFootstep()
    {
        if (_isDead) return;
        if (clipFootstep == null) return;
        if (_footstepTimer > 0f) return; // tránh spam

        _footstepTimer = footstepCooldown;

        if (audioFootstep != null)
            audioFootstep.PlayOneShot(clipFootstep, volumeFootstep);
        else if (audioSFX != null)
            audioSFX.PlayOneShot(clipFootstep, volumeFootstep);
    }

    /// Thêm vào clip: Slash01, Slash02, Stab — frame vung kiếm
    public void PlaySwordSlice()
    {
        if (_isDead) return;
        if (clipSwordSlice == null || audioSFX == null) return;
        audioSFX.PlayOneShot(clipSwordSlice, volumeSwordSlice);
    }

    /// Thêm vào clip: Death — frame đầu tiên
    public void PlayDie()
    {
        if (clipDie == null || audioSFX == null) return;
        _isDead = true;
        audioSFX.PlayOneShot(clipDie, volumeDie);

        // Tắt footstep khi chết
        if (audioFootstep != null && audioFootstep.isPlaying)
            audioFootstep.Stop();
    }

    // ── Gọi từ code nếu không dùng Animation Event ───────────
    public void OnDead() => PlayDie();
}