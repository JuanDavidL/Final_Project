using UnityEngine;

public class FireballProjectile : MonoBehaviour
{
    private Vector3 direction;
    private float speed;
    private float damage;
    private float explosionRadius;

    public GameObject explosionVFX;

    public void Init(Vector3 dir, float spd, float radius, float dmg)
    {
        this.direction = dir;
        this.direction.y = 0f;
        this.direction.Normalize();

        this.speed = spd;
        this.explosionRadius = radius;
        this.damage = dmg;

        Destroy(gameObject, 3f);
    }

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) return;

        Explode();
    }

    private void Explode()
    {
        Collider[] targets = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider t in targets)
        {
            EnemyHealth e = t.GetComponent<EnemyHealth>();
            if (e != null) e.TakeDamage(damage);
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
