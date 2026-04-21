using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClientSystem : MonoBehaviour
{
    [Header("UI - Monitores (World Space)")]
    [SerializeField]
    private Image crtMonitorImage;

    [SerializeField]
    private Image orderScreenImage;

    [SerializeField]
    private TextMeshProUGUI queueText;

    [SerializeField]
    private Image patienceBar;

    [Header("Datos y Assets")]
    [SerializeField]
    private Sprite tutorialClientSprite;

    [SerializeField]
    private ItemData tutorialPotionRequired;

    [SerializeField]
    private List<Sprite> normalClientSprites;

    [SerializeField]
    private List<ItemData> unlockedPotions;

    [Header("Ajustes de Tiempo y Energía")]
    public MonitorSwitch powerSwitch; // Arrastra tu switch aquí

    [SerializeField]
    //private float timeToNextClient = 2f;
    private float maxPatienceTime = 45f;

    [Header("Economía")]
    public int playerCredits = 0;

    // Estados internos
    private bool isTutorialCompleted;
    private int clientsInQueue = 0;
    private bool isClientActive = false;
    private float currentPatience;
    private ItemData currentOrder;
    private float nextClientTimer; // ¡NUEVO! El contador interno
    private bool isCallingNext = false; // El semáforo

    void Start()
    {
        // Usamos una pequeña comprobación de seguridad
        if (GameManager.Instance != null)
        {
            isTutorialCompleted = GameManager.Instance.isTutorialCompleted;
        }
        else
        {
            // Si por alguna razón el Singleton no está listo, lo leemos del disco directamente
            isTutorialCompleted = PlayerPrefs.GetInt("TutorialCompleted", 0) == 1;
        }

        ClearScreens();

        if (isTutorialCompleted)
        {
            GenerateSessionClients();
        }
    }

    private void CalculateDynamicPayout()
    {
        int baseValue = 50;
        float timeRatio = currentPatience / maxPatienceTime;
        int finalPayout =
            (timeRatio > 0.6f) ? Mathf.RoundToInt(baseValue * 1.2f)
            : (timeRatio > 0.3f) ? baseValue
            : Mathf.RoundToInt(baseValue * 0.5f);

        // 2. Sumamos al contador global
        GameManager.Instance.AddCredits(finalPayout);
    }

    void Update()
    {
        // Si el monitor está apagado, no hay servicio
        if (powerSwitch == null || !powerSwitch.IsOn)
            return;

        if (!isTutorialCompleted)
        {
            if (!isClientActive && !isCallingNext)
                StartTutorialMode();
            return;
        }

        // Lógica post-tutorial
        if (isClientActive)
        {
            HandlePatienceTimer();
        }
        else if (clientsInQueue > 0 && !isCallingNext)
        {
            // Esto se disparará apenas cargue la escena si clientsInQueue > 0
            StartCoroutine(CallNextClient());
        }
    }

    public void CompleteTutorialTrade()
    {
        // 1. Actualizamos el Singleton persistente
        GameManager.Instance.isTutorialCompleted = true;
        GameManager.Instance.totalCredits += 50;

        // 2. Forzamos el guardado físico en disco inmediatamente
        GameManager.Instance.SaveGlobalProgress();

        isTutorialCompleted = true;
        clientsInQueue = 0; // O la cantidad que desees
        UpdateQueueUI();

        StartCoroutine(ResolveClientAndCheckQueue());
    }

    public ItemData GetCurrentOrder()
    {
        // 1. Si NO hemos pasado el tutorial, el tubo debe buscar la poción del tutorial
        if (!isTutorialCompleted)
        {
            return tutorialPotionRequired;
        }

        // 2. Si ya estamos en el flujo normal, devuelve la orden del cliente actual
        return currentOrder;
    }

    private void GenerateSessionClients()
    {
        // Esto solo ocurre UNA VEZ al cargar la escena de la nave
        clientsInQueue = Random.Range(3, 6);
        UpdateQueueUI();
        Debug.Log($"Bienvenido de vuelta. Hay {clientsInQueue} clientes en espera.");
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
        isCallingNext = true;
        yield return new WaitForSeconds(1.5f);

        if (clientsInQueue > 0)
        {
            clientsInQueue--;
            UpdateQueueUI();
            GenerateDynamicClient();
        }

        isCallingNext = false;
    }

    private void GenerateDynamicClient()
    {
        // VALIDACIÓN CRÍTICA: Si no hay pociones o sprites, el sistema se rompe
        if (normalClientSprites.Count == 0 || unlockedPotions.Count == 0)
        {
            Debug.LogError("¡Error! No hay sprites o pociones asignadas en el Inspector.");
            return;
        }

        isClientActive = true;
        currentPatience = maxPatienceTime;

        // Asignar Sprite
        crtMonitorImage.sprite = normalClientSprites[Random.Range(0, normalClientSprites.Count)];
        crtMonitorImage.color = Color.white; // Aseguramos que no sea transparente

        // Asignar Orden
        currentOrder = unlockedPotions[Random.Range(0, unlockedPotions.Count)];
        orderScreenImage.sprite = currentOrder.itemIcon;
        orderScreenImage.color = Color.white;
    }

    private void HandlePatienceTimer()
    {
        // No restamos paciencia en el tutorial
        if (!isTutorialCompleted)
            return;

        currentPatience -= Time.deltaTime;
        float normalizedTime = currentPatience / maxPatienceTime;
        patienceBar.fillAmount = normalizedTime;

        if (normalizedTime > 0.6f)
            patienceBar.color = Color.green;
        else if (normalizedTime > 0.3f)
            patienceBar.color = Color.yellow;
        else
            patienceBar.color = Color.red;

        if (currentPatience <= 0)
            ProcessDelivery(null);
    }

    public void ProcessDelivery(ItemData potionDelivered)
    {
        if (!isClientActive || potionDelivered == null)
            return;

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

    // --- MODIFICACIÓN EN LA RESOLUCIÓN ---
    private IEnumerator ResolveClientAndCheckQueue()
    {
        isCallingNext = true;
        yield return new WaitForSeconds(2f); // Tiempo del feedback de "Vendido"

        ClearScreens();
        isClientActive = false; // <--- IMPORTANTE: Aquí marcamos que ya no hay nadie

        if (clientsInQueue > 0)
        {
            yield return new WaitForSeconds(1f);
            clientsInQueue--;
            UpdateQueueUI();
            GenerateDynamicClient();
            isCallingNext = false;
        }
        else
        {
            UpdateQueueUI();
            isCallingNext = false;

            // Ahora sí, el IsShiftComplete será True y el tubo mostrará el mensaje
            DeliveryTube3D tube = FindObjectOfType<DeliveryTube3D>();
            if (tube != null)
                tube.SendMessage("UpdateVisuals");
        }
    }

    private void ClearScreens()
    {
        crtMonitorImage.color = Color.clear;
        orderScreenImage.color = Color.clear;
        patienceBar.fillAmount = 0;
        currentOrder = null;
    }

    // --- EN TU SCRIPT ClientSystem.cs ---

    public bool IsShiftComplete()
    {
        // El día termina SOLO si:
        // 1. Ya pasó el tutorial.
        // 2. No hay nadie en la fila.
        // 3. NO hay un cliente siendo atendido actualmente (isClientActive).
        // 4. No estamos en proceso de llamar a otro (isCallingNext).

        return isTutorialCompleted && clientsInQueue == 0 && !isClientActive && !isCallingNext;
    }
}
