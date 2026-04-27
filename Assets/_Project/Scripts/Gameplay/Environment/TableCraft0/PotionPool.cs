using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PotionPool : MonoBehaviour
{
    public static PotionPool Instance;

    [SerializeField]
    private int defaultCapacity = 10;

    [SerializeField]
    private int maxSize = 20;

    // Diccionario que guarda un Pool independiente para cada Prefab
    private Dictionary<GameObject, ObjectPool<PoolableItem>> _poolsDict =
        new Dictionary<GameObject, ObjectPool<PoolableItem>>();

    // Variable temporal para saber qué prefab instanciar en CreateItem
    public GameObject _currentPrefabRequest;

    void Awake()
    {
        Instance = this;
    }

    public PoolableItem Get(GameObject prefab)
    {
        // 1. Si no existe un pool para este prefab, lo creamos
        if (!_poolsDict.ContainsKey(prefab))
        {
            CreatePoolForPrefab(prefab);
        }

        // 2. Pedimos el objeto al pool correspondiente
        return _poolsDict[prefab].Get();
    }

    private void CreatePoolForPrefab(GameObject prefab)
    {
        // Usamos una variable local para el closure de la lambda
        GameObject prefabToInstantiate = prefab;

        var newPool = new ObjectPool<PoolableItem>(
            createFunc: () =>
            {
                GameObject obj = Instantiate(prefabToInstantiate, transform);
                return obj.GetComponent<PoolableItem>();
            },
            actionOnGet: (item) => item.gameObject.SetActive(true),
            actionOnRelease: (item) => item.gameObject.SetActive(false),
            actionOnDestroy: (item) => Destroy(item.gameObject),
            collectionCheck: true,
            defaultCapacity: defaultCapacity,
            maxSize: maxSize
        );

        _poolsDict.Add(prefab, newPool);
    }

    public void Release(GameObject prefab, PoolableItem item)
    {
        if (_poolsDict.ContainsKey(prefab))
        {
            _poolsDict[prefab].Release(item);
        }
    }
}
