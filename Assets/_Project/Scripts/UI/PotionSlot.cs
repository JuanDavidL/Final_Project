using UnityEngine;

public class PotionSlot : MonoBehaviour
{
    public RecipeData recipe;
    public GameObject potionModel;

    public void UpdateSlot()
    {
        if (InventoryManager.Instance == null || recipe == null) return;

        bool hasItem = InventoryManager.Instance.inventory
            .Exists(slot => slot.item == recipe.resultPotion && slot.quantity > 0);

        potionModel.SetActive(hasItem);
    }
}