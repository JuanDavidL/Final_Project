using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    public GameObject inventoryPanel;
    public GameObject itemGridPanel;
    public Transform itemGrid;
    public GameObject slotPrefab;

    
    [Header("Detail UI")]
    public ItemDetailUI itemDetailUI;

    
    [Header("Empty Message")]
    public TextMeshProUGUI emptyText;

    public void Show()
    {
        inventoryPanel.SetActive(true);
        itemGridPanel.SetActive(true);
        RefreshUI();
    }

    public void Hide()
    {
        inventoryPanel.SetActive(false);
        itemGridPanel.SetActive(false);
    }

    public void RefreshUI()
    {
        // Asigna el detailUI estático si no está asignado
        if (InventorySlotUI.detailUI == null)
            InventorySlotUI.detailUI = itemDetailUI != null 
                ? itemDetailUI 
                : FindFirstObjectByType<ItemDetailUI>();

        // Limpia el grid
        foreach (Transform child in itemGrid)
            Destroy(child.gameObject);

        var inventory = InventoryManager.Instance.inventory;

        //Inventario vacío
        if (inventory.Count == 0)
        {
            if (emptyText != null)
            {
                emptyText.gameObject.SetActive(true);
                emptyText.text = "The inventory is empty :/";
            }

            // Limpia el detalle
            if (InventorySlotUI.detailUI != null)
            {
                InventorySlotUI.detailUI.titleText.text = "";
                InventorySlotUI.detailUI.descriptionText.text = "";
                InventorySlotUI.detailUI.itemSprite.sprite = null;
                InventorySlotUI.detailUI.itemSprite.gameObject.SetActive(false);
                InventorySlotUI.detailUI.spriteBackground?.SetActive(false);
                InventorySlotUI.detailUI.titleText.gameObject.SetActive(false);
                InventorySlotUI.detailUI.descriptionText.gameObject.SetActive(false);
            }
            return;
        }

        //Hay items — oculta el mensaje de vacío
        if (emptyText != null)
            emptyText.gameObject.SetActive(false);

        if (InventorySlotUI.detailUI != null)
        {
            InventorySlotUI.detailUI.itemSprite.gameObject.SetActive(true);
            InventorySlotUI.detailUI.spriteBackground?.SetActive(true);
            InventorySlotUI.detailUI.titleText.gameObject.SetActive(true);  
            InventorySlotUI.detailUI.descriptionText.gameObject.SetActive(true);
        }

        // Instancia los slots
        InventorySlotUI primerSlot = null;

        foreach (var slot in inventory)
        {
            GameObject newSlot = Instantiate(slotPrefab, itemGrid);
            InventorySlotUI slotUI = newSlot.GetComponent<InventorySlotUI>();
            slotUI.SetSlot(slot.item, slot.quantity);

            //Guarda el primero
            if (primerSlot == null)
                primerSlot = slotUI;
        }

        //Selecciona el primer slot automáticamente
        if (primerSlot != null)
        {
            primerSlot.button.Select();
            InventorySlotUI.detailUI?.ShowItem(primerSlot.currentItem);
        }
    }
}