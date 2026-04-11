using UnityEngine;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [System.Serializable] public class InventorySlot
    {
        public ItemData item;
        public int quantity;
    }

    public List<InventorySlot> inventory = new List<InventorySlot>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    public void AddItem(ItemData newItem, int amount = 1)
    {
        InventorySlot existing = inventory.Find(slot => slot.item == newItem);
        if (existing != null)
        {
            existing.quantity += amount;
        }
        else
        {
            inventory.Add(new InventorySlot { item = newItem, quantity = amount });
        }   
    }
}
