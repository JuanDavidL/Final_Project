using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Configuración de Datos")]
    public ItemData itemContenido;

    private Vector3 _posicionInicial;
    private Quaternion _rotacionInicial;
    private Camera _mainCamera;
    private Rigidbody _rb;
    private float _zDistance;

    void Awake()
    {
        _mainCamera = Camera.main;
        _rb = GetComponent<Rigidbody>();
        // posición inicial en el estante
        _posicionInicial = transform.position;
        _rotacionInicial = transform.rotation;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _rb.isKinematic = true;
        _zDistance = _mainCamera.WorldToScreenPoint(transform.position).z;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 mousePos = eventData.position;
        mousePos.z = _zDistance;
        Vector3 worldPos = _mainCamera.ScreenToWorldPoint(mousePos);

        transform.position = new Vector3(worldPos.x, worldPos.y + 0.2f, worldPos.z);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _rb.isKinematic = false;

        if (IsOverDeposit())
        {
            // Aquí notificaremos a la máquina que reste el material // PENDIENTE
            Debug.Log($"Tarro de {itemContenido.itemName} usado.");
        }

        RegresarAlEstante();
    }

    private void RegresarAlEstante()
    {
        transform.position = _posicionInicial;
        transform.rotation = _rotacionInicial;
        _rb.linearVelocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
    }

    private bool IsOverDeposit()
    {
        // Lanzamos el rayo
        bool hitSomething = Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 2f);

        // CRÍTICO: Primero verificamos si el rayo golpeó algo antes de preguntar por el Tag
        if (hitSomething && hit.collider != null)
        {
            return hit.collider.CompareTag("Deposito");
        }

        return false;
    }
}