using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewRecipe", menuName = "Alquimia/Receta")]
public class RecipeData : ScriptableObject
{
    [Header("Identificación")]
    public int recipeID;
    public string recipeName;
    public Sprite recipeIcon; // Para mostrar en el UI

    [Header("Resultado")]
    public ItemData resultPotion; // La poción que se obtiene

    [Header("Resultado Visual")]
    public GameObject potionPrefab; // El modelo 3D específico de esta poción

    [Header("Requisitos")]
    public List<InventoryManager.InventorySlot> requiredIngredients;

    [Header("Atributos de Juego")]
    [Range(1, 3)] public int difficulty; // 1 = Fácil, 5 = Legendaria / Posibles cambios

    [Header("Probabilidad")]
    [Range(0, 100)] public float successChance = 80f; // 80% de éxito por defecto

    [Header("Procesamiento")]
    public int requiredLeverPresses = 3; // Por defecto 3

    public float marketValue; // Cuántos creditos / recomepensa recibirá el jugador 
}