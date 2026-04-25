using UnityEngine;

public class SceneMusicTrigger : MonoBehaviour
{
    [Header("Configuración de Audio")]
    [SerializeField]
    private AudioClip sceneMusic;

    void Start()
    {
        // Llamamos a la instancia persistente del AudioManager
        if (AudioManager.Instance != null && sceneMusic != null)
        {
            AudioManager.Instance.PlayMusic(sceneMusic);
        }
    }
}
