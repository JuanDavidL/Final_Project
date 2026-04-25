using UnityEngine;

public class CameraRaycaster : MonoBehaviour
{
    [Header("Ajustes del Escáner")]
    public float scanDistance = 10f; // Qué tan lejos puede "ver" el jugador
    public LayerMask interactableLayer; // Opcional: para optimizar el láser

    private DialogueController belController;

    void Start()
    {
        // Buscamos a B.E.L. al iniciar la escena
        belController = FindObjectOfType<DialogueController>();
    }

    void Update()
    {
        // Si el juego está pausado (TimeScale 0), no escaneamos
        if (Time.timeScale == 0f || belController == null)
            return;

        // Disparamos el láser desde el centro de la cámara hacia adelante
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        // Dibujamos el láser en la vista 'Scene' de Unity para que puedas depurar
        Debug.DrawRay(ray.origin, ray.direction * scanDistance, Color.cyan);

        // Si el láser choca con algo en menos de 10 metros...
        if (Physics.Raycast(ray, out hit, scanDistance))
        {
            string hitTag = hit.collider.tag;

            // Verificamos si es una de nuestras estaciones
            if (hitTag == "Transmuter" || hitTag == "NodeSystem" || hitTag == "ClientSystem")
            {
                belController.TryStartContextDialogue(hitTag);
            }
        }
    }
}
