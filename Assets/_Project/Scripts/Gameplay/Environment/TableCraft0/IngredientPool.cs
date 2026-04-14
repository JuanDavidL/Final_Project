using UnityEngine;
using UnityEngine.Pool; // ¡La clave de la magia!

public class IngredientPool : MonoBehaviour
{
    public static IngredientPool Instance;

    [SerializeField] private PoolableItem prefabCubo;
    [SerializeField] private int defaultCapacity = 20;
    [SerializeField] private int maxSize = 50;

    private ObjectPool<PoolableItem> _pool;

    void Awake()
    {
        Instance = this;

        // Configuración del Pool Nativo
        _pool = new ObjectPool<PoolableItem>(
            createFunc: CreateItem,
            actionOnGet: OnGetItem,
            actionOnRelease: OnReleaseItem,
            actionOnDestroy: OnDestroyItem,
            collectionCheck: true, // Evita devolver el mismo objeto dos veces
            defaultCapacity: defaultCapacity,
            maxSize: maxSize
        );
    }

    // --- FUNCIONES DEL POOL ---
    private PoolableItem CreateItem() => Instantiate(prefabCubo, transform);
    
    private void OnGetItem(PoolableItem item) => item.gameObject.SetActive(true);

    private void OnReleaseItem(PoolableItem item) => item.gameObject.SetActive(false);

    private void OnDestroyItem(PoolableItem item) => Destroy(item.gameObject);

    // --- MÉTODOS PÚBLICOS ---
    public PoolableItem Get() => _pool.Get();

    public void Release(PoolableItem item) => _pool.Release(item);
}