using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Settings")]
    public float maxHealth = 50f;
    private float currentHealth;

    private Animator anim;
    private bool isDead = false;

    private LootTable lootTable;


    void Start()
    {
        currentHealth = maxHealth;
        anim = GetComponentInChildren<Animator>();
        lootTable = GetComponent<LootTable>();
    }

    public void TakeDamage(float damage)
    {
        // Si ya está muerto o el daño es inválido, salimos
        if (isDead || damage <= 0) return;

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            currentHealth = 0; // Evitamos valores negativos por estética
            Die();
        }
        // else
        // {
        // Tarea para luego, realizar animación de recibir daño o hurt en el enemigo
        //     if (anim != null) anim.SetTrigger("Hurt");
        // }
    }

    private void Die()
    {
        isDead = true;
        Debug.Log($"{gameObject.name} ha muerto.");

        // 1. Disparar animación de muerte
        if (anim != null) anim.SetTrigger("Die");

        // 2. Desactivar inteligencia y movimiento
        var aiScript = GetComponentInChildren<EnemyAI>();
        if (aiScript != null) aiScript.enabled = false;

        var agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agent != null) agent.isStopped = true;

        // 3. Desactivar colliders para que no estorbe muerto
        var collider = GetComponent<Collider>();
        if (collider != null) collider.enabled = false;

        // 4. Destruir el objeto después de que termine la animación (ej. 2 segundos)
        Destroy(gameObject, 2f);

        if (lootTable != null) lootTable.SpawnDrops();
    }
}