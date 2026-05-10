using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class WolfbossAttack3 : MonoBehaviour
{
    [Header("Animator")]
    public string paramAttack3 = "Attack3";

    [Header("Optional")]
    public ParticleSystem howlVFX;
    public AudioSource    howlSFX;

    // ── Public ────────────────────────────────────────────────
    public bool IsHowling { get; private set; }

    // ── Private ───────────────────────────────────────────────
    private WolfbossAttackData _data;
    private Animator           _anim;
    private WolfbossHealth     _health;
    private WolfbossAttack1    _atk1;

    void Awake()
    {
        _anim   = GetComponent<Animator>();
        _health = GetComponent<WolfbossHealth>();
        _atk1   = GetComponentInChildren<WolfbossAttack1>();
    }

    public void Init(WolfbossAttackData data) => _data = data;

    // ── Gọi từ WolfbossAI ─────────────────────────────────────
    public void StartHowl()
    {
        if (IsHowling || _data == null) return;
        StartCoroutine(HowlSequence());
    }

    IEnumerator HowlSequence()
    {
        IsHowling = true;
        _anim.SetTrigger(paramAttack3);
        if (howlVFX != null) howlVFX.Play();
        if (howlSFX != null) howlSFX.Play();

        Debug.Log("[Attack3] Boss GẦMM!");

        // Phase 2 — buff damage Attack1
        if (_health != null && _health.IsPhase2 && _atk1 != null && _data != null)
        {
            _atk1.ApplyDamageBuff(_data.atk3BuffMultiplier, _data.atk3Duration);
            Debug.Log($"[Attack3] Phase2 buff x{_data.atk3BuffMultiplier}!");
        }

        yield return new WaitForSeconds(_data != null ? _data.atk3Duration : 1.8f);

        IsHowling = false;
    }
}