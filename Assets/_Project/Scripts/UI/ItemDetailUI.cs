using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemDetailUI : MonoBehaviour
{
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;
    public Image itemSprite;

    public void ShowItem(ItemData item)
    {
        if (item == null) return;
        titleText.text = item.itemName;
        descriptionText.text = item.itemDescription;

        if (item.itemIcon != null)
           itemSprite.sprite = item.itemIcon;  
    }
}
