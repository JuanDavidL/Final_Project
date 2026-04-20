using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class ClientSystem : MonoBehaviour
{
    [Header("UI - Monitores (World Space)")]
    [SerializeField] private Image crtMonitorImage;
    [SerializeField] private Image orderScreenImage;
    [SerializeField] private TextMeshProUGUI queueText; 
    [SerializeField] private Image patienceBar;         

    [Header("Datos y Assets")]
    [SerializeField] private Sprite tutorialClientSprite;
    [SerializeField] private ItemData tutorialPotionRequired; 
    [SerializeField] private List<Sprite> normalClientSprites;
    [SerializeField] private List<ItemData> unlockedPotions;  

    [Header("Ajustes de Tiempo y Energía")]
    public MonitorSwitch powerSwitch; // Arrastra tu switch aquí
    [SerializeField] private float timeToNextClient = 2f; // ¡NUEVO! Tiempo entre clientes
    [SerializeField] private float maxPatienceTime = 45f;
    
    [Header("Economía")]
    public int playerCredits = 0;

    // Estados internos
    private bool isTutorialCompleted;
    private int clientsInQueue = 0;
    private bool isClientActive = false;
    private float currentPatience;
    private ItemData currentOrder;
    private float nextClientTimer; // ¡NUEVO! El contador interno

    void Start()
    {
        // Limpiamos datos para pruebas
        PlayerPrefs.DeleteKey("TutorialCompleted"); 
        ClearScreens();

        isTutorialCompleted = PlayerPrefs.GetInt("TutorialCompleted", 0) == 1;

        if (!isTutorialCompleted)
        {
            // El tutorial se activa solo al encender el monitor por primera vez
            Debug.Log("Esperando encendido del monitor para iniciar Tutorial.");
        }
        else
        {
            // Cargamos unos clientes iniciales en la fila
            clientsInQueue = Random.Range(2, 5);
            UpdateQueueUI();
        }
    }
    
    void Update()
    {
        
        // 1. EL GRAN FILTRO: Si el monitor está apagado, la lógica se congela
        if (powerSwitch == null || !powerSwitch.IsOn) 
        {
            return; 
        }

        // 2. Si estamos en modo Tutorial y no ha empezado, lo lanzamos
        if (!isTutorialCompleted && !isClientActive)
        {
            //Debug.Log("Iniciando Tutorial porque el Switch está ON."); // Funciona bien
            StartTutorialMode();
            return;
        }

        // 3. Si hay un cliente activo, procesamos su paciencia
        if (isClientActive)
        {
            HandlePatienceTimer();
        }
        // 4. Si NO hay cliente activo, pero hay gente en la fila, esperamos para llamar al siguiente
        else if (clientsInQueue > 0)
        {
             nextClientTimer += Time.deltaTime;
             if (nextClientTimer >= timeToNextClient)
             {
                 StartCoroutine(CallNextClient());
                 nextClientTimer = 0;
             }
        }
    }

    // ==========================================
    // NUEVA FUNCIÓN: Spawneador de Clientes
    // ==========================================
    private void SpawnNewClient()
    {
        // Esta función añade alguien a la fila "invisible"
        AddClientsToQueue(1);
        Debug.Log("Un nuevo cliente ha llegado a la órbita de la nave.");
    }

    // ==========================================
    // RESTO DE TUS MÉTODOS (TUTORIAL Y BUCLE)
    // ==========================================
    
    private void StartTutorialMode()
    {
        isClientActive = true;
        currentOrder = tutorialPotionRequired;

        crtMonitorImage.sprite = tutorialClientSprite;
        crtMonitorImage.color = Color.white;
        
        orderScreenImage.sprite = currentOrder.itemIcon;
        orderScreenImage.color = Color.white;

        patienceBar.fillAmount = 1f;
        patienceBar.color = Color.cyan; 
        queueText.text = "TUTORIAL"; 
    }

    public void CompleteTutorialTrade()
    {
        playerCredits += 50;
        PlayerPrefs.SetInt("TutorialCompleted", 1);
        isTutorialCompleted = true;
        
        clientsInQueue += Random.Range(2, 5); 
        UpdateQueueUI();

        StartCoroutine(ResolveClientAndCheckQueue());
    }

    public void AddClientsToQueue(int amount)
    {
        clientsInQueue += amount;
        UpdateQueueUI();
    }

    private void UpdateQueueUI()
    {
        if (clientsInQueue > 0)
            queueText.text = "x " + clientsInQueue.ToString("D2");
        else
            queueText.text = "";
    }

    private IEnumerator CallNextClient()
    {
        if (isClientActive) yield break; // Seguridad

        yield return new WaitForSeconds(1.5f); 
        clientsInQueue--;
        UpdateQueueUI();
        GenerateDynamicClient();
    }

    private void GenerateDynamicClient()
    {
        isClientActive = true;
        currentPatience = maxPatienceTime;

        crtMonitorImage.sprite = normalClientSprites[Random.Range(0, normalClientSprites.Count)];
        crtMonitorImage.color = new Color(Random.Range(0.6f, 1f), Random.Range(0.6f, 1f), Random.Range(0.6f, 1f), 1f);

        currentOrder = unlockedPotions[Random.Range(0, unlockedPotions.Count)];
        orderScreenImage.sprite = currentOrder.itemIcon;
        orderScreenImage.color = Color.white;
    }

    private void HandlePatienceTimer()
    {
        // No restamos paciencia en el tutorial
        if (!isTutorialCompleted) return;

        currentPatience -= Time.deltaTime;
        float normalizedTime = currentPatience / maxPatienceTime;
        patienceBar.fillAmount = normalizedTime;

        if (normalizedTime > 0.6f) patienceBar.color = Color.green;
        else if (normalizedTime > 0.3f) patienceBar.color = Color.yellow;
        else patienceBar.color = Color.red;

        if (currentPatience <= 0) ProcessDelivery(null);
    }

    public void ProcessDelivery(ItemData potionDelivered)
    {
        if (!isClientActive || potionDelivered == null) return;

        isClientActive = false; 

        if (!isTutorialCompleted)
        {
            if (potionDelivered.id == tutorialPotionRequired.id)
                CompleteTutorialTrade();
            return;
        }

        if (potionDelivered.id == currentOrder.id)
            CalculateDynamicPayout();

        StartCoroutine(ResolveClientAndCheckQueue());
    }

    private void CalculateDynamicPayout()
    {
        int baseValue = 50; 
        float timeRatio = currentPatience / maxPatienceTime;
        int finalPayout = (timeRatio > 0.6f) ? Mathf.RoundToInt(baseValue * 1.2f) : 
                          (timeRatio > 0.3f) ? baseValue : Mathf.RoundToInt(baseValue * 0.5f);

        playerCredits += finalPayout;
    }

    private IEnumerator ResolveClientAndCheckQueue()
    {
        yield return new WaitForSeconds(2f);
        ClearScreens();
        // El Update se encargará de llamar al siguiente si el switch está ON
    }

    private void ClearScreens()
    {
        crtMonitorImage.color = Color.clear;
        orderScreenImage.color = Color.clear;
        patienceBar.fillAmount = 0;
        currentOrder = null;
    }
}