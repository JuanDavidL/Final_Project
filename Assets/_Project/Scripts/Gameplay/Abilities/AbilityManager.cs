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

    // Referencia al Animator en el hijo
    private Animator anim;
    // Variable para saber qué animación usar (1 o 2)
    private int currentAttackID = 0;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        ability1Action = playerInput.actions["Ability1"];
        ability2Action = playerInput.actions["Ability2"];
        fireAction = playerInput.actions["Attack"];

        anim = GetComponentInChildren<Animator>();
    }

    void OnEnable()
    {
        ability1Action.performed += OnAbility1;
        ability2Action.performed += OnAbility2;
        fireAction.performed += OnFire;
    }

    void OnDisable()
    {
        ability1Action.performed -= OnAbility1;
        ability2Action.performed -= OnAbility2;
        fireAction.performed -= OnFire;
    }

    private void OnAbility1(InputAction.CallbackContext context)
    {
        currentAttackID = 1; // ID para la primera habilidad
        SelectAbility(abilitySlot1, MagicBook.BookState.Ability1);
    }

    private void OnAbility2(InputAction.CallbackContext context)
    {
        currentAttackID = 2; // ID para la segunda habilidad
        SelectAbility(abilitySlot2, MagicBook.BookState.Ability2);
    }

    private void SelectAbility(BaseAbility ability, MagicBook.BookState bookState)
    {
        if (ability == null) return;
        if (ability.IsOnCooldown()) return;

        if (activeAbility != null && activeAbility != ability)
        {
            activeAbility.HideIndicator();
        }

        if (activeAbility == ability)
        {
            activeAbility.HideIndicator();
            activeAbility = null;
            currentAttackID = 0; // Reset ID
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

        // --- DISPARAR ANIMACIÓN SEGÚN LA HABILIDAD ---
        if (anim != null && currentAttackID != 0)
        {
            anim.SetInteger("AttackType", currentAttackID); // Decimos cuál es (1 o 2)
            anim.SetTrigger("Attack"); // Disparamos el ataque
        }

        activeAbility.Use();
        activeAbility = null;
        currentAttackID = 0;

        if (magicBook != null)
        {
            magicBook.SetState(MagicBook.BookState.Orbiting);
        }
    }
}