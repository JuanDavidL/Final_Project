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
    
    // Referencia al SpriteRenderer que está en el objeto hijo (MageHoodMove)
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        // Obtener componentes en el objeto principal (Player)
        playerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody>();

        // BUSCAR EL COMPONENTE EN EL HIJO (MageHoodMove)
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (playerInput != null)
        {
            moveAction = playerInput.actions["Move"];
        }
    }

    void OnEnable()
    {
        moveAction?.Enable();
    }

    void OnDisable()
    {
        moveAction?.Disable();
    }

    void FixedUpdate()
    {
        // Leer entrada
        moveInput = moveAction.ReadValue<Vector2>();
        
        // Aplicar velocidad al Rigidbody
        Vector3 moveDirection = new Vector3(moveInput.x, 0f, moveInput.y);
        rb.linearVelocity = new Vector3(moveDirection.x * moveSpeed, rb.linearVelocity.y, moveDirection.z * moveSpeed);

        // Lógica para voltear el Sprite horizontalmente
        if (spriteRenderer != null) 
        {
            if (moveInput.x > 0.1f)
            {
                spriteRenderer.flipX = false; // Mirar a la derecha
            }
            else if (moveInput.x < -0.1f)
            {
                spriteRenderer.flipX = true;  // Mirar a la izquierda
            }
        }
    }

    void LateUpdate()
    {
        // Seguimiento de cámara
        if (cameraTransform == null) return;

        Vector3 targetPosition = transform.position + cameraOffset;
        cameraTransform.position = Vector3.Lerp(cameraTransform.position, targetPosition, cameraSmooth * Time.deltaTime);
    }
}