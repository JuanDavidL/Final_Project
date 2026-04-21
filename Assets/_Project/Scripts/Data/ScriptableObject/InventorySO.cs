using System;
using System.Collections.Generic;
using UnityEngine;

// [CreateAssetMenu(fileName = "PlayerInventory", menuName = "Inventory/System")]
public class InventorySO : ScriptableObject
{
    // Usamos una lista serializable para verla en el inspector
    [SerializeField] private List<InventorySlot> slots = new List<InventorySlot>();

    // Evento para que la UI o el Caldero se enteren de cambios sin preguntar cada frame
    public event Action OnInventoryChanged;

    public int AddItem(ItemDataOld data, int amount)
    {
        int remaining = amount;

        // 1. Intentar llenar slots existentes primero
        foreach (var slot in slots.FindAll(s => s.item == data && s.quantity < data.maxStack))
        {
            int spaceInSlot = data.maxStack - slot.quantity;
            int addAmount = Mathf.Min(remaining, spaceInSlot);

            slot.quantity += addAmount;
            remaining -= addAmount;

            if (remaining <= 0) break;
        }

        // 2. Si aún queda, intentar crear nuevos slots
        while (remaining > 0)
        {
            // Supongamos que hay un límite de slots totales (ej. 20)
            if (slots.Count >= 20) return remaining;

            int addAmount = Mathf.Min(remaining, data.maxStack);
            slots.Add(new InventorySlot { item = data, quantity = addAmount });
            remaining -= addAmount;
        }

        OnInventoryChanged?.Invoke();
        return remaining; // 0 si se guardó todo, > 0 si quedó algo fuera
    }
    public bool RemoveItem(ItemData data, int amount)
    {
        InventorySlot slot = slots.Find(s => s.item == data);

        if (slot != null && slot.quantity >= amount)
        {
            slot.quantity -= amount;
            if (slot.quantity <= 0) slots.Remove(slot);

            OnInventoryChanged?.Invoke();
            return true;
        }
        return false;
    }
}

[Serializable]
public class InventorySlot
{
    public ItemDataOld item;
    public int quantity;
}