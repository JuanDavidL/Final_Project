using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    public ItemData item;
    public int amount = 1;
    public bool hasLuch = true;

    private Rigidbody rb;
    private bool canBePickedUp = false;

    private void Start()
    {
        if (hasLuch)
        {
            rb = GetComponent<Rigidbody>();
        
            Vector3 randomForce = new Vector3(Random.Range(-2f, 2f), Random.Range(4f, 6f), Random.Range(-2f, 2f));
            rb.AddForce(randomForce, ForceMode.Impulse);
            Invoke(nameof(EnablePickup), 0.5f);
        }
        else
        {
            canBePickedUp = true;
        }
    }

    private void OnCollisionEnter (Collision collision)
    {
        if (hasLuch && rb != null && !rb.isKinematic && collision.gameObject.CompareTag("Ground"))
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (canBePickedUp && other.CompareTag("Player"))
        {
            InventoryManager.Instance.AddItem(item, amount);
            Destroy(gameObject);
        }
    }

    private void EnablePickup()
    {
        canBePickedUp = true;
    }
}
