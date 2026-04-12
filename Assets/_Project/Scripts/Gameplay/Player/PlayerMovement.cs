using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;

    private Vector2 moveInput;
    private PlayerInput playerInput;
    private InputAction moveAction;
    private Rigidbody rb;

    [Header("Camera")]
    public Transform cameraTransform;
    public Vector3 cameraOffset = new Vector3(0f, 5f, -10f);
    public float cameraSmooth = 5f;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];
        rb = GetComponent<Rigidbody>();
    }

    void OnEnable()
    {
        moveAction.Enable();
    }

    void OnDisable()
    {
        moveAction.Disable();
    }

    void FixedUpdate()
    {
        moveInput = moveAction.ReadValue<Vector2>();
        
        Vector3 moveDirection = new Vector3(moveInput.x, 0f, moveInput.y);
        rb.linearVelocity = new Vector3(moveDirection.x * moveSpeed, rb.linearVelocity.y, moveDirection.z * moveSpeed);

    }
    // Update is called once per frame
    void LateUpdate()
    {
        if (cameraTransform == null) return;
        {
           Vector3 targetPosition = transform.position + cameraOffset;
           cameraTransform.position = Vector3.Lerp(cameraTransform.position, targetPosition, cameraSmooth * Time.deltaTime);
        }
    }
}
