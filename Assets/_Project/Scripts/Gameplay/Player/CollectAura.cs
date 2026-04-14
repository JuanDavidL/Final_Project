using UnityEngine;
using UnityEngine.InputSystem;

public class CollectAura : MonoBehaviour
{
    public float attractRadius = 10f;
    public MagicBook magicBook;


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
       if (magicBook != null)
        {
            magicBook.SetState(MagicBook.BookState.Collecting);
        }
       
       
        Collider[] colliders = Physics.OverlapSphere(transform.position, attractRadius);
        foreach (Collider collider in colliders)
        {
            CollectOrb orb = collider.GetComponent<CollectOrb>();
            if (orb != null)
            {
                orb.StartAttract(magicBook.transform);
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

        if (magicBook != null)
        {
            magicBook.SetState(MagicBook.BookState.Orbiting);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (magicBook != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(magicBook.GetBookPosition(), attractRadius);
        }
    }
}
