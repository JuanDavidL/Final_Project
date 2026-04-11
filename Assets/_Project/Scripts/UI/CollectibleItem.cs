using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    public ItemData item;
    public int amount = 1;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            InventoryManager.Instance.AddItem(item, amount);
            Destroy(gameObject);
        }
    }
}
