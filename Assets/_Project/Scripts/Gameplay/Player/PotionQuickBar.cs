using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PotionQuickBar : MonoBehaviour
{
    [Header("Referencias de Inventario")]
    public InventorySO inventory;

    [Header("Configuración de Salud (Poción 1)")]
    public RecipeData healthRecipe;
    public Image healthIcon;
    public float healthRecoverAmount = 50f;

    [Header("Configuración de Maná (Poción 2)")]
    public RecipeData manaRecipe;
    public Image manaIcon;
    public float manaRecoverAmount = 40f;

    private PlayerHealth healthSystem;
    private PlayerMana manaSystem;

    void Awake()
    {
        healthSystem = GetComponent<PlayerHealth>();
        manaSystem = GetComponent<PlayerMana>();
    }

    void Start()
    {
        if (healthRecipe != null && healthIcon != null) healthIcon.sprite = healthRecipe.recipeIcon;
        if (manaRecipe != null && manaIcon != null) manaIcon.sprite = manaRecipe.recipeIcon;
    }

    void Update()
    {
        if (Time.timeScale == 0) return;
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        if (keyboard.digit1Key.wasPressedThisFrame)
        {
            UsePotion(healthRecipe, true);
        }

        if (keyboard.digit2Key.wasPressedThisFrame)
        {
            UsePotion(manaRecipe, false);
        }
    }

    private void UsePotion(RecipeData recipe, bool isHealth)
    {
        if (recipe == null || recipe.resultPotion == null) return;

        if (inventory.RemoveItem(recipe.resultPotion, 1))
        {
            if (isHealth)
            {
                healthSystem.currentHealth = Mathf.Min(healthSystem.currentHealth + healthRecoverAmount, healthSystem.maxHealth);
                healthSystem.SendMessage("ActualizarUI", SendMessageOptions.DontRequireReceiver);
            }
            else
            {
                manaSystem.currentMana = Mathf.Min(manaSystem.currentMana + manaRecoverAmount, manaSystem.maxMana);
                manaSystem.SendMessage("ActualizarUI", SendMessageOptions.DontRequireReceiver);
            }
        }
    }
}