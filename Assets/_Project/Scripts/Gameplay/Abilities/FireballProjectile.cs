using UnityEngine;

public class FireballProjectile : MonoBehaviour
{
    private Vector3 direction;
    private float speed;
    private float damage;
    private float explosionRadius;

    public GameObject explosionVFX;
    public AudioClip explosionSFX;
    private AudioSource audioSource;

    public void Init(Vector3 dir, float spd, float radius, float dmg)
    {
        this.direction = dir;
        this.direction.y = 0f;
        audioSource = FindFirstObjectByType<AudioSource>();
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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player")) return;
        Debug.Log("golpeo a " + collision.collider.tag);
        Explode();
    }

    private void Explode()
    {
        // 1. Detectamos todo en el radio
        Collider[] targets = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider t in targets)
        {
            // 2. Intentamos obtener el script de vida
            EnemyHealth e = t.GetComponentInParent<EnemyHealth>();

            // 3. ¡CRÍTICO! Solo actuamos si realmente hay un enemigo
            if (e != null)
            {
                e.TakeDamage(damage);
                Debug.Log($"Fireball explotó y golpeó a {e.name} con {damage} de daño.");
            }
            else
            {
                // Opcional: Log para saber que golpeó algo inerte
                Debug.Log($"Impacto en objeto sin vida: {t.name}");
            }
        }

        // 4. Efectos visuales
        if (explosionVFX != null)
        {
            GameObject vfx = Instantiate(explosionVFX, transform.position, Quaternion.identity);
            Destroy(vfx, 2f);
        }

        // 5. Efectos de sonido
        if (audioSource != null && explosionSFX != null)
        {
            audioSource.PlayOneShot(explosionSFX);
        }

        // 6. Destruir el proyectil
        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
