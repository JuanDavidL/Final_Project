using UnityEngine;
using System.Collections.Generic;
using System;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [System.Serializable]
    public class InventorySlot
    {
        public ItemData item;
        public int quantity;
    }

    public List<InventorySlot> inventory = new List<InventorySlot>();

    // evento para que el sistema de pociones se actualice visualmente
    public event Action OnInventoryUpdated;

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

        OnInventoryUpdated?.Invoke(); // Notificar cambio
    }

    // MÉTODO para que "sacar" cosas a la mesa 3D
    public bool RemoveItem(ItemData itemToRemove, int amount)
    {
        InventorySlot existing = inventory.Find(slot => slot.item == itemToRemove);
        if (existing != null && existing.quantity >= amount)
        {
            existing.quantity -= amount;
            if (existing.quantity <= 0) inventory.Remove(existing);

            OnInventoryUpdated?.Invoke();
            return true; // Se pudo sacar el material
        }
        return false; // No hay suficiente material
    }
}