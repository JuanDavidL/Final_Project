using UnityEngine;
using TMPro;

public class PotionSlotUI : MonoBehaviour
{
    public RecipeData recipe;
    public TextMeshProUGUI[] quantityTexts; // ← array de textos

    void Start()
    {
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnInventoryUpdated += UpdateQuantity;
            UpdateQuantity();
        }
    }

    void OnDestroy()
    {
        if (InventoryManager.Instance != null)
            InventoryManager.Instance.OnInventoryUpdated -= UpdateQuantity;
    }

    private void UpdateQuantity()
    {
        var slot = InventoryManager.Instance.inventory
            .Find(s => s.item == recipe.resultPotion);

        int quantity = slot != null ? slot.quantity : 0;
        string text = $"x{quantity}";

        foreach (var t in quantityTexts)
            t.text = text;
    }
}