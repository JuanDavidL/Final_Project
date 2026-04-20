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
    private float timeToNextClient = 2f; // ¡NUEVO! Tiempo entre clientes

    [SerializeField]
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
        // 1. Quita los comentarios de estas líneas:
        //PlayerPrefs.DeleteKey("TutorialCompleted");
        //PlayerPrefs.DeleteKey("TotalCredits");
        //PlayerPrefs.Save(); // Forzamos el borrado inmediato

        // 2. Dale al Play una vez.
        // 3. Detén el juego y vuelve a comentar las líneas anteriores.

        isTutorialCompleted = GameManager.Instance.isTutorialCompleted;
        // ...
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
        if (powerSwitch == null || !powerSwitch.IsOn)
            return;

        if (!isTutorialCompleted)
        {
            if (!isClientActive && !isCallingNext) // Añadimos semáforo
                StartTutorialMode();
            return;
        }

        if (isClientActive)
        {
            HandlePatienceTimer();
        }
        // ELIMINAMOS el "else if" que llamaba al cliente desde aquí para evitar el bug
    }

    public void CompleteTutorialTrade()
    {
        // Avisamos al sistema global que el tutorial fue un éxito
        GameManager.Instance.isTutorialCompleted = true;
        GameManager.Instance.AddCredits(50);
        GameManager.Instance.SaveGlobalProgress();

        isTutorialCompleted = true;

        // IMPORTANTE: Dejamos la cola en 0 para que no aparezca nadie más
        clientsInQueue = 0;
        UpdateQueueUI();

        Debug.Log("Tutorial finalizado. Nave en espera de exploración.");
        StartCoroutine(ResolveClientAndCheckQueue());
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
        yield return new WaitForSeconds(2f); // Tiempo para ver el feedback

        ClearScreens(); // Limpiamos la pantalla del cliente viejo
        isClientActive = false;

        // Solo si quedan clientes, llamamos al siguiente DESPUÉS de limpiar
        if (clientsInQueue > 0)
        {
            yield return new WaitForSeconds(0.5f); // Breve pausa dramática
            StartCoroutine(CallNextClient());
        }
        else
        {
            isCallingNext = false;
        }
    }

    private void ClearScreens()
    {
        crtMonitorImage.color = Color.clear;
        orderScreenImage.color = Color.clear;
        patienceBar.fillAmount = 0;
        currentOrder = null;
        // IMPORTANTE: No toques isClientActive aquí, ya lo manejamos en la corrutina
    }
}
