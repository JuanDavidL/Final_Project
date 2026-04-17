using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;
    
    [Header("UI Integration")]
    [Tooltip("Arrastra aquí la imagen que tiene el color rosa (Fill)")]
    public Image healthFillImage; 

    [Header("Damage Settings")]
    public float invulnerabilityDuration = 1f;
    private bool isInvulnerable = false;

    [Header("Regeneration Settings")]
    public float regenWaitTime = 5f;
    public float regenRate = 5f; 
    private float lastDamageTime;

    private Animator anim;
    private SpriteRenderer spriteRenderer; // Para el efecto de parpadeo
    private bool isDead = false;

    void Awake()
    {
        currentHealth = maxHealth;
        // Buscamos componentes en los hijos
        anim = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        
        ActualizarUI();
    }

    void Update()
    {
        if (isDead) return;

        // Lógica de Regeneración (solo si ha pasado el tiempo suficiente)
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
        currentHealth = Mathf.Min(currentHealth, maxHealth); // No exceder el máximo
        ActualizarUI();
    }

    private void ActualizarUI()
    {
        if (healthFillImage != null)
        {
            // Importante: Asegúrate de que healthFillImage tenga Image Type: Filled
            healthFillImage.fillAmount = currentHealth / maxHealth;
        }
    }

    private IEnumerator BecomeInvulnerable()
    {
        isInvulnerable = true;
        
        float timer = 0;
        while (timer < invulnerabilityDuration)
        {
            if (spriteRenderer != null)
                spriteRenderer.enabled = !spriteRenderer.enabled; // Efecto parpadeo
            
            yield return new WaitForSeconds(0.1f);
            timer += 0.1f;
        }

        if (spriteRenderer != null) spriteRenderer.enabled = true;
        isInvulnerable = false;
    }

    private void Die()
    {
        isDead = true;
        currentHealth = 0;
        ActualizarUI();
        
        if (anim != null) anim.SetTrigger("Die");
        
        // Desactivamos el script de movimiento para que no se deslice muerto
        PlayerMovement moveScript = GetComponent<PlayerMovement>();
        if (moveScript != null) moveScript.enabled = false;
        
        Debug.Log("Game Over");
    }

    private void OnTriggerStay(Collider other)
    {
        // Asegúrate de que tus enemigos tengan el Tag "Enemy"
        if (other.CompareTag("Enemy")) 
        {
            TakeDamage(10f); 
        }
    }
}