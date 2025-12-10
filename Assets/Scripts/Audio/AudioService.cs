using System.Collections.Generic;
using UnityEngine;

public enum SFXType
{
    None,
    PlayerMovement,
    PlayerAttack,
    PlayerJump,
    PlayerDash,
    PlayerHurt,
    PlayerDeath,
    EnemyHit,
    BoxfishBubble,
    EelBiting,
    PlayerDoubleJump,
    JellyfishTrampoline,
    PropBreaking,
    Checkpoint
}

public enum MusicType
{
    None,
    Area01,
    Area02,
    Area03,
    Area04,
    LionFishFight
}

public class AudioService : MonoBehaviour
{
    public static AudioService Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource sfxLoopSource;

    [Header("Music Fade Settings")]
    [SerializeField] private float musicFadeDuration = 0.5f;

    [Header("SFX List")]
    [SerializeField] private List<SFXEntry> sfxList = new List<SFXEntry>();

    [Header("Music List")]
    [SerializeField] private List<MusicEntry> musicList = new List<MusicEntry>();

    private Coroutine musicFadeRoutine;

    [System.Serializable]
    public class SFXEntry
    {
        public SFXType type;
        public AudioClip clip;
        [Range(0f, 1f)] public float volume = 1f;
    }

    [System.Serializable]
    public class MusicEntry
    {
        public MusicType type;
        public AudioClip clip;
        [Range(0f, 1f)] public float volume = 1f;
    }

    private Dictionary<SFXType, SFXEntry> sfxDict;
    private Dictionary<MusicType, MusicEntry> musicDict;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = true;
        }

        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.loop = false;
        }

        if (sfxLoopSource == null)
        {
            sfxLoopSource = gameObject.AddComponent<AudioSource>();
            sfxLoopSource.loop = true;
        }

        musicSource.volume = 1f;
        sfxSource.volume = 1f;

        BuildDictionaries();
    }

    private void BuildDictionaries()
    {
        sfxDict = new Dictionary<SFXType, SFXEntry>();
        foreach (var entry in sfxList)
        {
            if (entry == null || entry.clip == null) continue;
            if (entry.type == SFXType.None) continue;

            if (!sfxDict.ContainsKey(entry.type))
                sfxDict.Add(entry.type, entry);
        }

        musicDict = new Dictionary<MusicType, MusicEntry>();
        foreach (var entry in musicList)
        {
            if (entry == null || entry.clip == null) continue;
            if (entry.type == MusicType.None) continue;

            if (!musicDict.ContainsKey(entry.type))
                musicDict.Add(entry.type, entry);
        }
    }

    public void PlaySFX(SFXType type)
    {
        if (type == SFXType.None) return;
        if (sfxSource == null || sfxDict == null) return;

        if (sfxDict.TryGetValue(type, out SFXEntry entry))
        {
            float vol = Mathf.Clamp01(entry.volume);
            sfxSource.PlayOneShot(entry.clip, vol);
        }
        else
        {
            Debug.LogWarning($"AudioService: No SFX mapped for type {type}");
        }
    }

    public void PlayLoop(SFXType type)
    {
        if (type == SFXType.None) return;
        if (sfxLoopSource == null || sfxDict == null) return;

        if (!sfxDict.TryGetValue(type, out SFXEntry entry) || entry.clip == null)
            return;

        if (sfxLoopSource.clip == entry.clip && sfxLoopSource.isPlaying)
            return;

        sfxLoopSource.clip = entry.clip;
        sfxLoopSource.volume = Mathf.Clamp01(entry.volume);
        sfxLoopSource.loop = true;
        sfxLoopSource.Play();
    }

    public void StopLoop(SFXType type)
    {
        if (sfxLoopSource == null) return;

        if (sfxDict != null && sfxDict.TryGetValue(type, out SFXEntry entry) && entry.clip != null)
        {
            if (sfxLoopSource.clip == entry.clip && sfxLoopSource.isPlaying)
                sfxLoopSource.Stop();
        }
        else
            sfxLoopSource.Stop();
    }

    public void PlayMusic(MusicType type)
    {
        if (musicSource == null || musicDict == null) return;

        if (type == MusicType.None)
        {
            if (musicFadeRoutine != null)
                StopCoroutine(musicFadeRoutine);

            musicFadeRoutine = StartCoroutine(FadeOutAndStop());
            return;
        }

        if (!musicDict.TryGetValue(type, out MusicEntry newEntry))
        {
            Debug.LogWarning($"AudioService: No Music mapped for type {type}");
            return;
        }

        if (musicSource.clip == newEntry.clip && musicSource.isPlaying) return;

        if (musicFadeRoutine != null)
            StopCoroutine(musicFadeRoutine);

        musicFadeRoutine = StartCoroutine(FadeToNewMusic(newEntry));
    }

    public void StopMusic()
    {
        if (musicSource == null) return;

        if (musicFadeRoutine != null)
            StopCoroutine(musicFadeRoutine);

        musicFadeRoutine = StartCoroutine(FadeOutAndStop());
    }

    private System.Collections.IEnumerator FadeToNewMusic(MusicEntry newEntry)
    {
        float startVolume = musicSource.volume;
        float t = 0f;
        float duration = Mathf.Max(0.01f, musicFadeDuration);

        while (t < duration)
        {
            t += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(startVolume, 0f, t / duration);
            yield return null;
        }

        musicSource.volume = 0f;

        musicSource.clip = newEntry.clip;
        musicSource.loop = true;
        musicSource.Play();

        float targetVolume = Mathf.Clamp01(newEntry.volume);
        t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(0f, targetVolume, t / duration);
            yield return null;
        }

        musicSource.volume = targetVolume;
        musicFadeRoutine = null;
    }

    private System.Collections.IEnumerator FadeOutAndStop()
    {
        float startVolume = musicSource.volume;
        float t = 0f;
        float duration = Mathf.Max(0.01f, musicFadeDuration);

        while (t < duration)
        {
            t += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(startVolume, 0f, t / duration);
            yield return null;
        }

        musicSource.volume = 0f;
        musicSource.Stop();
        musicSource.clip = null;
        musicFadeRoutine = null;
    }
}
