using UnityEngine;
using UnityEngine.UI;

public class CreatureSlotUI : MonoBehaviour
{
    public Image icon;
    public CreatureData _creature;
    private CreaturesInfoUI _infoUI;
    private Button _button;

    void Awake()
    {
        _button = GetComponent<Button>();
        if (_button != null)
            _button.onClick.AddListener(OnClicked);
    }

    public void Setup(CreatureData creature, CreaturesInfoUI infoUI)
    {
        _creature = creature;
        _infoUI = infoUI;
        if (creature.creatureSprite != null)
            icon.sprite = creature.creatureSprite;
    }

    public void Select()
    {
        _button?.Select();
        _infoUI?.ShowCreature(_creature);
    }

    private void OnClicked()
    {
        _infoUI?.ShowCreature(_creature);
    }
}