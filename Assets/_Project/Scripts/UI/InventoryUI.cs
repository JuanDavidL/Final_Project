using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class InventoryUI : MonoBehaviour
{
    public GameObject inventoryPanel;
    public Transform itemGrid;
    public GameObject slotPrefab;

    private bool isOpen = false;
    private PlayerInput playerInput;
    private InputAction inventoryAction;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        playerInput = FindFirstObjectByType<PlayerInput>();
        inventoryAction = playerInput.actions["Inventory"];
        inventoryPanel.SetActive(false);
    }

    void OnEnable()
    {
        inventoryAction.performed += ToggleInventory;
    }
    void OnDisable()
    {
        inventoryAction.performed -= ToggleInventory;
    }

    void ToggleInventory(InputAction.CallbackContext context)
    {
        isOpen = !isOpen;
        inventoryPanel.SetActive(isOpen);
        if (isOpen)
        {
            RefreshUI();
        }
    }

    void RefreshUI()
    {
        foreach (Transform child in itemGrid)
        {
            Destroy(child.gameObject);
        }

        foreach (var slot in InventoryManager.Instance.inventory)
        {
            GameObject newSlot = Instantiate(slotPrefab, itemGrid);
            InventorySlotUI slotUI = newSlot.GetComponent<InventorySlotUI>();
            slotUI.SetSlot(slot.item, slot.quantity);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
