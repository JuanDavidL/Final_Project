using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class PhysicalButton : MonoBehaviour
{
    public UnityEvent OnClicked;

    [Header("Ajustes de Animación")]
    [SerializeField] private float pushDepth = 0.02f; // Qué tanto se hunde
    [SerializeField] private float speed = 10f;       // Rapidez del movimiento
    
    private Vector3 originalLocalPos;
    private bool isPushing = false;

    void Start()
    {
        originalLocalPos = transform.localPosition;
    }

    private void OnMouseDown()
    {
        if (isPushing) return; // Evita clics múltiples mientras se mueve

        if (OnClicked != null)
        {
            OnClicked.Invoke();
            StartCoroutine(PushAnimation());
        }
    }

    IEnumerator PushAnimation()
    {
        isPushing = true;

        // Definimos la posición hacia abajo (usando el eje local del botón)
        Vector3 targetPos = originalLocalPos + (Vector3.down * pushDepth);

        // Movimiento hacia ABAJO
        while (Vector3.Distance(transform.localPosition, targetPos) > 0.001f)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, Time.deltaTime * speed);
            yield return null;
        }

        // Movimiento hacia ARRIBA (Volver al original)
        while (Vector3.Distance(transform.localPosition, originalLocalPos) > 0.001f)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, originalLocalPos, Time.deltaTime * speed);
            yield return null;
        }

        transform.localPosition = originalLocalPos;
        isPushing = false;
    }
}