using UnityEngine;

public class ItemContainer : MonoBehaviour
{
    public ItemData data;
    public int currentQuantity;

    // Puedes llamar a esto cuando el jugador interactúa con el frasco
    public void SetupContainer(ItemData item, int amount)
    {
        data = item;
        currentQuantity = amount;
        // Aquí podrías cambiar el color del frasco según el item
    }
}