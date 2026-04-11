using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Inventory/NewItem")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon; // Para el 2D
    public GameObject prefab3D; // Para la mesa 3D
    public int maxStack = 20;
}