using UnityEngine;
using UnityEngine.UI; // Necesario para Image
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;
    
    [Header("UI Integration")]
    public Image healthFillImage; // Arrastra aquí la imagen con el Fill rosa

    [Header("Damage Settings")]
    public float invulnerabilityDuration = 1f;
    private bool isInvulnerable = false;

    [Header("Regeneration Settings")]
    public float regenWaitTime = 5f;
    public float regenRate = 5f; 
    private float lastDamageTime;

    private Animator anim;
    private bool isDead = false;

    void Awake()
    {
        currentHealth = maxHealth;
        anim = GetComponentInChildren<Animator>();
        ActualizarUI();
    }

    void Update()
    {
        if (isDead) return;

        // Lógica de Regeneración
        if (Time.time - lastDamageTime >= regenWaitTime && currentHealth < maxHealth)
        {
            RegenerateHealth();
        }
    }

    public void TakeDamage(float damage)
    {
        if (isInvulnerable || isDead) return;

        currentHealth -= damage;
        lastDamageTime = Time.time;
        ActualizarUI();

        if (currentHealth <= 0) Die();
        else
        {
            if (anim != null) anim.SetTrigger("Hurt");
            StartCoroutine(BecomeInvulnerable());
        }
    }

    private void RegenerateHealth()
    {
        currentHealth += regenRate * Time.deltaTime;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        ActualizarUI();
    }

    private void ActualizarUI()
    {
        if (healthFillImage != null)
        {
            // El fillAmount espera un valor entre 0 y 1
            healthFillImage.fillAmount = currentHealth / maxHealth;
        }
    }

    private IEnumerator BecomeInvulnerable()
    {
        isInvulnerable = true;
        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
        // Parpadeo simple
        for (float i = 0; i < invulnerabilityDuration; i += 0.2f)
        {
            if(sr) sr.enabled = !sr.enabled;
            yield return new WaitForSeconds(0.1f);
        }
        if(sr) sr.enabled = true;
        isInvulnerable = false;
    }

    private void Die()
    {
        isDead = true;
        currentHealth = 0;
        ActualizarUI();
        if (anim != null) anim.SetTrigger("Die");
        if (GetComponent<PlayerMovement>() != null) GetComponent<PlayerMovement>().enabled = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Enemy")) TakeDamage(10f);
    }
}