using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewRecipe", menuName = "Alquimia/Receta")]
public class RecipeData : ScriptableObject
{
    [Header("Identificación")]
    public int recipeID;
    public string recipeName;

    [Header("Resultado")]
    public ItemData resultPotion; // La poción que se obtiene

    [Header("Requisitos")]
    public List<InventoryManager.InventorySlot> requiredIngredients;

    [Header("Atributos de Juego")]
    [Range(1, 3)] public int difficulty; // 1 = Fácil, 5 = Legendaria / Posibles cambios
    public float marketValue; // Cuántos creditos / recomepensa recibirá el jugador 
}
