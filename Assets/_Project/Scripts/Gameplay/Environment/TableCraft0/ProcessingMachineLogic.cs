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
}