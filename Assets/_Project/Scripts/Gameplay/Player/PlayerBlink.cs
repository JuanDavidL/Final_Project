using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBlink : MonoBehaviour
{
    [Header("Blink")]
    public float blinkDistance = 5f;
    public float cooldown = 1f;

    [Header("VFX")]
    public GameObject blinkVFX;

    private PlayerInput playerInput;
    private InputAction blinkAction;
    private float LastBlinkTime = -10f;
    private Camera mainCamera;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        blinkAction = playerInput.actions["Blink"];
        mainCamera = Camera.main;
    }

    void OnEnable()
    {
        blinkAction.performed += OnBlink;
    }

    void OnDisable()
    {
        blinkAction.performed -= OnBlink;
    }

    private void OnBlink (InputAction.CallbackContext context)
    {
        if (Time.time - LastBlinkTime < cooldown) return;

        Vector3 blinkTarget = GetBlinkTarget();
        SpawnVFX(transform.position);
        transform.position = blinkTarget;
        SpawnVFX(blinkTarget);
        LastBlinkTime = Time.time;
    }

    private Vector3 GetBlinkTarget()
    {
        Vector3 mouseScreen = Mouse.current.position.ReadValue();
        Ray ray = mainCamera.ScreenPointToRay(mouseScreen);

        Vector3 mouseWorldPosition = transform.position;

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            mouseWorldPosition = hit.point;
        }

        Vector3 direction = (mouseWorldPosition - transform.position).normalized;
        Vector3 target = transform.position + direction * blinkDistance;
        
        return new Vector3(target.x, transform.position.y, target.z);
    }

    private void SpawnVFX(Vector3 position)
    {
        if (blinkVFX == null) return;
        {
            GameObject vfx = Instantiate(blinkVFX, position, Quaternion.identity);
            Destroy(vfx, 2f);
        }
    }

    void Update()
    {
        
    }
}
