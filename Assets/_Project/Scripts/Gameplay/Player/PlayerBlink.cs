using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections; // Necesario para usar Corrutinas

public class PlayerBlink : MonoBehaviour
{
    [Header("Blink Settings")]
    public float blinkDistance = 5f;
    public float cooldown = 1f;
    
    // NUEVO: Tiempo de espera para que se vea la animación antes del teletransporte
    [Tooltip("Tiempo en segundos que se muestra la animación antes de mover al personaje.")]
    public float blinkDelay = 0.5f; 

    [Header("VFX")]
    public GameObject blinkVFX;

    private PlayerInput playerInput;
    private InputAction blinkAction;
    private float LastBlinkTime = -10f;
    private Camera mainCamera;
    private Animator anim;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        blinkAction = playerInput.actions["Blink"];
        mainCamera = Camera.main;
        anim = GetComponentInChildren<Animator>();
    }

    void OnEnable() { blinkAction.performed += OnBlink; }
    void OnDisable() { blinkAction.performed -= OnBlink; }

    private void OnBlink(InputAction.CallbackContext context)
    {
        if (Time.time - LastBlinkTime < cooldown) return;

        // NUEVO: En lugar de hacerlo directo, llamamos a la Corrutina
        StartCoroutine(BlinkSequence());
    }

    // NUEVO: Esta es la secuencia que maneja el tiempo
    private IEnumerator BlinkSequence()
    {
        LastBlinkTime = Time.time; // Ponemos el cooldown al inicio para evitar spam

        // 1. INICIAR ANIMACIÓN (Anticipación)
        if (anim != null)
        {
            anim.SetTrigger("Dash");
        }

        // 2. ESPERAR (Aquí es donde ocurre la magia del retraso)
        // Esto pausa este método, pero el juego sigue corriendo
        yield return new WaitForSeconds(blinkDelay);

        // 3. LOGICA DE TELETRANSPORTE (Después de la espera)
        Vector3 blinkTarget = GetBlinkTarget();
        
        // VFX en el origen (donde estaba)
        SpawnVFX(transform.position); 

        // MOVER AL PERSONAJE INSTANTÁNEAMENTE
        transform.position = blinkTarget;

        // VFX en el destino (donde apareció)
        SpawnVFX(blinkTarget); 
    }

    private Vector3 GetBlinkTarget()
    {
        Vector2 mouseScreen = Mouse.current.position.ReadValue();
        Ray ray = mainCamera.ScreenPointToRay(mouseScreen);
        Vector3 mouseWorldPosition = transform.position;

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            mouseWorldPosition = hit.point;
        }

        Vector3 direction = (mouseWorldPosition - transform.position).normalized;
        if (direction == Vector3.zero) return transform.position;

        Vector3 target = transform.position + direction * blinkDistance;
        return new Vector3(target.x, transform.position.y, target.z);
    }

    private void SpawnVFX(Vector3 position)
    {
        if (blinkVFX == null) return;
        GameObject vfx = Instantiate(blinkVFX, position, Quaternion.identity);
        Destroy(vfx, 2f);
    }
}