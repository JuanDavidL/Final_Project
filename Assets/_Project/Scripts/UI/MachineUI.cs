using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MachineUI : MonoBehaviour
{
    [Header("Paneles")]
    public GameObject panelMakePotion;
    public GameObject panelChooseRecipe;
    public GameObject panelYouNeed;
    public GameObject textGiraLaPalanca;

    [Header("Choose Recipe")]
    public Image potionSprite;
    public TextMeshProUGUI potionName;
    public Button btnLeft;
    public Button btnRight;

    [Header("You Need")]
    public Image spriteIngredient1;
    public Image spriteIngredient2;
    public TextMeshProUGUI quantityText1;
    public TextMeshProUGUI quantityText2;
    public Button btnConfirm;

    [Header("Recetas")]
    public RecipeData[] recipes;

    private int _required1 = 0;
    private int _required2 = 0;
    private int _current1 = 0;
    private int _current2 = 0;
    private ItemData _ingredient1;
    private ItemData _ingredient2;
    private int _currentRecipeIndex = 0;
    private ProcessingMachineLogic _machine;

    void Awake()
    {
        _machine = FindFirstObjectByType<ProcessingMachineLogic>();
    }

    void Start()
    {
        ShowPanel(panelMakePotion);
    }

    // Boton Make a Potion
    public void OnMakePotionClick()
    {
        ShowPanel(panelChooseRecipe);
        UpdateRecipeDisplay();
    }

    // Flechas del carousel
    public void OnLeftClick()
    {
        _currentRecipeIndex--;
        if (_currentRecipeIndex < 0)
            _currentRecipeIndex = recipes.Length - 1;
        UpdateRecipeDisplay();
    }

    public void OnRightClick()
    {
        _currentRecipeIndex++;
        if (_currentRecipeIndex >= recipes.Length)
            _currentRecipeIndex = 0;
        UpdateRecipeDisplay();
    }

    // Boton Confirm
    public void OnConfirmClick()
    {
        RecipeData selected = recipes[_currentRecipeIndex];
        _machine.SeleccionarRecetaManual(selected);
        ShowPanel(panelYouNeed);
        UpdateIngredientsDisplay(selected);
    }

    // Llamado desde ProcessingMachineLogic cuando todos los ingredientes fueron echados
    public void OnIngredientsComplete()
    {
        ShowPanel(textGiraLaPalanca);
    }

    // Llamado cuando termina el proceso
    public void ResetUI()
    {
        ShowPanel(panelMakePotion);
    }

    private void UpdateRecipeDisplay()
    {
        RecipeData recipe = recipes[_currentRecipeIndex];
        potionName.text = recipe.recipeName;

        if (recipe.recipeIcon != null)
            potionSprite.sprite = recipe.recipeIcon;
        else
            Debug.LogWarning("recipeIcon es null!");
    }
    
    private void UpdateIngredientsDisplay(RecipeData recipe)
    {
        if (recipe.requiredIngredients.Count > 0)
        {
            _ingredient1 = recipe.requiredIngredients[0].item;
            _required1 = recipe.requiredIngredients[0].quantity;
            _current1 = 0;
            spriteIngredient1.gameObject.SetActive(true);
            spriteIngredient1.sprite = _ingredient1.itemIcon;
            quantityText1.text = $"0/{_required1}";
        }
        else
        {
            spriteIngredient1.gameObject.SetActive(false);
        }

        if (recipe.requiredIngredients.Count > 1)
        {
            _ingredient2 = recipe.requiredIngredients[1].item;
            _required2 = recipe.requiredIngredients[1].quantity;
            _current2 = 0;
            spriteIngredient2.gameObject.SetActive(true);
            spriteIngredient2.sprite = _ingredient2.itemIcon;
            quantityText2.text = $"0/{_required2}";
        }
        else
        {
            spriteIngredient2.gameObject.SetActive(false);
        }
    }

    public void OnIngredientDeposited(ItemData item)
    {
        if (_ingredient1 != null && item == _ingredient1)
        {
            _current1 = Mathf.Min(_current1 + 1, _required1);
            quantityText1.text = $"{_current1}/{_required1}";
        }
        else if (_ingredient2 != null && item == _ingredient2)
        {
            _current2 = Mathf.Min(_current2 + 1, _required2);
            quantityText2.text = $"{_current2}/{_required2}";
        }

        // Verifica si todos los ingredientes fueron depositados
        if (_current1 >= _required1 && _current2 >= _required2)
            quantityText1.transform.parent.GetComponentInParent<TextMeshProUGUI>()?.gameObject.SetActive(false);
    }

    private void ShowPanel(GameObject panel)
    {
        panelMakePotion.SetActive(false);
        panelChooseRecipe.SetActive(false);
        panelYouNeed.SetActive(false);
        textGiraLaPalanca.SetActive(false);

        panel.SetActive(true);
    }
}