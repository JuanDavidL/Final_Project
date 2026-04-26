using UnityEngine;
using UnityEngine.UI;

public class RecipeSlotUI : MonoBehaviour
{
    public Image icon;
    public RecipeData recipe;

    private Button _button;
    private static RecipeDetailUI _detailUI;

    void Awake()
    {
        _button = GetComponent<Button>();
        if (_button != null)
            _button.onClick.AddListener(OnSlotClicked);
    }

    public void SetSlot(RecipeData recipeData)
    {
        recipe = recipeData;

        if (recipe.recipeIcon != null)
            icon.sprite = recipe.recipeIcon;
    }

    public void Select()
    {
        if (_detailUI == null)
            _detailUI = FindFirstObjectByType<RecipeDetailUI>();

        _button?.Select();
        _detailUI?.ShowRecipe(recipe);
    }

    private void OnSlotClicked()
    {
        if (_detailUI == null)
            _detailUI = FindFirstObjectByType<RecipeDetailUI>();

        _detailUI?.ShowRecipe(recipe);
    }
}