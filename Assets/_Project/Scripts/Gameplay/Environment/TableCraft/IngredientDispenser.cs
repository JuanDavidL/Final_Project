using UnityEngine;
using UnityEngine.EventSystems;

public class IngredientDispenser : MonoBehaviour, IPointerClickHandler
{
    [Header("Configuración de Datos")]
    [SerializeField] private ItemData itemAsociado;

    [Header("Configuración Visual")]
    [SerializeField] private Color colorDelRecurso;
    [SerializeField] private Transform puntoDeSalida;

    [Header("Física")]
    [SerializeField] private float fuerzaLanzamiento = 3f;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (InventoryManager.Instance != null && InventoryManager.Instance.RemoveItem(itemAsociado, 1))
        {
            SpawnFromPool();
        }
    }

    private void SpawnFromPool()
    {
        PoolableItem cubo = IngredientPool.Instance.Get();
        cubo.transform.position = puntoDeSalida.position;
        cubo.Initialize(itemAsociado, colorDelRecurso);

        Rigidbody rb = cubo.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            Vector3 direccion = (puntoDeSalida.forward + puntoDeSalida.right * Random.Range(-0.3f, 0.3f)).normalized;
            rb.AddForce(direccion * fuerzaLanzamiento, ForceMode.Impulse);
        }
    }
}