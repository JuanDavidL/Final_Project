using UnityEngine;

public class MachineDeposit : MonoBehaviour
{
    // Referencia a la lógica de la máquina para avisarle que hay ingredientes nuevos
    [SerializeField] private ProcessingMachineLogic machineLogic;

    private void OnTriggerEnter(Collider other)
    {
        // Si el frasco entra en el área del depósito
        if (other.TryGetComponent<DraggableItem>(out DraggableItem jar))
        {
            // Intentamos restar 1 unidad del inventario global
            if (InventoryManager.Instance.RemoveItem(jar.itemContenido, 1))
            {
                Debug.Log($"Depositado: {jar.itemContenido.itemName}. Restando del inventario.");
                machineLogic.AddIngredient(jar.itemContenido);
            }
            else
            {
                Debug.LogWarning("No queda suficiente material en este frasco.");
            }
        }
    }
}
