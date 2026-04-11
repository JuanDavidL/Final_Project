using UnityEngine;
using UnityEngine.InputSystem;

public class CollectAura : MonoBehaviour
{
    public Transform collectPoint;
    public float attractRadius = 10f;
    private PlayerInput playerInput;
    private InputAction CollectAction;

    void Awake()
    {
        playerInput = FindFirstObjectByType<PlayerInput>();
        CollectAction = playerInput.actions["Collect"];
    }

    void OnEnable()
    {
        CollectAction.performed += OnCollectStarted;
        CollectAction.canceled += OnCollectCanceled;
    }

    void OnDisable()
    {
        CollectAction.performed -= OnCollectStarted;
        CollectAction.canceled -= OnCollectCanceled;
    }

    private void OnCollectStarted (InputAction.CallbackContext context)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, attractRadius);
        foreach (Collider collider in colliders)
        {
            CollectOrb orb = collider.GetComponent<CollectOrb>();
            if (orb != null)
            {
                orb.StartAttract(collectPoint);
            }
        }

    }

    private void OnCollectCanceled(InputAction.CallbackContext context)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, attractRadius);
        foreach (Collider collider in colliders)
        {
            CollectOrb orb = collider.GetComponent<CollectOrb>();
            if (orb != null)
            {
                orb.StopAttract();
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (collectPoint != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(collectPoint.position, attractRadius);
        }
    }
}
