using UnityEngine;

public class Destructible : MonoBehaviour
{
    public float maxHealth = 20f;
    private float currentHealth;
    private LootTable lootTable;

    void Start()
    {
        currentHealth = maxHealth;
        lootTable = GetComponent<LootTable>();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TakeDamage(10f);
        }
    }
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (lootTable != null)
        {
            lootTable.SpawnDrops();
        }
        Destroy(gameObject);
    }
}
