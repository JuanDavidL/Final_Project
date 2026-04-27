using UnityEngine;

public class InfoPageUI : MonoBehaviour
{
    [Header("Select Mood")]
    public GameObject selectMood;        // panel con los 2 botones
    public GameObject selectMoodRight;   // SelectMoodInfo en RightPage

    [Header("Ingredientes")]
    public GameObject ingredientesInfo;      // IngredientesInfo en LeftPage
    public GameObject ingredientesMoodInfo;  // IngredientsMoodInfo en RightPage
    public GameObject buttonComeBackIngredients; // botón volver en LeftPage

    [Header("Criaturas")]
    public GameObject creaturesInfo;         // CreaturesInfo en LeftPage
    public GameObject creaturesMoodInfo;     // CreatureMoodInfo en RightPage
    public GameObject buttonComeBackCreatures;

    void Start()
    {
        // ✅ Inicializa los slots antes de que el usuario interactúe
        IngredientsInfoUI ingredientsUI = ingredientesInfo.GetComponent<IngredientsInfoUI>();
        if (ingredientsUI != null) ingredientsUI.SetupSlots();

        CreaturesInfoUI creaturesUI = creaturesInfo.GetComponent<CreaturesInfoUI>();
        if (creaturesUI != null) creaturesUI.SetupSlots();

        MostrarSelectMood();
    }

    public void RefreshUI()
    {
        MostrarSelectMood();
    }

    // ── Pantalla inicial ───────────────────────────

    public void MostrarSelectMood()
    {
        // Izquierda
        selectMood.SetActive(true);
        ingredientesInfo.SetActive(false);
        creaturesInfo.SetActive(false);
        buttonComeBackIngredients.SetActive(false);
        buttonComeBackCreatures.SetActive(false);

        // Derecha
        selectMoodRight.SetActive(true);
        ingredientesMoodInfo.SetActive(false);
        creaturesMoodInfo.SetActive(false);
    }

    // ── Botón About Ingredients ────────────────────

    public void OnSelectIngredients()
    {
        // Izquierda
        selectMood.SetActive(false);
        ingredientesInfo.SetActive(true);
        creaturesInfo.SetActive(false);
        buttonComeBackIngredients.SetActive(true);
        buttonComeBackCreatures.SetActive(false);

        // Derecha
        selectMoodRight.SetActive(false);
        ingredientesMoodInfo.SetActive(true);
        creaturesMoodInfo.SetActive(false);

        // ✅ Selecciona el primer ingrediente automáticamente
        IngredientsInfoUI ingredientsUI = ingredientesInfo.GetComponent<IngredientsInfoUI>();
        if (ingredientsUI != null)
            ingredientsUI.SelectFirst();
    }

    // ── Botón About Creatures ──────────────────────

    public void OnSelectCreatures()
    {
        // Izquierda
        selectMood.SetActive(false);
        ingredientesInfo.SetActive(false);
        creaturesInfo.SetActive(true);
        buttonComeBackIngredients.SetActive(false);
        buttonComeBackCreatures.SetActive(true);

        // Derecha
        selectMoodRight.SetActive(false);
        ingredientesMoodInfo.SetActive(false);
        creaturesMoodInfo.SetActive(true);

        // ✅ Selecciona la primera criatura automáticamente
        CreaturesInfoUI creaturesUI = creaturesInfo.GetComponent<CreaturesInfoUI>();
        if (creaturesUI != null)
            creaturesUI.SelectFirst();
    }
}