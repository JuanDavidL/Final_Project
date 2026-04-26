using UnityEngine;

public class RecipesUI : MonoBehaviour
{
    [Header("Recetas disponibles")]
    public RecipeData[] recipes; // arrastra tus RecipeData aquí en orden

    [Header("Slots de pociones (botones)")]
    // Arrastra en orden: P_Healt, P_Mana, P_Hyper, P_Shield, P_Quinta
    public RecipeSlotUI[] recipeSlots;

    void Start()
    {
        SetupSlots();
    }

    public void RefreshUI()
    {
        SetupSlots();
    }

    private void SetupSlots()
    {
        for (int i = 0; i < recipeSlots.Length; i++)
        {
            if (i < recipes.Length)
            {
                recipeSlots[i].gameObject.SetActive(true);
                recipeSlots[i].SetSlot(recipes[i]);
            }
            else
            {
                // Si no hay receta para este slot lo oculta
                recipeSlots[i].gameObject.SetActive(false);
            }
        }

        // ✅ Selecciona P_Healt automáticamente al abrir
        if (recipeSlots.Length > 0 && recipes.Length > 0)
            recipeSlots[0].Select();
    }
}