using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RecipeDetailUI : MonoBehaviour
{
    [Header("Info Poción")]
    public TextMeshProUGUI titlePotion;
    public TextMeshProUGUI potionDescription;
    public Image potionSprite;

    [Header("Ingredientes")]
    public Image ingredient1Sprite;
    public Image ingredient2Sprite;
    public TextMeshProUGUI ingredient1Count; // el "Plus" o cantidad
    public TextMeshProUGUI ingredient2Count;

    [Header("Créditos")]
    public TextMeshProUGUI creditCount;

    public void ShowRecipe(RecipeData recipe)
    {
        if (recipe == null) return;

        // ── Info de la poción desde ItemData ──────────
        titlePotion.text = recipe.resultPotion.itemName;
        potionDescription.text = recipe.resultPotion.itemDescription;

        if (recipe.resultPotion.itemIcon != null)
            potionSprite.sprite = recipe.resultPotion.itemIcon;

        // ── Ingredientes ──────────────────────────────
        if (recipe.requiredIngredients.Count > 0)
        {
            var ing1 = recipe.requiredIngredients[0];
            ingredient1Sprite.gameObject.SetActive(true);
            ingredient1Sprite.sprite = ing1.item.itemIcon;
            if (ingredient1Count != null)
                ingredient1Count.text = ing1.quantity > 1 ? $"x{ing1.quantity}" : "";
        }
        else
            ingredient1Sprite.gameObject.SetActive(false);

        if (recipe.requiredIngredients.Count > 1)
        {
            var ing2 = recipe.requiredIngredients[1];
            ingredient2Sprite.gameObject.SetActive(true);
            ingredient2Sprite.sprite = ing2.item.itemIcon;
            if (ingredient2Count != null)
                ingredient2Count.text = ing2.quantity > 1 ? $"x{ing2.quantity}" : "";
        }
        else
            ingredient2Sprite.gameObject.SetActive(false);

        // ── Precio de venta ───────────────────────────
        creditCount.text = $"{recipe.marketValue} credits";
    }
}