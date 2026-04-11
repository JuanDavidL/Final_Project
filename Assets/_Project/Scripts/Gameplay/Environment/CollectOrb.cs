using UnityEngine;

public class CollectOrb : MonoBehaviour
{
   [SerializeField] private Transform target;
   private bool isAtracting = false;

   private float angle = 0f;
   private float radius = 3f;
   private float orbitSpeed = 180f;
   private float approachSpeed = 2f;

   public void StartAttract(Transform collectPoint)
    {
    target = collectPoint;
    isAtracting = true;

    if (TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            if (!rb.isKinematic)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            rb.isKinematic = true;
        }
    }

    public void StopAttract()
    {
        isAtracting = false;
        target = null;

        if (TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rb.isKinematic = false;
        }
    }



    // Update is called once per frame
    void Update()
    {
       if (!isAtracting || target == null) return;

       angle += orbitSpeed * Time.deltaTime;
       radius = Mathf.Max(0f, radius - approachSpeed * Time.deltaTime);

       float x = Mathf.Cos(angle * Mathf.Deg2Rad) * radius;
       float z = Mathf.Sin(angle * Mathf.Deg2Rad) * radius;

         Vector3 orbitPosition = new Vector3(target.position.x + x, target.position.y + 1f, target.position.z + z);
         transform.position = Vector3.Lerp(transform.position, orbitPosition, Time.deltaTime * 5f);

         if (radius <= 0.5f)
        {
            CollectibleItem collectible = GetComponent<CollectibleItem>();
            if (collectible != null)
            {
                InventoryManager.Instance.AddItem(collectible.item, collectible.amount);
                Destroy(gameObject);
            }
        }
    }

}
