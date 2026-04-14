using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

public class BookUI : MonoBehaviour
{
    [Header("Panels")]
    public GameObject bookPanel;
    public GameObject inventoryPanel;
    public GameObject recipesPage;
    public GameObject infoPage;
    public GameObject abilitiesPanel;

    [Header("Tabs")]
    public Button tabInventory;
    public Button tabRecipes;
    public Button tabInfo;
    public Button tabAbilities;
    public float tabSelectedOffsetY = 10f;

    private PlayerInput playerInput;
    private InputAction inventoryAction;
    private BookAnimatorController bookAnimator;
    private GameObject lastPage;
    private Dictionary<Button, float> tabOriginalPositions = new Dictionary<Button, float>();
    private bool tabsInitialized = false;

    void Awake()
    {
        playerInput = FindFirstObjectByType<PlayerInput>();
        inventoryAction = playerInput.actions["Inventory"];
        bookAnimator = bookPanel.GetComponent<BookAnimatorController>();
        lastPage = inventoryPanel;

        

        bookAnimator.onBookOpened += () => ShowPage(lastPage);
    }

    private void InitTabPositions()
    {
        if (tabsInitialized) return;

        tabOriginalPositions[tabInventory] = tabInventory.GetComponent<RectTransform>().anchoredPosition.y;
        tabOriginalPositions[tabRecipes] = tabRecipes.GetComponent<RectTransform>().anchoredPosition.y;
        tabOriginalPositions[tabInfo] = tabInfo.GetComponent<RectTransform>().anchoredPosition.y;
        tabOriginalPositions[tabAbilities] = tabAbilities.GetComponent<RectTransform>().anchoredPosition.y;

        tabsInitialized = true;
    }

    void OnEnable()
    {
        inventoryAction.performed += ToggleBook;
        tabInventory.onClick.AddListener(() => ShowPage(inventoryPanel));
        tabRecipes.onClick.AddListener(() => ShowPage(recipesPage));
        tabInfo.onClick.AddListener(() => ShowPage(infoPage));
        tabAbilities.onClick.AddListener(() => ShowPage(abilitiesPanel));
    }

    void OnDisable()
    {
        inventoryAction.performed -= ToggleBook;
        tabInventory.onClick.RemoveAllListeners();
        tabRecipes.onClick.RemoveAllListeners();
        tabInfo.onClick.RemoveAllListeners();
        tabAbilities.onClick.RemoveAllListeners();
    }

    private void ToggleBook(InputAction.CallbackContext context)
    {
        if (bookAnimator.IsOpen())
            bookAnimator.CloseBook();
        else
            bookAnimator.OpenBook();
    }

    private void ShowPage(GameObject page)
    {
        InitTabPositions();
        inventoryPanel.SetActive(false);
        recipesPage.SetActive(false);
        infoPage.SetActive(false);
        abilitiesPanel.SetActive(false);

        page.SetActive(true);
        lastPage = page;

    
        InventoryUI inventoryUI = inventoryPanel.GetComponent<InventoryUI>();
        if (inventoryUI != null)
            inventoryUI.RefreshUI();

        ResetTabPosition(tabInventory);
        ResetTabPosition(tabRecipes);
        ResetTabPosition(tabInfo);
        ResetTabPosition(tabAbilities);

        if (page == inventoryPanel) RaiseTab(tabInventory);
        else if (page == recipesPage) RaiseTab(tabRecipes);
        else if (page == infoPage) RaiseTab(tabInfo);
        else if (page == abilitiesPanel) RaiseTab(tabAbilities);
    }

    private void RaiseTab(Button tab)
    {
        RectTransform rt = tab.GetComponent<RectTransform>();
        StartCoroutine(MoveTab(rt, rt.anchoredPosition.y, tabOriginalPositions[tab] + tabSelectedOffsetY));
    }

    private void ResetTabPosition(Button tab)
    {
        InitTabPositions();
        RectTransform rt = tab.GetComponent<RectTransform>();
        StartCoroutine(MoveTab(rt, rt.anchoredPosition.y, tabOriginalPositions[tab]));
    }

    private IEnumerator MoveTab(RectTransform rt, float from, float to)
    {
        float elapsed = 0f;
        float duration = 0.15f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);
            rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, Mathf.Lerp(from, to, t));
            yield return null;
        }

        rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, to);
    }
}