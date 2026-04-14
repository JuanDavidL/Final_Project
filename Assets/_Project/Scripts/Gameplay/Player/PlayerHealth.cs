using UnityEngine;
using UnityEngine.UI; // NECESARIO para controlar el Slider
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;
    
    [Header("UI Integration")]
    public Slider healthSlider;

    [Header("Damage Settings")]
    public float invulnerabilityDuration = 1f;
    private bool isInvulnerable = false;

    [Header("Regeneration Settings")]
    public float regenWaitTime = 5f;
    public float regenRate = 10f;
    private float lastDamageTime;

    private Animator anim;
    private bool isDead = false;

    void Awake()
    {
        currentHealth = maxHealth;
        anim = GetComponentInChildren<Animator>();

        // Inicializar la barra de vida
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
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

        ActualizarUI(); // Actualizar barra al recibir daño

        if (currentHealth <= 0)
        {
            Die();
        }
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
        ActualizarUI(); // Actualizar barra mientras se cura
    }

    private void ActualizarUI()
    {
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }
    }

    private IEnumerator BecomeInvulnerable()
    {
        isInvulnerable = true;
        // Efecto visual de parpadeo (opcional)
        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
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

    // Detectar contacto con enemigos
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy")) TakeDamage(10);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Enemy")) TakeDamage(10);
    }
}