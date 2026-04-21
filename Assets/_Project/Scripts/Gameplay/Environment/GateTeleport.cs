using UnityEngine;
using System.Collections;

public class GateTeleport : MonoBehaviour
{
    [Header("Configuración de Teletransporte")]
    [SerializeField] private Transform spawnPoint; // El hijo "Punto de Spawn"
    [SerializeField] private float transitionDelay = 0.5f; // Tiempo de espera antes de moverlo
    
    [Header("Seguridad")]
    private bool _isTeleporting = false;
    private static bool _globalCooldown = false; // Evita que todas las puertas se activen a la vez

    private void OnTriggerEnter(Collider other)
    {
        // 1. Validamos que sea el Jugador y que no estemos ya en un proceso
        if (other.CompareTag("Player") && !_isTeleporting && !_globalCooldown)
        {
            StartCoroutine(DoTeleport(other.transform));
        }
    }

    private IEnumerator DoTeleport(Transform playerTransform)
    {
        _isTeleporting = true;
        _globalCooldown = true;

        Debug.Log("Iniciando transición de puerta...");

        // 2. Esperamos el tiempo estimado (útil para efectos de fundido a negro)
        yield return new WaitForSeconds(transitionDelay);

        // 3. MOVIMIENTO
        // Si usas CharacterController, debes desactivarlo un frame para que no interfiera
        CharacterController cc = playerTransform.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        playerTransform.position = spawnPoint.position;

        if (cc != null) cc.enabled = true;

        Debug.Log("Jugador desplazado con éxito.");

        // 4. Cooldown de seguridad para no volver por error
        yield return new WaitForSeconds(1.0f); 
        
        _isTeleporting = false;
        _globalCooldown = false;
    }

    // Método para la validación futura de enemigos
    public bool CanOpenGate()
    {
        // Aquí irá tu lógica de contar enemigos en la escena
        return true; 
    }
}