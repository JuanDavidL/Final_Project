using UnityEngine;

public class MachineDeposit : MonoBehaviour
{
    // Referencia a la lógica de la máquina para avisarle que hay ingredientes nuevos
    [SerializeField] private ProcessingMachineLogic machineLogic;

    public void TryDeposit(ItemData itemData)
    {

        Debug.Log($"TryDeposit llamado con: {itemData?.itemName}");
        Debug.Log($"machineLogic es: {machineLogic}");
        Debug.Log($"InventoryManager es: {InventoryManager.Instance}");
        if (InventoryManager.Instance.RemoveItem(itemData, 1))
        {
            Debug.Log($"Depositado: {itemData.itemName}");
            machineLogic.AddIngredient(itemData);
        }
        else
        {
            Debug.LogWarning("No hay suficiente material.");
        }
    }
}
