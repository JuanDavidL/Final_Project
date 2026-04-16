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
    [SerializeField] private TextMeshProUGUI queueText; // Texto retro "x 02"
    [SerializeField] private Image patienceBar;         // Barra en modo "Filled"

    [Header("Datos y Assets")]
    [SerializeField] private Sprite tutorialClientSprite;
    [SerializeField] private ItemData tutorialPotionRequired; // Poción de Salud Estelar
    [SerializeField] private List<Sprite> normalClientSprites;
    [SerializeField] private List<ItemData> unlockedPotions;  // Las pociones que ya sabe hacer

    [Header("Ajustes de Tiempo y Economía")]
    [SerializeField] private float maxPatienceTime = 45f;
    public int playerCredits = 0;

    // Estados internos
    private bool isTutorialCompleted;
    private int clientsInQueue = 0;
    private bool isClientActive = false;
    private float currentPatience;
    private ItemData currentOrder;

    void Start()
    {

        // BORRAR ESTA LÍNEA DESPUÉS DE LAS PRUEBAS
        PlayerPrefs.DeleteKey("TutorialCompleted");
        ClearScreens();

        // 1. Verificamos si es el "Día 0" leyendo los datos guardados
        isTutorialCompleted = PlayerPrefs.GetInt("TutorialCompleted", 0) == 1;

        if (!isTutorialCompleted)
        {
            StartTutorialMode();
        }
        else
        {
            // Modo Normal: Simulamos que trajimos clientes de la expedición
            AddClientsToQueue(Random.Range(2, 5));
        }
    }
    

    void Update()
    {
        // El tiempo solo corre en modo normal y si hay un cliente en pantalla
        if (isClientActive && isTutorialCompleted)
        {
            HandlePatienceTimer();
        }
    }

    // ==========================================
    // ESTADO A: TUTORIAL (Comercio Scriptado)
    // ==========================================
    private void StartTutorialMode()
    {
        isClientActive = true;
        currentOrder = tutorialPotionRequired;

        // Visuales estáticas del tutorial
        crtMonitorImage.sprite = tutorialClientSprite;
        crtMonitorImage.color = Color.white;
        
        orderScreenImage.sprite = currentOrder.itemIcon;
        orderScreenImage.color = Color.white;

        // La barra de paciencia y la fila están desactivadas
        patienceBar.fillAmount = 1f;
        patienceBar.color = Color.cyan; // Un color que indique "Modo Seguro"
        queueText.text = ""; 
    }

    public void CompleteTutorialTrade()
    {
        // Se llama cuando se entrega la poción del tutorial
        playerCredits += 50;
        PlayerPrefs.SetInt("TutorialCompleted", 1);
        isTutorialCompleted = true;
        
        Debug.Log("Tutorial completado. Créditos: " + playerCredits);

        // CORRECCIÓN: Sumamos a la variable directamente para no disparar el gatillo doble
        clientsInQueue += Random.Range(2, 5); 
        // Actualizamos el texto retro "x 03"
        if (clientsInQueue > 0) queueText.text = "x " + clientsInQueue.ToString("D2"); 

        StartCoroutine(ResolveClientAndCheckQueue());
    }

    // ==========================================
    // ESTADO B: BUCLE DE COMERCIO NORMAL
    // ==========================================
    public void AddClientsToQueue(int amount)
    {
        clientsInQueue += amount;
        UpdateQueueUI();

        if (!isClientActive && clientsInQueue > 0)
        {
            StartCoroutine(CallNextClient());
        }
    }

    private void UpdateQueueUI()
    {
        if (clientsInQueue > 0)
        {
            queueText.text = "x " + clientsInQueue.ToString("D2");
        }
        else
        {
            queueText.text = ""; // Pantalla apagada si no hay nadie
        }
    }

    private IEnumerator CallNextClient()
    {
        yield return new WaitForSeconds(1.5f); // Breve pausa inmersiva

        clientsInQueue--;
        UpdateQueueUI();
        GenerateDynamicClient();
    }

    private void GenerateDynamicClient()
    {
        isClientActive = true;
        currentPatience = maxPatienceTime;

        // Asignar Sprite aleatorio con tinte
        crtMonitorImage.sprite = normalClientSprites[Random.Range(0, normalClientSprites.Count)];
        crtMonitorImage.color = new Color(Random.Range(0.6f, 1f), Random.Range(0.6f, 1f), Random.Range(0.6f, 1f), 1f);

        // Elegir pedido aleatorio de las pociones desbloqueadas
        currentOrder = unlockedPotions[Random.Range(0, unlockedPotions.Count)];
        orderScreenImage.sprite = currentOrder.itemIcon;
        orderScreenImage.color = Color.white;
    }

    private void HandlePatienceTimer()
    {
        currentPatience -= Time.deltaTime;
        float normalizedTime = currentPatience / maxPatienceTime;
        patienceBar.fillAmount = normalizedTime;

        if (normalizedTime > 0.6f) patienceBar.color = Color.green;
        else if (normalizedTime > 0.3f) patienceBar.color = Color.yellow;
        else patienceBar.color = Color.red;

        if (currentPatience <= 0)
        {
            ProcessDelivery(null); // Tiempo agotado
        }
    }

    // ==========================================
    // RESOLUCIÓN (Conexión con la Vending Machine)
    // ==========================================
    
    // Tu botón Verde de la máquina debe llamar a esta función pasando la poción que fabricó
   public void ProcessDelivery(ItemData potionDelivered)
    {
        Debug.Log("--- INICIANDO ENTREGA ---");
        
        // 1. Revisar si hay un cliente vivo
        Debug.Log("¿Hay cliente activo?: " + isClientActive);
        if (!isClientActive) 
        {
            Debug.Log("ERROR LOGICO: No hay cliente. Abortando entrega.");
            return;
        }

        // 2. Revisar qué poción nos envió el botón
        if (potionDelivered == null)
        {
            Debug.Log("ERROR LOGICO: El botón envió un ItemData vacío (Null). ¡Revisa el Inspector del botón!");
            return;
        }
        Debug.Log("Poción entregada por el botón: " + potionDelivered.itemName + " (ID: " + potionDelivered.id + ")");

        // 3. Revisar qué poción está pidiendo el cliente
        if (currentOrder == null)
        {
            Debug.Log("ERROR LOGICO: El cliente no tiene ningún pedido asignado.");
            return;
        }
        Debug.Log("Poción que pide el cliente: " + currentOrder.itemName + " (ID: " + currentOrder.id + ")");

        isClientActive = false; // Detenemos el tiempo

        // 4. Comparación Final
        if (!isTutorialCompleted)
        {
            Debug.Log("Modo: TUTORIAL. Validando...");
            if (potionDelivered.id == tutorialPotionRequired.id)
            {
                Debug.Log("¡ÉXITO EN TUTORIAL! Los IDs coinciden.");
                CompleteTutorialTrade();
            }
            else
            {
                Debug.Log("FALLO EN TUTORIAL: Le entregaste el ID " + potionDelivered.id + " pero requiere el ID " + tutorialPotionRequired.id);
            }
            return;
        }

        Debug.Log("Modo: BUCLE NORMAL. Validando...");
        if (potionDelivered.id == currentOrder.id)
        {
            Debug.Log("¡ÉXITO EN MODO NORMAL! Los IDs coinciden.");
            CalculateDynamicPayout();
        }
        else
        {
            Debug.Log("FALLO MODO NORMAL: Le entregaste una poción equivocada. Pedía ID " + currentOrder.id);
        }

        StartCoroutine(ResolveClientAndCheckQueue());
    }

    private void CalculateDynamicPayout()
    {
        int baseValue = 50; // Idealmente esto estaría en tu ItemData (ej. currentOrder.baseValue)
        float timeRatio = currentPatience / maxPatienceTime;

        int finalPayout;
        if (timeRatio > 0.6f) finalPayout = baseValue + Mathf.RoundToInt(baseValue * 0.2f); // Propina
        else if (timeRatio > 0.3f) finalPayout = baseValue; // Normal
        else finalPayout = Mathf.RoundToInt(baseValue * 0.5f); // Penalizado

        playerCredits += finalPayout;
        Debug.Log("Poción entregada. Créditos actuales: " + playerCredits);
    }

    private IEnumerator ResolveClientAndCheckQueue()
    {
        yield return new WaitForSeconds(2f); // Tiempo para animaciones/reacciones
        ClearScreens();

        if (clientsInQueue > 0 && isTutorialCompleted)
        {
            StartCoroutine(CallNextClient());
        }
    }

    private void ClearScreens()
    {
        crtMonitorImage.color = Color.clear;
        orderScreenImage.color = Color.clear;
        patienceBar.fillAmount = 0;
        currentOrder = null;
    }
}