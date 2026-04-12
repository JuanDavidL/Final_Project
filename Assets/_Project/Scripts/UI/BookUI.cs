using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

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

   private bool isOpen = false;
   private PlayerInput playerInput;
   private InputAction inventoryAction;

   void Awake()
   {
       playerInput = FindFirstObjectByType<PlayerInput>();
       inventoryAction = playerInput.actions["Inventory"];
       bookPanel.SetActive(false);
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
        isOpen = !isOpen;
        bookPanel.SetActive(isOpen);
        if (isOpen)
        {
            ShowPage(inventoryPanel);
        }
    }

    private void ShowPage(GameObject page)
    {
        inventoryPanel.SetActive(false);
        recipesPage.SetActive(false);
        infoPage.SetActive(false);
        abilitiesPanel.SetActive(false);

        page.SetActive(true);

        if (page == inventoryPanel)
        {
            InventoryUI inventoryUI = inventoryPanel.GetComponent<InventoryUI>();
            if (inventoryUI != null)
            {
                inventoryUI.Show();
            }
        }
    }
}
