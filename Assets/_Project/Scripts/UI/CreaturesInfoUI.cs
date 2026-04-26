using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CreaturesInfoUI : MonoBehaviour
{
    [Header("Slots de criaturas")]
    public CreatureSlotUI[] creatureSlots;
    public CreatureData[] creatures;

    [Header("Detail - RightPage")]
    public TextMeshProUGUI textTitle;
    public TextMeshProUGUI textDescription;
    public TextMeshProUGUI textWhereYouFind;
    public Image spriteCreature;
    public GameObject slotBackground;

    void Start()
    {
        SetupSlots();
    }

    public void SetupSlots()
    {
        for (int i = 0; i < creatureSlots.Length; i++)
        {
            if (i < creatures.Length)
            {
                creatureSlots[i].gameObject.SetActive(true);
                creatureSlots[i].Setup(creatures[i], this);
            }
            else
                creatureSlots[i].gameObject.SetActive(false);
        }
    }

    public void SelectFirst()
    {
        if (creatureSlots.Length > 0 && creatures.Length > 0)
            creatureSlots[0].Select();
    }

    public void ShowCreature(CreatureData creature)
    {
        if (creature == null) return;

        textTitle.text = creature.creatureName;
        textDescription.text = creature.creatureDescription;
        textWhereYouFind.text = creature.whereYouFindIt;

        if (creature.creatureSprite != null)
        {
            spriteCreature.sprite = creature.creatureSprite;
            spriteCreature.gameObject.SetActive(true);
            if (slotBackground != null)
                slotBackground.SetActive(true);
        }
    }
}