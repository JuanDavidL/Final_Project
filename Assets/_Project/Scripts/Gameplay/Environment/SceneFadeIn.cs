using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SceneFadeIn : MonoBehaviour
{
    [SerializeField] private float fadeDuration = 1f;

    void Start()
    {
        StartCoroutine(EsperarYFadeIn());
    }

    private IEnumerator EsperarYFadeIn()
    {
    // ✅ Espera un frame para que DontDestroyOnLoad termine de registrar el canvas
    yield return null;
    yield return null; // dos frames de seguridad

    StartCoroutine(DoFadeIn());
    }

    private IEnumerator DoFadeIn()
    {
        // Busca el canvas de fade que quedó en DontDestroyOnLoad
        GameObject fadeCanvas = GameObject.Find("FadeCanvas");
        if (fadeCanvas == null)
        {
            Destroy(gameObject);
            yield break;
        }

        Image fadeImage = fadeCanvas.GetComponentInChildren<Image>();
        if (fadeImage == null)
        {
            Destroy(fadeCanvas);
            Destroy(gameObject);
            yield break;
        }

        fadeImage.color = new Color(0f, 0f, 0f, 1f);
        yield return null;

        // ✅ Fade inverso — de negro a transparente
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Clamp01(1f - (timer / fadeDuration));
            fadeImage.color = new Color(0f, 0f, 0f, alpha);
            yield return null;
        }

        // ✅ Limpia todo
        Destroy(fadeCanvas);
        Destroy(gameObject);
    }
}