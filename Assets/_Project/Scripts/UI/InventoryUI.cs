using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public GameObject inventoryPanel;
    public Transform itemGrid;
    public GameObject slotPrefab;

    public void Show()
    {
        RefreshUI();
        inventoryPanel.SetActive(true);
    }

    public void Hide()
    {
        inventoryPanel.SetActive(false);
    }
   

    public void RefreshUI()
    {
        foreach (Transform child in itemGrid)
        {
            Destroy(child.gameObject);
        }

        foreach (var slot in InventoryManager.Instance.inventory)
        {
            GameObject newSlot = Instantiate(slotPrefab, itemGrid);
            InventorySlotUI slotUI = newSlot.GetComponent<InventorySlotUI>();
            slotUI.SetSlot(slot.item, slot.quantity);
        }
    }
}
