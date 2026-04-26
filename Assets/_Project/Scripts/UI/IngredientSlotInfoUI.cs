using UnityEngine;
using UnityEngine.UI;

public class IngredientSlotInfoUI : MonoBehaviour
{
    public Image icon;
    public  ItemData _item;
    private IngredientsInfoUI _infoUI;
    private Button _button;

    void Awake()
    {
        _button = GetComponent<Button>();
        if (_button != null)
            _button.onClick.AddListener(OnClicked);
    }

    public void Setup(ItemData item, IngredientsInfoUI infoUI)
    {
        _item = item;
        _infoUI = infoUI;
        if (item.itemIcon != null)
            icon.sprite = item.itemIcon;
    }

    public void Select()
    {
        _button?.Select();
        _infoUI?.ShowIngredient(_item);
    }

    private void OnClicked()
    {
        _infoUI?.ShowIngredient(_item);
    }
}