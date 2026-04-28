using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Bocinas (Audio Sources)")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadVolumeSettings(); // ✅ carga volumen guardado
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ─── Volumen ──────────────────────────────────────────────────

    public void SetMusicVolume(float value)
    {
        // value viene del slider 0-100, lo convertimos a 0-1
        musicSource.volume = value / 100f;
        PlayerPrefs.SetFloat("MusicVolume", value);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float value)
    {
        sfxSource.volume = value / 100f;
        PlayerPrefs.SetFloat("SFXVolume", value);
        PlayerPrefs.Save();
    }

    public float GetMusicVolume() => musicSource.volume * 100f;
    public float GetSFXVolume() => sfxSource.volume * 100f;

    private void LoadVolumeSettings()
    {
        float music = PlayerPrefs.GetFloat("MusicVolume", 25f);
        float sfx = PlayerPrefs.GetFloat("SFXVolume", 25f);
        musicSource.volume = music / 100f;
        sfxSource.volume = sfx / 100f;
    }

    // ─── Música ───────────────────────────────────────────────────

    public void PlayMusic(AudioClip backgroundMusic)
    {
        if (musicSource.clip == backgroundMusic) return;
        musicSource.clip = backgroundMusic;
        musicSource.Play();
    }

    public void StopMusic() => musicSource.Stop();

    // ─── SFX ──────────────────────────────────────────────────────

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip);
    }

    public void PlaySFXRandomPitch(AudioClip clip, float minPitch = 0.9f, float maxPitch = 1.1f)
    {
        if (clip == null) return;
        sfxSource.pitch = Random.Range(minPitch, maxPitch);
        sfxSource.PlayOneShot(clip);
        sfxSource.pitch = 1f;
    }
}