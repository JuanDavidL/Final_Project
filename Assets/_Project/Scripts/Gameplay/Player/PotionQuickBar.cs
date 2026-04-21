using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class PotionQuickBar : MonoBehaviour
{
    [Header("Salud (Poción 1)")]
    public ItemData healthPotionItem;
    public Image healthBackgroundIcon; // La imagen oscura de fondo
    public Image healthCooldownImage; // La imagen clara de encima (con el reloj)
    public TextMeshProUGUI healthCountText;
    public float healthRecoverAmount = 50f;
    public float healthCooldown = 15f;
    private float healthTimer = 0f;

    [Header("Maná (Poción 2)")]
    public ItemData manaPotionItem;
    public Image manaBackgroundIcon; // La imagen oscura de fondo
    public Image manaCooldownImage; // La imagen que va delante
    public TextMeshProUGUI manaCountText;
    public float manaRecoverAmount = 30f;
    public float manaCooldown = 5f;
    private float manaTimer = 0f;

    private PlayerHealth healthSystem;
    private PlayerMana manaSystem;

    void Awake()
    {
        healthSystem = GetComponent<PlayerHealth>();
        manaSystem = GetComponent<PlayerMana>();
    }

    void Start()
    {
        // Asignar el sprite a ambas imágenes (fondo y frente)
        if (healthPotionItem != null)
        {
            if (healthBackgroundIcon != null) healthBackgroundIcon.sprite = healthPotionItem.itemIcon;
            if (healthCooldownImage != null) healthCooldownImage.sprite = healthPotionItem.itemIcon;
        }
        
        if (manaPotionItem != null)
        {
            if (manaBackgroundIcon != null) manaBackgroundIcon.sprite = manaPotionItem.itemIcon;
            if (manaCooldownImage != null) manaCooldownImage.sprite = manaPotionItem.itemIcon;
        }

        if (InventoryManager.Instance != null)
            InventoryManager.Instance.OnInventoryUpdated += UpdatePotionUI;
        
        UpdatePotionUI();
        
        // Empezar con el reloj lleno (disponible)
        if (healthCooldownImage != null) healthCooldownImage.fillAmount = 1;
        if (manaCooldownImage != null) manaCooldownImage.fillAmount = 1;
    }

    void OnDestroy()
    {
        if (InventoryManager.Instance != null) InventoryManager.Instance.OnInventoryUpdated -= UpdatePotionUI;
    }

    void Update()
    {
        if (Time.timeScale == 0) return;

        // Lógica Visual Salud
        if (healthTimer > 0)
        {
            healthTimer -= Time.deltaTime;
            if (healthCooldownImage != null)
                healthCooldownImage.fillAmount = 1 - (healthTimer / healthCooldown);
        }
        else if (healthCooldownImage != null && healthCooldownImage.fillAmount < 1)
        {
            healthCooldownImage.fillAmount = 1;
        }

        // Lógica Visual Maná
        if (manaTimer > 0)
        {
            manaTimer -= Time.deltaTime;
            if (manaCooldownImage != null)
                manaCooldownImage.fillAmount = 1 - (manaTimer / manaCooldown);
        }
        else if (manaCooldownImage != null && manaCooldownImage.fillAmount < 1)
        {
            manaCooldownImage.fillAmount = 1;
        }

        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        if (keyboard.digit1Key.wasPressedThisFrame) TryUseHealthPotion();
        if (keyboard.digit2Key.wasPressedThisFrame) TryUseManaPotion();
    }

    private void TryUseHealthPotion()
    {
        if (healthTimer > 0 || healthSystem.currentHealth >= healthSystem.maxHealth) return;

        if (UsePotionFromInventory(healthPotionItem))
        {
            healthSystem.currentHealth = Mathf.Min(healthSystem.currentHealth + healthRecoverAmount, healthSystem.maxHealth);
            healthSystem.SendMessage("ActualizarUI", SendMessageOptions.DontRequireReceiver);
            healthTimer = healthCooldown;
            if (healthCooldownImage != null) healthCooldownImage.fillAmount = 0; // Reiniciar reloj
        }
    }

    private void TryUseManaPotion()
    {
        if (manaTimer > 0 || manaSystem.currentMana >= manaSystem.maxMana) return;

        if (UsePotionFromInventory(manaPotionItem))
        {
            manaSystem.currentMana = Mathf.Min(manaSystem.currentMana + manaRecoverAmount, manaSystem.maxMana);
            manaSystem.SendMessage("ActualizarUI", SendMessageOptions.DontRequireReceiver);
            manaTimer = manaCooldown;
            if (manaCooldownImage != null) manaCooldownImage.fillAmount = 0; // Reiniciar reloj
        }
    }

    private bool UsePotionFromInventory(ItemData potionItem)
    {
        if (potionItem == null || InventoryManager.Instance == null) return false;
        return InventoryManager.Instance.RemoveItem(potionItem, 1);
    }

    public void UpdatePotionUI()
    {
        if (InventoryManager.Instance == null) return;
        if (healthCountText != null) healthCountText.text = GetItemQuantity(healthPotionItem).ToString();
        if (manaCountText != null) manaCountText.text = GetItemQuantity(manaPotionItem).ToString();
    }

    private int GetItemQuantity(ItemData item)
    {
        if (item == null) return 0;
        var slot = InventoryManager.Instance.inventory.Find(s => s.item == item);
        return slot != null ? slot.quantity : 0;
    }
}