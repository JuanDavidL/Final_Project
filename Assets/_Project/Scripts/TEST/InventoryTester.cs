using UnityEngine;

public class InventoryTester : MonoBehaviour
{
    [SerializeField] private InventorySO inventory;
    [SerializeField] private ItemDataOld resourceToTest;
    [SerializeField] private int amountToPickUp = 150;

    [ContextMenu("Test Add Item")] // Esto permite ejecutarlo desde el Inspector
    public void TestPickup()
    {
        int remaining = inventory.AddItem(resourceToTest, amountToPickUp);
        
        if(remaining > 0)
            Debug.Log($"<color=yellow>Inventario lleno!</color> Quedaron {remaining} unidades fuera.");
        else
            Debug.Log($"<color=green>Éxito:</color> Todos los items guardados.");
    }
}

