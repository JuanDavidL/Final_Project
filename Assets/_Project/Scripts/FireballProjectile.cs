using UnityEngine;

public class FireballProjectile : MonoBehaviour
{
    private Vector3 direction;
    private float speed;
    private float Damage;
    private float explosionRadius;

    public GameObject explosionVFX;

    public void Init(Vector3 dir, float spd, float radius, float dmg)
    {
        this.direction = dir;
        this.direction.y = 0f;
        this.direction.Normalize();

        this.speed = spd;
        this.explosionRadius = radius;
        this.Damage = dmg;

        Destroy(gameObject, 3f);
    }

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
       if (other.CompareTag("Player")) return;

       Explote();
    }

    private void Explote()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider hit in hits)
        {
           Destructible destructible = hit.GetComponent<Destructible>();
              if (destructible != null)
              {
                destructible.TakeDamage(Damage);
              }
        }

        if (explosionVFX != null)
        {
            GameObject vfx = Instantiate(explosionVFX, transform.position, Quaternion.identity);
            Destroy(vfx, 2f);
        }

        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
