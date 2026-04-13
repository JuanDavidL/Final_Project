using UnityEngine;

public class IngredientDispenser : MonoBehaviour
{
    [Header("Configuración de Datos")]
    [SerializeField] private ItemData itemAsociado; 
    
    [Header("Configuración Visual")]
    [SerializeField] private Color colorDelRecurso; 
    [SerializeField] private Transform puntoDeSalida;

    [Header("Física")]
    [SerializeField] private float fuerzaLanzamiento = 3f;

    private void OnMouseDown()
    {
        // 1. Verificación de seguridad (Early Return)
        if (InventoryManager.Instance == null || IngredientPool.Instance == null) return;

        // 2. Lógica de negocio: ¿Podemos extraer del inventario?
        if (InventoryManager.Instance.RemoveItem(itemAsociado, 1))
        {
            SpawnFromPool();
        }
        else
        {
            Debug.LogWarning($"Inventario insuficiente para: {itemAsociado.itemName}");
            // Aquí podrías disparar un sonido de "error"
        }
    }

    private void SpawnFromPool()
    {
        // 3. En lugar de Instantiate, pedimos al Pool Nativo
        PoolableItem cubo = IngredientPool.Instance.Get();
        
        // 4. Posicionamos el objeto antes de lanzarlo
        cubo.transform.position = puntoDeSalida.position;
        cubo.transform.rotation = puntoDeSalida.rotation;

        // 5. Inyectamos los datos y el color (Reemplaza la lógica de ItemProxy)
        cubo.Initialize(itemAsociado, colorDelRecurso);

        // 6. Aplicamos física
        Rigidbody rb = cubo.GetComponent<Rigidbody>();
        if (rb != null)
        {
            // Reset de velocidad para que no salga disparado si fue reutilizado
            rb.linearVelocity = Vector3.zero; 
            
            // Dispersión aleatoria en el eje X (derecha/izquierda del frasco)
            Vector3 dispersion = puntoDeSalida.right * Random.Range(-0.3f, 0.3f);
            Vector3 direccionFinal = (puntoDeSalida.forward + dispersion).normalized;
            
            rb.AddForce(direccionFinal * fuerzaLanzamiento, ForceMode.Impulse);
        }
    }
}