using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class PotionQuickBar : MonoBehaviour
{
    [Header("Configuración de Salud (Poción 1)")]
    public ItemData healthPotionItem;
    public Image healthIcon;
    public TextMeshProUGUI healthCountText;
    public float healthRecoverAmount = 50f;
    public float healthCooldown = 15f;
    private float healthTimer = 0f;

    [Header("Configuración de Maná (Poción 2)")]
    public ItemData manaPotionItem;
    public Image manaIcon;
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
        if (healthPotionItem != null && healthIcon != null) 
            healthIcon.sprite = healthPotionItem.itemIcon;
        
        if (manaPotionItem != null && manaIcon != null) 
            manaIcon.sprite = manaPotionItem.itemIcon;

        if (InventoryManager.Instance != null)
            InventoryManager.Instance.OnInventoryUpdated += UpdatePotionUI;
        
        UpdatePotionUI();
        
        // Inicializar los iconos como llenos
        if (healthIcon != null) healthIcon.fillAmount = 1;
        if (manaIcon != null) manaIcon.fillAmount = 1;
    }

    void OnDestroy()
    {
        if (InventoryManager.Instance != null)
            InventoryManager.Instance.OnInventoryUpdated -= UpdatePotionUI;
    }

    void Update()
    {
        if (Time.timeScale == 0) return;

        // Manejo de Cooldown Salud
        if (healthTimer > 0)
        {
            healthTimer -= Time.deltaTime;
            if (healthIcon != null)
            {
                // El fillAmount va de 0 a 1 conforme pasa el tiempo
                healthIcon.fillAmount = 1 - (healthTimer / healthCooldown);
                healthIcon.color = new Color(0.3f, 0.3f, 0.3f, 1f); // Oscuro mientras carga
            }
        }
        else if (healthIcon != null && healthIcon.fillAmount < 1)
        {
            healthIcon.fillAmount = 1;
            healthIcon.color = Color.white; // Color normal cuando está lista
        }

        // Manejo de Cooldown Maná
        if (manaTimer > 0)
        {
            manaTimer -= Time.deltaTime;
            if (manaIcon != null)
            {
                manaIcon.fillAmount = 1 - (manaTimer / manaCooldown);
                manaIcon.color = new Color(0.3f, 0.3f, 0.3f, 1f);
            }
        }
        else if (manaIcon != null && manaIcon.fillAmount < 1)
        {
            manaIcon.fillAmount = 1;
            manaIcon.color = Color.white;
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