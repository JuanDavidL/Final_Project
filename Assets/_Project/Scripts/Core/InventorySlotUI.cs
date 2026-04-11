using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlotUI : MonoBehaviour
{
    public Image icon;
    public TMP_Text quantityText;

    public void SetSlot(ItemData item, int quantity)
    {
        icon.sprite = item.itemIcon;
        quantityText.text = quantity.ToString();
    }
}
