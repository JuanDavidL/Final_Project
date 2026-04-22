using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Progreso Global")]
    public bool isTutorialCompleted;
    public int totalCredits;

    [Header("Mejoras Compradas")]
    public int fireballUpgradesPurchased = 0;
    public int frostNovaUpgradesPurchased = 0;

    void Awake()
    {
        // Regla de oro del Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadGlobalProgress(); // Opcional: Cargar de PlayerPrefs al iniciar
            ResetUpgrades();
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

        fireballUpgradesPurchased = PlayerPrefs.GetInt("FireballUpgradesPurchased", 0);
        frostNovaUpgradesPurchased = PlayerPrefs.GetInt("FrostNovaUpgradesPurchased", 0);
        //Debug.Log($"LoadGlobalProgress → Creditos: {totalCredits} | Fireball: {fireballUpgradesPurchased} | FrostNova: {frostNovaUpgradesPurchased}");
    }

    public void SaveGlobalProgress()
    {
        PlayerPrefs.SetInt("TutorialCompleted", isTutorialCompleted ? 1 : 0);
        PlayerPrefs.SetInt("TotalCredits", totalCredits);
        PlayerPrefs.SetInt("FireballUpgradesPurchased", fireballUpgradesPurchased);
        PlayerPrefs.SetInt("FrostNovaUpgradesPurchased", frostNovaUpgradesPurchased);
        PlayerPrefs.Save();
    }

    void Start()
    {
        // Ejemplo de cómo usar AddCredits (puedes eliminar esto después de probar)
        
    }

    [ContextMenu("Reset Progress")]
    public void ResetUpgrades()
    {
        fireballUpgradesPurchased = 0;
        frostNovaUpgradesPurchased = 0;
        totalCredits = 10000; // créditos de prueba
        SaveGlobalProgress(); // ✅ guarda el reset en PlayerPrefs inmediatamente
        //Debug.Log($"Reseteado! Verificando PlayerPrefs → Fireball: {PlayerPrefs.GetInt("FireballUpgrades")} | FrostNova: {PlayerPrefs.GetInt("FrostNovaUpgrades")}");
    }
}