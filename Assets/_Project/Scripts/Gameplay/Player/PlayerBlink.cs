using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerBlink : BaseAbility
{
    [Header("Blink Settings")]
    public float blinkDistance = 5f;    
    [Tooltip("Tiempo en segundos que se muestra la animación antes de mover al personaje.")]
    public float blinkDelay = 0.5f; 
    [Header("VFX")]
    public GameObject blinkVFX;

    private PlayerInput playerInput;
    private InputAction blinkAction;
    private Camera mainCamera;
    private Animator anim;

    // Nota: 'lastUsedTime' viene de BaseAbility y lo usamos para el cooldown

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        blinkAction = playerInput.actions["Blink"];
        mainCamera = Camera.main;
        anim = GetComponentInChildren<Animator>();
    }

    void OnEnable() 
    { 
        if (blinkAction != null) blinkAction.performed += OnBlink; 
    }

    void OnDisable() 
    { 
        if (blinkAction != null) blinkAction.performed -= OnBlink; 
    }

    private void OnBlink(InputAction.CallbackContext context)
    {
        // Usamos IsOnCooldown() que ya está definido en BaseAbility
        if (IsOnCooldown()) return;

        StartCoroutine(BlinkSequence());
    }

    private IEnumerator BlinkSequence()
    {
        // Seteamos el tiempo de uso para que el cooldown empiece a contar
        lastUsedTime = Time.time; 

        // 1. Iniciar Animación de Dash
        if (anim != null)
        {
            anim.SetTrigger("Dash");
        }

        // 2. Esperar el retraso de la animación
        yield return new WaitForSeconds(blinkDelay);

        // 3. Lógica de Teletransporte
        Vector3 blinkTarget = GetBlinkTarget();
        
        // VFX en la posición inicial
        SpawnVFX(transform.position); 

        // Mover al personaje (solo en X y Z para no enterrarlo en el suelo)
        transform.position = blinkTarget;

        // VFX en la posición final
        SpawnVFX(blinkTarget); 
    }

    public override void Use()
    {
        // Este método es obligatorio por BaseAbility. 
        // No lo usamos directamente porque el Blink se activa por Input directo (OnBlink).
    }

    private Vector3 GetBlinkTarget()
    {
        Vector2 mouseScreen = Mouse.current.position.ReadValue();
        Ray ray = mainCamera.ScreenPointToRay(mouseScreen);
        Vector3 mouseWorldPosition = transform.position;

        // Detectar el suelo o terreno
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            mouseWorldPosition = hit.point;
        }

        Vector3 direction = (mouseWorldPosition - transform.position).normalized;
        if (direction == Vector3.zero) return transform.position;

        // Calculamos el punto final basado en la distancia máxima
        Vector3 target = transform.position + direction * blinkDistance;
        
        // Mantenemos la Y original del jugador para evitar que atraviese el terreno
        return new Vector3(target.x, transform.position.y, target.z);
    }

    private void SpawnVFX(Vector3 position)
    {
        if (blinkVFX == null) return;
        GameObject vfx = Instantiate(blinkVFX, position, Quaternion.identity);
        Destroy(vfx, 2f);
    }
}