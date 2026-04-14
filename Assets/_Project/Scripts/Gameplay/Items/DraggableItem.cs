using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Camera _mainCamera;
    private Rigidbody _rb;
    private float _zDistance;

    void Awake()
    {
        _mainCamera = Camera.main;
        _rb = GetComponent<Rigidbody>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _rb.isKinematic = true;
        _zDistance = _mainCamera.WorldToScreenPoint(transform.position).z;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 mousePos = eventData.position; // El evento ya nos da la posición del mouse
        mousePos.z = _zDistance;
        Vector3 worldPos = _mainCamera.ScreenToWorldPoint(mousePos);

        transform.position = new Vector3(worldPos.x, worldPos.y + 0.5f, worldPos.z);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _rb.isKinematic = false;
        CheckIfOverCauldron();
    }

    private void CheckIfOverCauldron()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 5f))
        {
            if (hit.collider.CompareTag("Caldero"))
            {
                Debug.Log("Soltado con éxito sobre el caldero.");
            }
        }
    }
}