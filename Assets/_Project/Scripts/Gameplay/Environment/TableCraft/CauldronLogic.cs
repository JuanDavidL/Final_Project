using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CauldronLogic : MonoBehaviour
{
    [Header("Configuración de Recetas")]
    [SerializeField] private List<RecipeData> allRecipes;
    [SerializeField] private ItemData potionBasura;

    [Header("Estado del Caldero")]
    // lista de ingredientes actualmente dentro del caldero
    private List<InventoryManager.InventorySlot> currentIngredients = new List<InventoryManager.InventorySlot>();

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

        // Efecto de encogimiento
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

        Debug.Log($"Caldero contiene: {data.itemName} x{existing?.quantity ?? 1}");
    }

    public void RefundIngredients()
    {
        if (currentIngredients.Count == 0) return;

        Debug.Log("Devolviendo ingredientes al inventario...");

        foreach (var slot in currentIngredients)
        {
            // Accedemos al Singleton para devolver la cantidad exacta
            InventoryManager.Instance.AddItem(slot.item, slot.quantity);
        }

        // limpiamos la lista local => el caldero quede vacío
        currentIngredients.Clear();

        Debug.Log("Caldero vaciado y recursos devueltos.");
    }

    public void Craft()
    {
        ItemData result = CheckRecipe(currentIngredients, allRecipes);
        Debug.Log($"¡Resultado: {result.itemName}!");

        currentIngredients.Clear();
    }

    public ItemData CheckRecipe(List<InventoryManager.InventorySlot> cauldronContent, List<RecipeData> allRecipes)
    {
        foreach (var recipe in allRecipes)
        {
            if (IsMatch(recipe.requiredIngredients, cauldronContent))
            {
                return recipe.resultPotion;
            }
        }
        return potionBasura; // referencia para poción fallida / Falta agregar...
    }

    private bool IsMatch(List<InventoryManager.InventorySlot> recipeReq, List<InventoryManager.InventorySlot> cauldron)
    {
        if (recipeReq.Count != cauldron.Count) return false;

        // Comparamos ID y cantidad
        foreach (var req in recipeReq)
        {
            var found = cauldron.Find(c => c.item.id == req.item.id && c.quantity == req.quantity);
            if (found == null) return false;
        }
        return true;
    }
}