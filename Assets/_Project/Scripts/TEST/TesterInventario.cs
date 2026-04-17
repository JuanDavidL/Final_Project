using UnityEngine;
using UnityEngine.InputSystem; // Necesario para el nuevo sistema

public class TesterInventario : MonoBehaviour
{
    public ItemData pocionPrueba; 

void Update()
{
    if (Keyboard.current != null && Keyboard.current.tKey.wasPressedThisFrame)
    {
        // 1. Comprobamos si arrastraste la poción al Inspector
        if (pocionPrueba == null)
        {
            Debug.LogError("¡Oye! Olvidaste asignar la poción en el Inspector de TesterInventario.");
            return;
        }

        // 2. Comprobamos si el Inventario existe en la escena
        if (InventoryManager.Instance == null)
        {
            Debug.LogError("¡No hay ningún InventoryManager en la escena! Asegúrate de tener uno.");
            return;
        }

        // Si todo está bien, procedemos
        InventoryManager.Instance.AddItem(pocionPrueba, 1);
        Debug.Log("Trampa exitosa: Añadida " + pocionPrueba.itemName);
    }
}
}