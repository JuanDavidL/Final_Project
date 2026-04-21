using UnityEngine;
using TMPro;

public class IngredientSlotUI : MonoBehaviour
{
    public ItemData item;
    public TextMeshProUGUI quantityText;

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
            .Find(s => s.item == item);

        int quantity = slot != null ? slot.quantity : 0;
        quantityText.text = $"x{quantity}";
    }
}