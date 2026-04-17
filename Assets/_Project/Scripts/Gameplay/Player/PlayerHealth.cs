using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("UI Integration")]
    public Image healthFillImage;
    public TextMeshProUGUI HPNumbers;
    [Tooltip("Arrastra aquí el objeto de la UI que tiene el Animator de la cara.")]
    public Animator faceUIAnimator; // Referencia para la animación del retrato

    [Header("Damage Settings")]
    public float invulnerabilityDuration = 1f;
    private bool isInvulnerable = false;

    [Header("Regeneration Settings")]
    public float regenWaitTime = 5f;
    public float regenRate = 5f;
    private float lastDamageTime;

    private Animator anim; // Animator del personaje en el mundo
    private SpriteRenderer spriteRenderer;
    private bool isDead = false;

    void Awake()
    {
        currentHealth = maxHealth;
        anim = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        ActualizarUI();
    }

    void Update()
    {
        if (isDead) return;

        // Lógica de regeneración automática
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

        // --- FEEDBACK VISUAL EN LA UI ---
        if (faceUIAnimator != null)
        {
            faceUIAnimator.SetTrigger("Hurt"); // Activa la transición a MageHurtFace
        }

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            // Animación de daño en el personaje físico (3D/2D World)
            if (anim != null) anim.SetTrigger("Hurt");

            // Iniciar parpadeo de invulnerabilidad
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
        // Actualizar barra de vida
        if (healthFillImage != null)
        {
            healthFillImage.fillAmount = currentHealth / maxHealth;
        }

        // Actualizar texto de vida (sin decimales)
        if (HPNumbers != null)
        {
            HPNumbers.text = Mathf.FloorToInt(currentHealth).ToString();
        }
    }

    private IEnumerator BecomeInvulnerable()
    {
        isInvulnerable = true;
        float timer = 0;

        // Efecto de parpadeo visual
        while (timer < invulnerabilityDuration)
        {
            if (spriteRenderer != null) spriteRenderer.enabled = !spriteRenderer.enabled;
            yield return new WaitForSeconds(0.1f);
            timer += 0.1f;
        }

        if (spriteRenderer != null) spriteRenderer.enabled = true;
        isInvulnerable = false;
    }

    private void Die()
    {
        if (isDead) return; // Evita que se ejecute varias veces
        isDead = true;
        currentHealth = 0;
        ActualizarUI();

        // 1. Disparar animaciones de muerte
        if (anim != null) anim.SetTrigger("Die");
        if (faceUIAnimator != null) faceUIAnimator.SetTrigger("Die");

        // 2. DESACTIVAR CONTROLES

        // Desactiva el script de movimiento
        if (GetComponent<PlayerMovement>() != null)
            GetComponent<PlayerMovement>().enabled = false;
        if (GetComponent<PlayerBlink>() != null)
            GetComponent<PlayerBlink>().enabled = false;
        // Desactiva cualquier habilidad adicional que tengas (si las hay)
        BaseAbility[] habilidades = GetComponents<BaseAbility>();
        foreach (BaseAbility habilidad in habilidades)
        {
            habilidad.enabled = false;
        }

        Debug.Log("El jugador ha muerto. Controles desactivados.");
    }

    private void OnTriggerStay(Collider other)
    {
        // Daño por contacto continuo con enemigos
        if (other.CompareTag("Enemy")) TakeDamage(10f);
    }
}