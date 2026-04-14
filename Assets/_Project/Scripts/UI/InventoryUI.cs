using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public GameObject inventoryPanel;
    public GameObject itemGridPanel;
    public Transform itemGrid;
    public GameObject slotPrefab;

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
        if (InventorySlotUI.detailUI == null)
        {
            InventorySlotUI.detailUI = FindFirstObjectByType<ItemDetailUI>();
        }
        
        foreach (Transform child in itemGrid)
        {
            Destroy(child.gameObject);
        }

        bool isFirst = true;

        foreach (var slot in InventoryManager.Instance.inventory)
        {
            GameObject newSlot = Instantiate(slotPrefab, itemGrid);
            InventorySlotUI slotUI = newSlot.GetComponent<InventorySlotUI>();
            slotUI.SetSlot(slot.item, slot.quantity);
        }

        if (isFirst)
        {
            InventorySlotUI.detailUI = FindFirstObjectByType<ItemDetailUI>();
            isFirst = false;
        }
    }
}
