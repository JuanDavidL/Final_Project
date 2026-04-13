using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Collections;

public class BookAnimatorController : MonoBehaviour
{
    [Header("Sprites")]
    public Sprite[] openSprites;
    public float spriteFrameRate = 0.05f;

    [Header("Movimiento")]
    public float startPosY = -1700f;
    public float endPosY = 0f;
    public float moveDuration = 0.4f;

    [Header("Fade")]
    public CanvasGroup contentGroup;
    public float fadeDuration = 0.3f;

    [Header("Pestañas")]
    public CanvasGroup tabsGroup;
    public float tabAnimDuration = 0.15f;
    

    private Vector2[] tabsOriginalPositions;


    private Image bookImage;
    private RectTransform rectTransform;
    private bool isOpen = false;
    private bool isAnimating = false;
    public System.Action onBookOpened;

    void Awake()
    {
        bookImage = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(0f, startPosY);
        contentGroup.alpha = 0f;

        tabsGroup.alpha = 0f;
        tabsGroup.gameObject.SetActive(false);
        
    }

    public bool IsOpen() => isOpen;

    public void OpenBook()
    {
        if (isAnimating) return;
        isOpen = true;
        gameObject.SetActive(true);
        StartCoroutine(OpenSequence());
    }

    public void CloseBook()
    {
        if (isAnimating) return;
        isOpen = false;
        StartCoroutine(CloseSequence());
    }

    private IEnumerator OpenSequence()
    {
        isAnimating = true;

        // 1. Sube el libro
        yield return StartCoroutine(MoveBook(startPosY, endPosY));

        // 2. Reproduce sprites de apertura
        yield return StartCoroutine(PlaySprites(false));
        yield return StartCoroutine(ShowTabs());

        // 3. Fade in del contenido
        yield return StartCoroutine(FadeContent(0f, 1f));

        isAnimating = false;
        onBookOpened?.Invoke();
    }

    private IEnumerator CloseSequence()
    {
        isAnimating = true;

        // 1. Fade out del contenido
        yield return StartCoroutine(FadeContent(1f, 0f));
        yield return StartCoroutine(HideTabs());

        // 2. Reproduce sprites al reves
        yield return StartCoroutine(PlaySprites(true));

        // 3. Baja el libro
        yield return StartCoroutine(MoveBook(endPosY, startPosY));

        gameObject.SetActive(false);
        isAnimating = false;
    }

    private IEnumerator MoveBook(float from, float to)
    {
        float elapsed = 0f;

        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / moveDuration);
            rectTransform.anchoredPosition = new Vector2(0f, Mathf.Lerp(from, to, t));
            yield return null;
        }

        rectTransform.anchoredPosition = new Vector2(0f, to);
    }

    private IEnumerator PlaySprites(bool reverse)
    {
        Sprite[] frames = openSprites;

        if (reverse)
        {
            frames = new Sprite[openSprites.Length];
            for (int i = 0; i < openSprites.Length; i++)
                frames[i] = openSprites[openSprites.Length - 1 - i];
        }

        foreach (Sprite frame in frames)
        {
            bookImage.sprite = frame;
            yield return new WaitForSeconds(spriteFrameRate);
        }
    }

    private IEnumerator FadeContent(float from, float to)
    {
        float elapsed = 0f;
        contentGroup.alpha = from;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            contentGroup.alpha = Mathf.Lerp(from, to, elapsed / fadeDuration);
            yield return null;
        }

        contentGroup.alpha = to;
    }

    private IEnumerator ShowTabs()
    {
        tabsGroup.gameObject.SetActive(true);
        float elapsed = 0f;
        tabsGroup.alpha = 0f;

        while (elapsed < tabAnimDuration)
        {
            elapsed += Time.deltaTime;
            tabsGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / tabAnimDuration);
            yield return null;
        }

        tabsGroup.alpha = 1f;
    }

    private IEnumerator HideTabs()
    {
        float elapsed = 0f;
        tabsGroup.alpha = 1f;

        while (elapsed < tabAnimDuration)
        {
            elapsed += Time.deltaTime;
            tabsGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / tabAnimDuration);
            yield return null;
        }

        tabsGroup.alpha = 0f;
        tabsGroup.gameObject.SetActive(false);
    }

    private IEnumerator AnimateTab (RectTransform tab, float from, float to)
    {
        float elapsed = 0f;
        Vector2 startPos = tab.anchoredPosition;

        while (elapsed < tabAnimDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / tabAnimDuration);
            tab.anchoredPosition = new Vector2(startPos.x, Mathf.Lerp(from, to, t));
            yield return null;
        }

        tab.anchoredPosition = new Vector2(startPos.x, to);
    }
}