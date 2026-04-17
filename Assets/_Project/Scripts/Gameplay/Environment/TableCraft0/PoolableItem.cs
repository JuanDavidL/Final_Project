using UnityEngine;

public class PoolableItem : MonoBehaviour
{
    public ItemData data;
    private Renderer _renderer;
    private Rigidbody _rb;

    void Awake()
    {
        _renderer = GetComponent<Renderer>();
        _rb = GetComponent<Rigidbody>();
    }

    // Este método se llama cada vez que el Pool nos "saca" a la mesa
    public void Initialize(ItemData newData, Color color)
    {
        data = newData;
        _renderer.material.color = color;
        
        // Resetear física para que no herede velocidades viejas
        if(_rb != null)
        {
            _rb.linearVelocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
        }
    }
}