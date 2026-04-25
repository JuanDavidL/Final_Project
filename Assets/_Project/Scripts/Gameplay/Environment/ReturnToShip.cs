using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.UI;

public class ReturnToShip : MonoBehaviour
{
    [Header("Escena")]
    [SerializeField] private string shipSceneName = "Nave";

    [Header("Fade")]
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private float delayAntesDeFade = 0.5f;

    [Header("Canvas Fade")]
    [SerializeField] private GameObject fadeCanvasPrefab;

    private bool _isTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !_isTriggered)
        {
            _isTriggered = true;
            StartCoroutine(DoReturn(other.gameObject));
        }
    }

    private IEnumerator DoReturn(GameObject playerGO)
    {
        // ── PASO 1: Detener al jugador ─────────────────
        PlayerInput playerInput = playerGO.GetComponent<PlayerInput>();
        PlayerMovement playerMovement = playerGO.GetComponent<PlayerMovement>();
        Rigidbody rb = playerGO.GetComponent<Rigidbody>();
        Animator anim = playerGO.GetComponentInChildren<Animator>();

        if (playerInput != null) playerInput.enabled = false;
        if (playerMovement != null) playerMovement.enabled = false;
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.isKinematic = true;
        }
        if (anim != null) anim.SetFloat("Speed", 0f);

        // ── PASO 2: Pausa dramática ────────────────────
        yield return new WaitForSeconds(delayAntesDeFade);

        // ── PASO 3: Crear canvas de fade ──────────────
        // Instancia el canvas prefab o lo crea dinámicamente
        GameObject fadeCanvas;
        Image fadeImage;

        if (fadeCanvasPrefab != null)
        {
            fadeCanvas = Instantiate(fadeCanvasPrefab);
            fadeImage = fadeCanvas.GetComponentInChildren<Image>();
        }
        else
        {
            // ✅ Crea el canvas dinámicamente si no hay prefab
            fadeCanvas = new GameObject("FadeCanvas");
            Canvas canvas = fadeCanvas.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 999;
            fadeCanvas.AddComponent<UnityEngine.UI.CanvasScaler>();
            fadeCanvas.AddComponent<UnityEngine.UI.GraphicRaycaster>();

            GameObject imageGO = new GameObject("FadeImage");
            imageGO.transform.SetParent(fadeCanvas.transform, false);
            fadeImage = imageGO.AddComponent<Image>();
            fadeImage.color = new Color(0f, 0f, 0f, 0f);

            RectTransform rect = fadeImage.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        DontDestroyOnLoad(fadeCanvas);

        // ── PASO 4: Fade a negro ───────────────────────
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Clamp01(timer / fadeDuration);
            fadeImage.color = new Color(0f, 0f, 0f, alpha);
            yield return null;
        }

        fadeImage.color = new Color(0f, 0f, 0f, 1f);

        // ── PASO 5: Guardar progreso y cargar escena ───
        GameManager.Instance?.SaveGlobalProgress();
        SceneManager.LoadScene(shipSceneName);
    }
}