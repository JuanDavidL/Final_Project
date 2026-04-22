using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class MachineUI : MonoBehaviour
{
    [Header("Status")]
    public TextMeshProUGUI statusText;

    [Header("Make A Potion")]
    public GameObject buttonMakeAPotion;

    [Header("Choose Recipe")]
    public GameObject buttonLeft;
    public GameObject buttonRight;
    public Image spritePotion;
    public TextMeshProUGUI textNamePotion;
    public GameObject buttonConfirmPotion;

    [Header("How Many")]
    public GameObject btnPlus;
    public GameObject btnMinus;
    public TextMeshProUGUI textQuantity;
    private int _quantity = 1;

    [Header("You Need")]
    public Image spriteIngredient1;
    public Image spriteIngredient2;
    public TextMeshProUGUI quantityText1;
    public TextMeshProUGUI quantityText2;

    [Header("Lever")]
    public GameObject textPullTheLever;
    public TextMeshProUGUI textCountOfLever;

    [Header("Cancel")]
    public GameObject buttonCancel;

    [Header("Recetas")]
    public RecipeData[] recipes;

    private int _required1 = 0;
    private int _required2 = 0;
    private int _current1 = 0;
    private int _current2 = 0;
    private ItemData _ingredient1;
    private ItemData _ingredient2;
    private int _currentRecipeIndex = 0;
    private int _requiredPresses = 3;
    private ProcessingMachineLogic _machine;

    void Awake()
    {
        _machine = FindFirstObjectByType<ProcessingMachineLogic>();
    }

    void Start()
    {
        ResetToStart();
    }

    private void ResetToStart()
    {
        statusText.text = "Press this button to make a potion ↓↓↓";

        buttonMakeAPotion.SetActive(true);
        buttonCancel.SetActive(false);
        buttonLeft.SetActive(false);
        buttonRight.SetActive(false);
        spritePotion.gameObject.SetActive(false);
        textNamePotion.gameObject.SetActive(false);
        buttonConfirmPotion.SetActive(false);
        spriteIngredient1.gameObject.SetActive(false);
        spriteIngredient2.gameObject.SetActive(false);
        quantityText1.gameObject.SetActive(false);
        quantityText2.gameObject.SetActive(false);
        textPullTheLever.SetActive(false);
        textCountOfLever.gameObject.SetActive(false);
        btnPlus.SetActive(false);
        btnMinus.SetActive(false);
        textQuantity.gameObject.SetActive(false);

        _quantity = 1;
        textQuantity.text = "x1";
    }

    // Boton Make a Potion
    public void OnMakePotionClick()
    {
        buttonMakeAPotion.SetActive(false);
        buttonCancel.SetActive(true);
        buttonLeft.SetActive(true);
        buttonRight.SetActive(true);
        spritePotion.gameObject.SetActive(true);
        textNamePotion.gameObject.SetActive(true);
        buttonConfirmPotion.SetActive(true);
        btnPlus.SetActive(true);
        btnMinus.SetActive(true);
        textQuantity.gameObject.SetActive(true);
        statusText.text = "Choose a recipe";
        UpdateRecipeDisplay();

        _quantity = 1;
        textQuantity.text = "x1";
    }

    // Flechas carousel
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
        _machine.SeleccionarRecetaManual(selected,_quantity);

        buttonLeft.SetActive(false);
        buttonRight.SetActive(false);
        spritePotion.gameObject.SetActive(false);
        textNamePotion.gameObject.SetActive(false);
        buttonConfirmPotion.SetActive(false);
        btnPlus.SetActive(false);
        btnMinus.SetActive(false);
        textQuantity.gameObject.SetActive(false);
        statusText.text = "Press RED button";
    }

    public void OnPlusClick()
    {
        if (_quantity >= 10) return;
        _quantity++;
        textQuantity.text = $"x{_quantity}";
    }

    public void OnMinusClick()
    {
        if (_quantity <= 1) return;
        _quantity--;
        textQuantity.text = $"x{_quantity}";
    }

    public void OnCancelClick()
    {
        _machine.CancelProcess();
        ResetToStart();
    }

    public void OnWrongIngredient()
    {
        statusText.text = "Wrong ingredient! Try again.";
        StartCoroutine(ResetStatusAfterDelay());
    }

    private IEnumerator ResetStatusAfterDelay()
    {
        yield return new WaitForSeconds(2f);
        statusText.text = "Press RED button";
    }


    // Llamado cuando se presiona boton rojo
    public void OnRedButtonPressed()
    {
        UpdateIngredientsDisplay(_machine.selectedRecipe);
        statusText.text = "Add ingredients";
    }

    // Llamado cuando se deposita un ingrediente
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

        if (_current1 >= _required1 && _current2 >= _required2)
            OnAllIngredientsDeposited();
    }

    public void OnAllIngredientsDeposited()
    {
        spriteIngredient1.gameObject.SetActive(false);
        spriteIngredient2.gameObject.SetActive(false);
        quantityText1.gameObject.SetActive(false);
        quantityText2.gameObject.SetActive(false);
        statusText.text = "Press yellow button";
    }

    // Llamado cuando se presiona boton amarillo
    public void OnYellowButtonPressed()
    {
        _requiredPresses = _machine.selectedRecipe.requiredLeverPresses;

        spriteIngredient1.gameObject.SetActive(false);
        spriteIngredient2.gameObject.SetActive(false);
        quantityText1.gameObject.SetActive(false);
        quantityText2.gameObject.SetActive(false);
        textPullTheLever.SetActive(true);
        textCountOfLever.gameObject.SetActive(true);
        textCountOfLever.text = $"0/{_requiredPresses}";
        statusText.text = "Pull the lever!";
    }

    // Actualiza contador de palanca
    public void UpdateLeverCounter(int count)
    {
        textCountOfLever.text = $"{count}/{_requiredPresses}";
    }

    // Llamado cuando termina la palanca
    public void OnLeverComplete()
    {
        textPullTheLever.SetActive(false);
        textCountOfLever.gameObject.SetActive(false);
        statusText.text = "Press green button";
    }

    // Llamado cuando se presiona boton verde
    public void OnProcessComplete(bool success, int successCount, int failCount)
    {
        if (failCount == 0)
            statusText.text = $"¡Éxito! {successCount} pociones";
        else if (successCount == 0)
            statusText.text = $"¡Fallo! {failCount} fallaron";
        else
            statusText.text = $"{successCount} éxito / {failCount} fallo";

        StartCoroutine(ResetAfterDelay());
    }

    private IEnumerator ResetAfterDelay()
    {
        yield return new WaitForSeconds(2f);
        ResetToStart();
    }

    public void ResetUI()
    {
        ResetToStart();
    }

    private void UpdateRecipeDisplay()
    {
        RecipeData recipe = recipes[_currentRecipeIndex];
        textNamePotion.text = recipe.recipeName;
        if (recipe.recipeIcon != null)
            spritePotion.sprite = recipe.recipeIcon;
    }

    private void UpdateIngredientsDisplay(RecipeData recipe)
    {
        if (recipe.requiredIngredients.Count > 0)
        {
            _ingredient1 = recipe.requiredIngredients[0].item;
            _required1 = recipe.requiredIngredients[0].quantity * _quantity;
            _current1 = 0;
            spriteIngredient1.gameObject.SetActive(true);
            spriteIngredient1.sprite = _ingredient1.itemIcon;
            quantityText1.gameObject.SetActive(true);
            quantityText1.text = $"0/{_required1}";
        }
        else
            spriteIngredient1.gameObject.SetActive(false);

        if (recipe.requiredIngredients.Count > 1)
        {
            _ingredient2 = recipe.requiredIngredients[1].item;
            _required2 = recipe.requiredIngredients[1].quantity * _quantity;
            _current2 = 0;
            spriteIngredient2.gameObject.SetActive(true);
            spriteIngredient2.sprite = _ingredient2.itemIcon;
            quantityText2.gameObject.SetActive(true);
            quantityText2.text = $"0/{_required2}";
        }
        else
            spriteIngredient2.gameObject.SetActive(false);
    }
}