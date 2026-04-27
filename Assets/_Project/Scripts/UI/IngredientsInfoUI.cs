using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IngredientsInfoUI : MonoBehaviour
{
    [Header("Slots de ingredientes")]
    public IngredientSlotInfoUI[] ingredientSlots;
    public ItemData[] ingredients; // arrastra tus ItemData de ingredientes

    [Header("Detail - RightPage")]
    public TextMeshProUGUI textTitle;
    public TextMeshProUGUI textDescription;
    public TextMeshProUGUI textWhereAndWho;
    public Image spriteIngredient;
    public GameObject slotBackground;

    void Start()
    {
        SetupSlots();
    }

    public void SetupSlots()
    {
        for (int i = 0; i < ingredientSlots.Length; i++)
        {
            if (i < ingredients.Length)
            {
                ingredientSlots[i].gameObject.SetActive(true);
                ingredientSlots[i].Setup(ingredients[i], this);
            }
            else
                ingredientSlots[i].gameObject.SetActive(false);
        }
    }

    public void SelectFirst()
    {
        if (ingredientSlots.Length > 0 && ingredients.Length > 0)
            ingredientSlots[0].Select();
    }

    public void ShowIngredient(ItemData item)
    {
        if (item == null) return;

        textTitle.text = item.itemName;
        textDescription.text = item.itemDescription;
        textWhereAndWho.text = item.whereAndWho;

        if (item.itemIcon != null)
        {
            spriteIngredient.sprite = item.itemIcon;
            spriteIngredient.gameObject.SetActive(true);
            if (slotBackground != null)
                slotBackground.SetActive(true);
        }
    }
}