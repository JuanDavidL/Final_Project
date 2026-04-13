using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewRecipe", menuName = "Alquimia/Receta")]
public class RecipeData : ScriptableObject
{
    public List<InventoryManager.InventorySlot> requiredIngredients;
    public ItemData resultPotion;
    public float baseValue; // Para la recompensa
}
