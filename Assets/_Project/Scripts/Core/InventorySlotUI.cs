using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlotUI : MonoBehaviour
{
    public Image icon;
    public TMP_Text quantityText;
    public ItemData currentItem;

    public Button button;
    public static ItemDetailUI detailUI;

    void Awake()
    {
        button = GetComponent<Button>();
        if (button != null)
            button.onClick.AddListener(OnSlotClicked);
        else
            Debug.LogWarning("Button component is missing on InventorySlotUI.");
    }

    public void SetSlot(ItemData item, int quantity)
    {
        currentItem = item;

        if (item.itemIcon != null)
            icon.sprite = item.itemIcon;

        if (quantity > 1)
        {
            quantityText.gameObject.SetActive(true);
            quantityText.text = quantity.ToString();
        }
        else
        {
            quantityText.gameObject.SetActive(false);
        }
    }

    private void OnSlotClicked()
    {
        if (detailUI != null && currentItem != null)
            detailUI.ShowItem(currentItem);
    }
}