using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProcessingMachineLogic : MonoBehaviour
{
    public enum MachineState { Cerrada, Recibiendo, Procesando, Lista }

    [Header("Estado Actual")]
    public MachineState currentState = MachineState.Cerrada;

    [Header("Configuración de Recetas")]
    [SerializeField] private RecipeData selectedRecipe;
    [SerializeField] private ItemData potionBasura;

    [Header("Ingredientes en el Contenedor")]
    private List<InventoryManager.InventorySlot> currentIngredients = new List<InventoryManager.InventorySlot>();

    [Header("Referencias de Botones")]
    [SerializeField] private MachineButton btnRojo;
    [SerializeField] private MachineButton btnAmarillo;
    [SerializeField] private MachineButton btnVerde;

    [Header("Animaciones/Objetos")]
    [SerializeField] private GameObject contenedorFisico; // El cajón que sale/entra

    public void HandleButtonPress(MachineButton.ButtonType tipo)
    {
        switch (tipo)
        {
            case MachineButton.ButtonType.Rojo:
                TryOpenMachine();
                break;
            case MachineButton.ButtonType.Amarillo:
                TryCloseAndLock();
                break;
            case MachineButton.ButtonType.Verde:
                TryDeliverPotion();
                break;
        }
    }

    private void TryOpenMachine()
    {
        if (currentState == MachineState.Cerrada || currentState == MachineState.Lista)
        {
            currentState = MachineState.Recibiendo;
            btnRojo.SetLight(true);
            btnAmarillo.SetLight(false);
            btnVerde.SetLight(false);

            // Animación simple: Mover el contenedor hacia afuera
            // LeanTween.moveLocalZ(contenedorFisico, 1.5f, 0.5f);
            // Debug.Log("Contenedor abierto. Esperando materiales.");
            // Reemplazo de LeanTween por Corrutina
            StartCoroutine(MoverContenedor(0f));
        }
    }

    private void TryCloseAndLock()
    {
        if (currentState == MachineState.Recibiendo && currentIngredients.Count > 0)
        {
            currentState = MachineState.Procesando;
            btnRojo.SetLight(false);
            btnAmarillo.SetLight(true);

            // Animación: El contenedor entra
            // LeanTween.moveLocalZ(contenedorFisico, 0f, 0.5f);
            // Debug.Log("Contenedor cerrado. ¡Hora de girar la palanca!");
            StartCoroutine(MoverContenedor(1.5f));
        }
    }

    private void TryDeliverPotion()
    {
        if (currentState == MachineState.Lista)
        {
            btnVerde.SetLight(false);
            currentState = MachineState.Cerrada;
            // Aquí llamarías al Pool para que el jugador recoja su poción
        }
    }

    // --- MÉTODOS DE CONTROL (Fase 2 y 3) ---

    public void SelectRecipe(RecipeData recipe)
    {
        selectedRecipe = recipe;
        Debug.Log($"Máquina configurada para: {recipe.recipeName}");
    }

    public void AddIngredient(ItemData data)
    {
        // Solo aceptamos ingredientes si la máquina está abierta (Botón Rojo pulsado)
        if (currentState != MachineState.Recibiendo) return;

        var existing = currentIngredients.Find(s => s.item == data);
        if (existing != null) existing.quantity++;
        else currentIngredients.Add(new InventoryManager.InventorySlot { item = data, quantity = 1 });

        Debug.Log($"Máquina recibió: {data.itemName}");
    }

    // --- LÓGICA DE PROCESAMIENTO (Fase 4) ---

    public void ProcessFinalPotion()
    {
        if (selectedRecipe == null) return;

        // Aquí aplicamos la probabilidad que mencionaste
        float randomRoll = Random.Range(0f, 100f);

        if (randomRoll <= selectedRecipe.successChance)
        {
            Debug.Log("<color=green>¡Éxito!</color>");
            FinalizeProcess(selectedRecipe.resultPotion);
        }
        else
        {
            Debug.Log("<color=red>¡Fallo!</color>");
            FinalizeProcess(potionBasura);
        }
    }

    private void FinalizeProcess(ItemData result)
    {
        currentIngredients.Clear();
        currentState = MachineState.Lista; // Habilita el Botón Verde

        // Aquí llamaríamos al Pool para spawnear la poción
        Debug.Log($"Poción generada: {result.itemName}");
    }

    public void UpdateLeverProgress(int count)
    {
        if (selectedRecipe == null) return;

        // Si alcanzamos los palancazos de la receta...
        if (count >= selectedRecipe.requiredLeverPresses)
        {
            Debug.Log("¡Mecánica completada! Calculando poción...");
            ProcessFinalPotion();

            // Reset de la palanca para la próxima vez
            GetComponentInChildren<MachineLever>()?.ResetLever();
        }
    }

    private IEnumerator MoverContenedor(float targetZ)
    {
        float duration = 0.5f;
        float elapsed = 0f;
        Vector3 startPos = contenedorFisico.transform.localPosition;
        Vector3 endPos = new Vector3(startPos.x, startPos.y, targetZ);

        while (elapsed < duration)
        {
            contenedorFisico.transform.localPosition = Vector3.Lerp(startPos, endPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        contenedorFisico.transform.localPosition = endPos;
    }
}