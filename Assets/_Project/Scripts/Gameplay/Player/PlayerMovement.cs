using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    [Header("Camera Settings")]
    public Transform cameraTransform;
    public Vector3 cameraOffset = new Vector3(0f, 5f, -10f);
    public float cameraSmooth = 5f;

    // Referencias privadas
    private Vector2 moveInput;
    private PlayerInput playerInput;
    private InputAction moveAction;
    private Rigidbody rb;
    
    // Referencias a componentes del hijo (MageHoodMove)
    private SpriteRenderer spriteRenderer;
    private Animator anim;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody>();

        // BUSCAR COMPONENTES EN EL HIJO
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        anim = GetComponentInChildren<Animator>();

        if (playerInput != null)
        {
            moveAction = playerInput.actions["Move"];
        }
    }

    void OnEnable() { moveAction?.Enable(); }
    void OnDisable() { moveAction?.Disable(); }

    void FixedUpdate()
    {
        moveInput = moveAction.ReadValue<Vector2>();
        
        Vector3 moveDirection = new Vector3(moveInput.x, 0f, moveInput.y);
        rb.linearVelocity = new Vector3(moveDirection.x * moveSpeed, rb.linearVelocity.y, moveDirection.z * moveSpeed);

        // --- ACTUALIZACIÓN DE ANIMACIONES Y VISUALES ---
        
        // Comprobamos que existan los componentes en el hijo
        if (anim != null)
        {
            // Calculamos la magnitud del movimiento (da un valor positivo si te mueves)
            // Esto activará Speed > 0 en tu Animator
            float currentSpeed = moveInput.magnitude; 
            anim.SetFloat("Speed", currentSpeed);
        }

        if (spriteRenderer != null) 
        {
            // Voltear el sprite
            if (moveInput.x > 0.1f)
            {
                spriteRenderer.flipX = false; // Derecha
            }
            else if (moveInput.x < -0.1f)
            {
                spriteRenderer.flipX = true;  // Izquierda
            }
        }
    }

    void LateUpdate()
    {
        if (cameraTransform == null) return;
        Vector3 targetPosition = transform.position + cameraOffset;
        cameraTransform.position = Vector3.Lerp(cameraTransform.position, targetPosition, cameraSmooth * Time.deltaTime);
    }
}