using UnityEngine;
using UnityEngine.InputSystem;

public class AbilityManager : MonoBehaviour
{
    public BaseAbility abilitySlot1;
    public BaseAbility abilitySlot2;

    private BaseAbility activeAbility;
    private PlayerInput playerInput;
    private InputAction ability1Action;
    private InputAction ability2Action;
    private InputAction fireAction;
    private Camera mainCamera;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        ability1Action = playerInput.actions["Ability1"];
        ability2Action = playerInput.actions["Ability2"];
        fireAction = playerInput.actions["Attack"];
        mainCamera = Camera.main;
    }

    void OnEnable()
    {
        ability1Action.performed += OnAbility1;
        Debug.Log("Ability 1 action assigned");
        ability2Action.performed += OnAbility2;
        Debug.Log("Ability 2 action assigned");
        fireAction.performed += OnFire;
        Debug.Log("Fire action assigned");
    }

    void OnDisable()
    {
        ability1Action.performed -= OnAbility1;
        ability2Action.performed -= OnAbility2;
        fireAction.performed -= OnFire;
    }

    private void OnAbility1 (InputAction.CallbackContext context)
    {
        SelectAbility(abilitySlot1);
    }

    private void OnAbility2 (InputAction.CallbackContext context)
    {
        SelectAbility(abilitySlot2);
    }

    private void SelectAbility(BaseAbility ability)
    {
        if (ability == null) return;

        if (ability.IsOnCooldown()) return;

        if (activeAbility != null  && activeAbility != ability)
        {
            activeAbility.HideIndicator();
        }

        if (activeAbility == ability)
        {
            activeAbility.HideIndicator();
            activeAbility = null;
            return;
        }

        activeAbility = ability;
        activeAbility.ShowIndicator();
    }

    private void OnFire(InputAction.CallbackContext context)
    {
       if (activeAbility == null) return;
       if (activeAbility.IsOnCooldown()) return;

       activeAbility.Use();
       activeAbility = null;
    }
}
