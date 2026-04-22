using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventario/Item")]

public class ItemData : ScriptableObject
{
    public int id;
    public string itemName;
    public Sprite itemIcon;
    public GameObject potionPrefab;     
    [TextArea] public string itemDescription; 
}
