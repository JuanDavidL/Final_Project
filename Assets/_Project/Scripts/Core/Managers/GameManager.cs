using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Progreso Global")]
    public bool isTutorialCompleted;
    public int totalCredits;

    void Awake()
    {
        // Regla de oro del Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadGlobalProgress(); // Opcional: Cargar de PlayerPrefs al iniciar
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddCredits(int amount)
    {
        totalCredits += amount;
        Debug.Log($"Créditos actualizados: {totalCredits}");
        // Aquí podrías disparar un evento de UI para que brille el contador
    }

    private void LoadGlobalProgress()
    {
        isTutorialCompleted = PlayerPrefs.GetInt("TutorialCompleted", 0) == 1;
        totalCredits = PlayerPrefs.GetInt("TotalCredits", 0);
    }

    public void SaveGlobalProgress()
    {
        PlayerPrefs.SetInt("TutorialCompleted", isTutorialCompleted ? 1 : 0);
        PlayerPrefs.SetInt("TotalCredits", totalCredits);
        PlayerPrefs.Save();
    }
}