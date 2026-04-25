using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class GateTeleport : MonoBehaviour
{
    [Header("Portal Destino")]
    [SerializeField]
    private Transform spawnPoint;

    // Offset para que el jugador no reaparezca dentro del trigger del portal destino
    [SerializeField]
    private Vector3 spawnOffset = new Vector3(2f, 0f, 0f);

    [Header("Waypoints del Viaje")]
    // Arrastras aquí los Empty Objects en el orden que quieres que viaje
    [SerializeField]
    private Transform[] waypoints;

    [SerializeField]
    private float travelSpeed = 5f;

    [Header("VFX (hijo del Player)")]
    // El GameObject hijo del Player que tiene el ParticleSystem en World Space
    [SerializeField]
    private GameObject vfxMago;

    [Header("Tiempos")]
    [SerializeField]
    private float delayAntesDeViajar = 1f; // Pausa dramática antes de iniciar

    [SerializeField]
    private float cooldownFinal = 1f; // Seguridad al llegar

    [Header("Curva del Viaje")]
    [SerializeField]
    private float amplitud = 1.5f;

    // ─────────────────────────────────────────────
    //  ESTADO INTERNO
    // ─────────────────────────────────────────────

    private bool _isTeleporting = false;
    private static bool _globalCooldown = false;

    // ─────────────────────────────────────────────
    //  TRIGGER
    // ─────────────────────────────────────────────

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !_isTeleporting && !_globalCooldown)
        {
            StartCoroutine(DoTeleport(other.gameObject));
        }
    }

    // ─────────────────────────────────────────────
    //  CORRUTINA PRINCIPAL
    // ─────────────────────────────────────────────

    private IEnumerator DoTeleport(GameObject playerGO)
    {
        _isTeleporting = true;
        _globalCooldown = true;

        // ── Obtenemos referencias del Player ──────────
        // Buscamos el PlayerMovement para acceder a la cámara y desactivarlo
        PlayerMovement playerMovement = playerGO.GetComponent<PlayerMovement>();

        // Buscamos el PlayerInput para desactivar los controles
        PlayerInput playerInput = playerGO.GetComponent<PlayerInput>();

        // Buscamos el Rigidbody para detener la inercia
        Rigidbody rb = playerGO.GetComponent<Rigidbody>();

        // Buscamos el SpriteRenderer en los hijos automáticamente
        // Así no necesitas asignarlo en el Inspector, lo encuentra solo
        SpriteRenderer spriteRenderer = playerGO.GetComponentInChildren<SpriteRenderer>();

        // Buscamos el Animator en los hijos para detener animaciones
        Animator anim = playerGO.GetComponentInChildren<Animator>();

        // ── PASO 1: Detener movimiento ─────────────────
        // Desactivamos el PlayerInput — corta TODO input del jugador de golpe
        if (playerInput != null)
            playerInput.enabled = false;

        // Ponemos velocidad del Rigidbody a cero para que no siga con inercia
        if (rb != null)
            rb.linearVelocity = Vector3.zero;

        if (rb != null)
            rb.useGravity = false;
        if (rb != null)
            rb.isKinematic = true;

        Collider playerCollider = playerGO.GetComponent<Collider>();
        if (playerCollider != null)
            playerCollider.enabled = false;

        // Desactivamos PlayerMovement para que su LateUpdate no pelee con el viaje
        // La cámara la movemos nosotros manualmente durante el viaje
        if (playerMovement != null)
            playerMovement.enabled = false;

        // Detenemos la animación de movimiento
        if (anim != null)
            anim.SetFloat("Speed", 0f);

        Debug.Log("[GateTeleport] Jugador detenido.");

        // ── PASO 2: Pausa dramática ────────────────────
        // El jugador está quieto y el sprite visible — momento de tensión
        yield return new WaitForSeconds(delayAntesDeViajar);

        // ── PASO 3: Apagar sprite ──────────────────────
        if (spriteRenderer != null)
            spriteRenderer.enabled = false;

        // ── PASO 4: Activar VFX ───────────────────────
        // El VFX es hijo del Player, ya está en su posición exacta
        if (vfxMago != null)
        {
            vfxMago.SetActive(true);
            ParticleSystem ps = vfxMago.GetComponent<ParticleSystem>();
            if (ps != null)
                ps.Play();
        }

        Debug.Log("[GateTeleport] VFX activado, iniciando viaje.");

        // ── PASO 5: Obtener referencia de cámara ───────
        // La tomamos del PlayerMovement para moverla igual que él lo hacía
        Transform camTransform = playerMovement != null ? playerMovement.cameraTransform : null;
        Vector3 camOffset = playerMovement != null ? playerMovement.cameraOffset : Vector3.zero;
        float camSmooth = playerMovement != null ? playerMovement.cameraSmooth : 5f;

        // ── PASO 6: Viajar por los Waypoints ──────────
        // Movemos el Player completo — cámara se actualiza dentro de MoverHacia
        foreach (Transform waypoint in waypoints)
        {
            yield return StartCoroutine(
                MoverHacia(
                    playerGO.transform,
                    waypoint.position,
                    camTransform,
                    camOffset,
                    camSmooth
                )
            );
        }

        // Último tramo: viajamos al spawnPoint del portal destino
        yield return StartCoroutine(
            MoverHacia(playerGO.transform, spawnPoint.position, camTransform, camOffset, camSmooth)
        );

        Debug.Log("[GateTeleport] Llegó al destino.");

        // ── PASO 7: Aplicar offset de salida ──────────
        // Desplazamos al jugador fuera del trigger del portal destino
        playerGO.transform.position = spawnPoint.position + spawnOffset;

        if (rb != null)
            rb.isKinematic = false;
        if (rb != null)
            rb.useGravity = true;

        if (playerCollider != null)
            playerCollider.enabled = true;

        // Actualizamos la cámara al offset final inmediatamente
        if (camTransform != null)
        {
            camTransform.position = playerGO.transform.position + camOffset;
        }

        // ── PASO 8: Apagar VFX y mostrar sprite ───────
        if (vfxMago != null)
        {
            ParticleSystem ps = vfxMago.GetComponent<ParticleSystem>();
            if (ps != null) ps.Stop();
            yield return new WaitForSeconds(1.5f); // Esperamos a que el VFX se disipe un poco
            vfxMago.SetActive(false);
        }

        if (spriteRenderer != null)
            spriteRenderer.enabled = true;

        // ── PASO 9: Reactivar movimiento ──────────────
        if (playerMovement != null)
            playerMovement.enabled = true;
        if (playerInput != null)
            playerInput.enabled = true;

        Debug.Log("[GateTeleport] Jugador reactivado.");

        // ── PASO 10: Cooldown de seguridad ────────────
        yield return new WaitForSeconds(cooldownFinal);

        _isTeleporting = false;
        _globalCooldown = false;

        Debug.Log("[GateTeleport] Portal listo.");
    }

    // ─────────────────────────────────────────────
    //  CORRUTINA DE MOVIMIENTO CON PIRUETA Y CÁMARA
    // ─────────────────────────────────────────────

    private IEnumerator MoverHacia(
        Transform objeto,
        Vector3 destino,
        Transform camTransform,
        Vector3 camOffset,
        float camSmooth
    )
    {
        Vector3 origen = objeto.position;

        // ── Punto de control de la parábola ──
        // Es el punto medio entre origen y destino
        Vector3 puntoMedio = (origen + destino) * 0.5f;

        // ✅ Solo sube en Y para crear la parábola, sin desplazamiento lateral
        Vector3 puntoControl = puntoMedio + new Vector3(0f, amplitud, 0f);

        float progreso = 0f;

        float distanciaAprox = Vector3.Distance(origen, destino) * 1.5f;

        while (progreso < 1f)
            {
            progreso += (travelSpeed * Time.deltaTime) / distanciaAprox;
            progreso = Mathf.Clamp01(progreso);

            float t = progreso;
            float u = 1f - t;

            // ✅ Misma fórmula Bézier cuadrática pero el puntoControl está arriba
            // Esto produce una parábola perfecta entre origen y destino
            Vector3 posicionCurva = (u * u * origen)
                              + (2f * u * t * puntoControl)
                              + (t * t * destino);

            objeto.position = posicionCurva;

            if (camTransform != null)
            {
                Vector3 targetCamPos = objeto.position + camOffset;
                camTransform.position = Vector3.Lerp(camTransform.position, targetCamPos, camSmooth * Time.deltaTime);
            }

            yield return null;
            }
        
        objeto.position = destino;
    }

    // ─────────────────────────────────────────────
    //  VALIDACIÓN FUTURA
    // ─────────────────────────────────────────────

    public bool CanOpenGate()
    {
        // Aquí irá la lógica de enemigos
        return true;
    }
}
