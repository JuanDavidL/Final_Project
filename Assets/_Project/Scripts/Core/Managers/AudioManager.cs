using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Bocinas (Audio Sources)")]
    [SerializeField]
    private AudioSource musicSource;

    [SerializeField]
    private AudioSource sfxSource;

    private void Awake()
    {
        // Patrón Singleton Persistente
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // --- MÉTODOS PARA MÚSICA ---
    public void PlayMusic(AudioClip backgroundMusic)
    {
        // Si ya está sonando esta misma canción, no la reiniciamos
        if (musicSource.clip == backgroundMusic)
            return;

        musicSource.clip = backgroundMusic;
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    // --- MÉTODOS PARA EFECTOS (SFX) ---
    public void PlaySFX(AudioClip clip)
    {
        if (clip == null)
            return;

        sfxSource.PlayOneShot(clip);
    }

    // Método sobrecargado para variar el "Pitch" (tono) y que no suene repetitivo
    public void PlaySFXRandomPitch(AudioClip clip, float minPitch = 0.9f, float maxPitch = 1.1f)
    {
        if (clip == null)
            return;
        sfxSource.pitch = Random.Range(minPitch, maxPitch);
        sfxSource.PlayOneShot(clip);
        sfxSource.pitch = 1f; // Restauramos al tono original
    }
}
