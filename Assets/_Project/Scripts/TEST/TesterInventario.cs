using UnityEngine;
using UnityEngine.InputSystem;

public class TesterInventario : MonoBehaviour
{
    [Header("Catálogo de Pruebas")]
    public ItemData pocionVida;
    public ItemData pocionMana;

    void Update()
    {
        if (Keyboard.current == null) return;

        // Presiona V para Vida
        if (Keyboard.current.vKey.wasPressedThisFrame)
        {
            AddWithCheck(pocionVida);
        }

        // Presiona M para Maná
        if (Keyboard.current.mKey.wasPressedThisFrame)
        {
            AddWithCheck(pocionMana);
        }
    }

    private void AddWithCheck(ItemData item)
    {
        if (item != null && InventoryManager.Instance != null)
        {
            InventoryManager.Instance.AddItem(item, 1);
            Debug.Log($"Añadida: {item.itemName} al inventario.");
        }
        else
        {
            Debug.LogWarning("Falta asignar el ítem en el Inspector o no hay InventoryManager.");
        }
    }
}