using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SFXEntry
{
    public string    name;
    public AudioClip clip;
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Music")]
    public List<AudioClip> tracks = new List<AudioClip>();
    public float maxVolume    = 1f;
    public float fadeDuration = 1.5f;

    [Header("SFX")]
    public List<SFXEntry> sfxClips = new List<SFXEntry>();
    [Range(0f, 1f)]
    public float sfxVolume = 1f;

    private AudioSource _source;
    private AudioSource _sfxSource;
    private Coroutine   _fadeRoutine;

    void Awake()
    {
        Instance = this;

        if (FindFirstObjectByType<AudioListener>() == null)
            gameObject.AddComponent<AudioListener>();

        _source        = gameObject.AddComponent<AudioSource>();
        _source.loop   = true;
        _source.volume = 0f;

        _sfxSource        = gameObject.AddComponent<AudioSource>();
        _sfxSource.loop   = false;
        _sfxSource.volume = sfxVolume;
    }

    // Gọi lúc bắt đầu scene để phát track đầu tiên
    public void PlayTrack(int index)
    {
        if (tracks.Count == 0) { Debug.LogWarning("[AudioManager] tracks list is empty!"); return; }
        if (!IsValidIndex(index)) { Debug.LogWarning($"[AudioManager] index {index} out of range (count={tracks.Count})"); return; }

        if (_fadeRoutine != null) StopCoroutine(_fadeRoutine);

        _source.clip = tracks[index];
        _source.Play();
        _fadeRoutine = StartCoroutine(FadeTo(maxVolume));
    }

    // Gọi khi muốn đổi nhạc (ví dụ gặp boss)
    public void SwitchTrack(int index)
    {
        if (!IsValidIndex(index)) return;
        if (_fadeRoutine != null) StopCoroutine(_fadeRoutine);

        _fadeRoutine = StartCoroutine(FadeOutThenIn(index));
    }

    // Fade out nhạc đang chạy (ví dụ khi pause hoặc die)
    public void StopMusic()
    {
        if (_fadeRoutine != null) StopCoroutine(_fadeRoutine);
        _fadeRoutine = StartCoroutine(FadeTo(0f, stopAfter: true));
    }

    private IEnumerator FadeTo(float target, bool stopAfter = false)
    {
        float start = _source.volume;
        float t     = 0f;

        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / fadeDuration;
            _source.volume = Mathf.Lerp(start, target, t);
            yield return null;
        }

        _source.volume = target;
        if (stopAfter) _source.Stop();
    }

    private IEnumerator FadeOutThenIn(int index)
    {
        yield return FadeTo(0f);

        _source.clip = tracks[index];
        _source.Play();

        yield return FadeTo(maxVolume);
    }

    // Gọi SFX theo tên
    public void PlaySFX(string sfxName)
    {
        SFXEntry entry = sfxClips.Find(e => e.name == sfxName);
        if (entry == null || entry.clip == null) { Debug.LogWarning($"[AudioManager] SFX '{sfxName}' not found"); return; }
        _sfxSource.PlayOneShot(entry.clip, sfxVolume);
    }

    // Gọi SFX trực tiếp bằng clip
    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        _sfxSource.PlayOneShot(clip, sfxVolume);
    }

    private bool IsValidIndex(int index) => index >= 0 && index < tracks.Count;
}
