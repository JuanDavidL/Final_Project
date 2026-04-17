using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems; // ¡IMPORTANTE!
using System.Collections;

// Al agregar IPointerClickHandler, el EventSystem nos avisará del clic
public class PhysicalButton : MonoBehaviour, IPointerClickHandler
{
    public UnityEvent OnClicked;

    [Header("Ajustes de Animación")]
    [SerializeField] private float pushDepth = 0.05f; 
    [SerializeField] private float speed = 15f;       
    
    private Vector3 originalPos;
    private bool isMoving = false;

    void Start() => originalPos = transform.localPosition;

    // Esta función reemplaza a OnMouseDown y es mucho más confiable
    public void OnPointerClick(PointerEventData eventData)
    {
        if (isMoving) return;

        Debug.Log("¡Clic detectado por IPointerClickHandler en: " + gameObject.name);
        
        OnClicked?.Invoke();
        StartCoroutine(AnimatePress());
    }

    IEnumerator AnimatePress()
    {
        isMoving = true;
        Vector3 targetPos = originalPos + (Vector3.down * pushDepth); 

        while (Vector3.Distance(transform.localPosition, targetPos) > 0.001f)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, Time.deltaTime * speed);
            yield return null;
        }
        while (Vector3.Distance(transform.localPosition, originalPos) > 0.001f)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, originalPos, Time.deltaTime * speed);
            yield return null;
        }
        transform.localPosition = originalPos;
        isMoving = false;
    }
}