using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class EndingManager : MonoBehaviour
{
    public static EndingManager Instance;

    [Header("Quinta Esencia")]
    public ItemData quintessenceItem;

    [Header("Canvas Final")]
    public GameObject endingPanel;
    public Button buttonPlayAgain;
    public Button buttonContinue;

    [Header("Escena")]
    public string shipSceneName = "Nave";

    private bool _endingTriggered = false;

    void Awake()
    {
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

    void Start()
    {
        endingPanel.SetActive(false);
        buttonPlayAgain.onClick.AddListener(OnPlayAgain);
        buttonContinue.onClick.AddListener(OnContinue);
    }

    // ─── Detección ────────────────────────────────────────────────

    public void CheckForQuintessence(ItemData item)
    {
         Debug.Log($"CheckForQuintessence llamado con: {(item == null ? "NULL" : item.itemName)} | ID: {item?.id}");
    


        if (_endingTriggered) return;
        if (item == null) return;

        Debug.Log($"Comparando → id == 15: {item.id == 15} | nombre: {item.itemName == "Quintessence Elixir"}");


        if (item.id == 15)
        {
            _endingTriggered = true;
            StartCoroutine(TriggerEnding());
        }
    }

    private IEnumerator TriggerEnding()
    {
        // ✅ Activa el Canvas directamente
        endingPanel.SetActive(true);
        yield break;
    }

    // ─── Botones ──────────────────────────────────────────────────

    public void OnPlayAgain()
    {
        // ✅ Borra TODO el progreso
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        if (GameManager.Instance != null)
        {
            GameManager.Instance.totalCredits = 0;
            GameManager.Instance.fireballUpgradesPurchased = 0;
            GameManager.Instance.frostNovaUpgradesPurchased = 0;
            GameManager.Instance.isTutorialCompleted = false;
            GameManager.Instance.hasGameStarted = false;
            GameManager.Instance.selectedPlanetIndex = 0;
        }

        if (InventoryManager.Instance != null)
            InventoryManager.Instance.inventory.Clear();

        _endingTriggered = false;
        endingPanel.SetActive(false);

        SceneManager.LoadScene(shipSceneName);
    }

    public void OnContinue()
    {
        // ✅ Cierra el panel y sigue jugando
        _endingTriggered = false;
        endingPanel.SetActive(false);
    }
}