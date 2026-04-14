using UnityEngine;
using UnityEngine.InputSystem;

public class AbilityManager : MonoBehaviour
{
    public BaseAbility abilitySlot1;
    public BaseAbility abilitySlot2;
    public MagicBook magicBook;

    private BaseAbility activeAbility;
    private PlayerInput playerInput;
    private InputAction ability1Action;
    private InputAction ability2Action;
    private InputAction fireAction;
    

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        ability1Action = playerInput.actions["Ability1"];
        ability2Action = playerInput.actions["Ability2"];
        fireAction = playerInput.actions["Attack"];
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
        SelectAbility(abilitySlot1, MagicBook.BookState.Ability1);
    }

    private void OnAbility2 (InputAction.CallbackContext context)
    {
        SelectAbility(abilitySlot2, MagicBook.BookState.Ability2);
    }

    private void SelectAbility(BaseAbility ability, MagicBook.BookState bookState)
    {
        if (ability == null) return;

        if (ability.IsOnCooldown()) return;

        if (activeAbility != null  && activeAbility != ability)
        {
            activeAbility.HideIndicator();
            magicBook.SetState(MagicBook.BookState.Orbiting);
        }

        if (activeAbility == ability)
        {
            activeAbility.HideIndicator();
            activeAbility = null;
            magicBook.SetState(MagicBook.BookState.Orbiting);
            return;
        }

        activeAbility = ability;
        activeAbility.ShowIndicator();
        if (magicBook != null)
        {
            magicBook.SetState(bookState);
        }
    }

    private void OnFire(InputAction.CallbackContext context)
    {
       if (activeAbility == null) return;
       if (activeAbility.IsOnCooldown()) return;

       activeAbility.Use();
       activeAbility = null;
       if (magicBook != null)
       {
           magicBook.SetState(MagicBook.BookState.Orbiting);
       }
    }
}
