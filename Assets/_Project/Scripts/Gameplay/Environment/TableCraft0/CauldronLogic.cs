using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CauldronLogic : MonoBehaviour
{
    [Header("Configuración de Recetas")]
    [SerializeField] private ItemData potionBasura;
    
    // esta es la receta que el jugador eligió en la UI o el libro // por definir
    private RecipeData selectedRecipe;

    [Header("Estado del Caldero")]
    // lista de ingredientes actualmente dentro del caldero
    private List<InventoryManager.InventorySlot> currentIngredients = new List<InventoryManager.InventorySlot>();

    // cuando el jugador selecciona una receta en el menú
    public void SelectRecipe(RecipeData recipe)
    {
        selectedRecipe = recipe;
        Debug.Log($"Receta seleccionada: {recipe.recipeName}. ¡A cocinar!");
    }

    private void OnTriggerEnter(Collider other)
    {
        // obtenemos la data del objeto físico
        if (other.TryGetComponent<PoolableItem>(out PoolableItem item))
        {
            StartCoroutine(AbsorbIngredient(item));
        }
    }

    private IEnumerator AbsorbIngredient(PoolableItem item)
    {
        // se agrega al listado interno del caldero
        AddIngredientToList(item.data);

        if (item.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rb.isKinematic = true;
        }

        // efecto de encogimiento -> opcional usarlos para transportar las pociones
        float duration = 0.5f;
        float elapsed = 0f;
        Vector3 initialScale = item.transform.localScale;

        while (elapsed < duration)
        {
            item.transform.localScale = Vector3.Lerp(initialScale, Vector3.zero, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // devuelve al pool
        item.transform.localScale = initialScale;
        if (rb != null) rb.isKinematic = false;

        IngredientPool.Instance.Release(item);
    }

    private void AddIngredientToList(ItemData data)
    {
        var existing = currentIngredients.Find(s => s.item == data);
        if (existing != null) existing.quantity++;
        else currentIngredients.Add(new InventoryManager.InventorySlot { item = data, quantity = 1 });

        Debug.Log($"Caldero: {data.itemName} x{(existing != null ? existing.quantity : 1)}");
    }

    public void RefundIngredients()
    {
        if (currentIngredients.Count == 0) return;

        foreach (var slot in currentIngredients)
        {
            InventoryManager.Instance.AddItem(slot.item, slot.quantity);
        }

        // limpiamos la lista local => el caldero quede vacío
        currentIngredients.Clear();
        Debug.Log("Caldero vaciado y recursos devueltos.");
    }

    public void TryCraft()
    {
        if (selectedRecipe == null)
        {
            Debug.LogWarning("¡No has seleccionado ninguna receta!");
            return;
        }

        if (IsRecipeMatch(selectedRecipe))
        {
            Debug.Log($"<color=green>¡Éxito!</color> Crafteaste: {selectedRecipe.recipeName}");
            Debug.Log($"Dificultad: {selectedRecipe.difficulty} | Valor: {selectedRecipe.marketValue}");
            FinalizeCraft(selectedRecipe.resultPotion);
        }
        else
        {
            Debug.Log("<color=red>Fallo:</color> Ingredientes incorrectos para esta receta.");
            FinalizeCraft(potionBasura);
        }
    }

    private bool IsRecipeMatch(RecipeData recipe)
    {
        // 1. Verificación básica: ¿Misma cantidad de tipos de ingredientes?
        if (recipe.requiredIngredients.Count != currentIngredients.Count) return false;

        // 2. Verificación detallada
        foreach (var required in recipe.requiredIngredients)
        {
            var inCauldron = currentIngredients.Find(x => x.item == required.item);

            if (inCauldron == null || inCauldron.quantity != required.quantity)
                return false;
        }

        return true;
    }

    private void FinalizeCraft(ItemData result)
    {
        currentIngredients.Clear();
        // spawn del objeto físico de la poción // por definir
        Debug.Log($"Objeto generado: {result.itemName}");
    }
}