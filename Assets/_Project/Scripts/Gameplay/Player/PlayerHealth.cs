using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("UI Integration")]
    public Image healthFillImage;
    public TextMeshProUGUI HPNumbers;
    [Tooltip("Arrastra aquí el objeto de la UI que tiene el Animator de la cara.")]
    public Animator faceUIAnimator;
    [Tooltip("Arrastra aquí el Panel de Muerte que contiene el botón de volver.")]
    public GameObject deathPanel;

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

        // Asegurarnos de que el panel de muerte esté oculto al iniciar
        if (deathPanel != null) deathPanel.SetActive(false);

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
            faceUIAnimator.SetTrigger("Hurt");
        }

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            // Animación de daño en el personaje físico
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
        if (healthFillImage != null)
        {
            healthFillImage.fillAmount = currentHealth / maxHealth;
        }

        if (HPNumbers != null)
        {
            HPNumbers.text = Mathf.FloorToInt(currentHealth).ToString();
        }
    }

    private IEnumerator BecomeInvulnerable()
    {
        isInvulnerable = true;
        float timer = 0;

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
        if (isDead) return;
        isDead = true;
        currentHealth = 0;
        ActualizarUI();

        // 1. Disparar animaciones de muerte
        if (anim != null) anim.SetTrigger("Die");
        //if (faceUIAnimator != null) faceUIAnimator.SetTrigger("Die");

        // 2. DESACTIVAR CONTROLES Y SISTEMAS

        // Desactiva el script de movimiento
        if (GetComponent<PlayerMovement>() != null)
            GetComponent<PlayerMovement>().enabled = false;

        // Desactiva el blink
        if (GetComponent<PlayerBlink>() != null)
            GetComponent<PlayerBlink>().enabled = false;

        // Desactiva el Manager de habilidades (para que no procese más Inputs)
        if (GetComponent<AbilityManager>() != null)
            GetComponent<AbilityManager>().enabled = false;

        // Desactiva cada script de habilidad individualmente y oculta indicadores
        BaseAbility[] habilidades = GetComponents<BaseAbility>();
        foreach (BaseAbility habilidad in habilidades)
        {
            habilidad.HideIndicator();
            habilidad.enabled = false;
        }

        Debug.Log("Jugador muerto. Sistemas desactivados.");

        // 3. Mostrar Panel de UI tras un breve retraso
        StartCoroutine(ShowDeathPanelRoutine());
    }

    private IEnumerator ShowDeathPanelRoutine()
    {
        yield return new WaitForSecondsRealtime(2f);

        if (deathPanel != null)
        {
            deathPanel.SetActive(true);

            // PAUSAR EL JUEGO
            Time.timeScale = 0f;

            // Liberar el mouse
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    // Función para el botón "Volver a la nave"
    public void BackToShip()
    {
        Time.timeScale = 1f; // REANUDAR EL TIEMPO
        SceneManager.LoadScene("Integration-1");
    }

    // private void OnTriggerStay(Collider other)
    // {
    //     if (other.CompareTag("Enemy")) TakeDamage(10f);
    // }
}