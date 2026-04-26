using UnityEngine;

[CreateAssetMenu(fileName = "NewCreature", menuName = "Info/CreatureData")]
public class CreatureData : ScriptableObject
{
    public string creatureName;
    public Sprite creatureSprite;
    [TextArea] public string creatureDescription;
    [TextArea] public string whereYouFindIt;
}