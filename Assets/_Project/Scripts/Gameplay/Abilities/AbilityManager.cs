using UnityEngine;
using UnityEngine.InputSystem;

public class AbilityManager : MonoBehaviour
{
    [Header("UI Slots")]
    public SkillSlot uiSlot1;
    public SkillSlot uiSlot2;
    public SkillSlot blinkSlot;
    public BaseAbility abilitySlot1;
    public BaseAbility abilitySlot2;
    private PlayerBlink playerBlink;
    public MagicBook magicBook;
    private BaseAbility activeAbility;
    private PlayerInput playerInput;
    private InputAction ability1Action;
    private InputAction ability2Action;
    private InputAction fireAction;

    // Referencias nuevas
    private Animator anim;
    private PlayerMana playerMana;

    private int currentAttackID = 0;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        ability1Action = playerInput.actions["Ability1"];
        ability2Action = playerInput.actions["Ability2"];
        fireAction = playerInput.actions["Attack"];

        anim = GetComponentInChildren<Animator>();
        playerMana = GetComponent<PlayerMana>();
        playerBlink = GetComponent<PlayerBlink>();
    }
    void Start()
    {
        // Al iniciar (o al bajar al planeta), asignamos las visuales
        SetupAbilitySlot();
        if (blinkSlot != null && playerBlink != null)
        {
            blinkSlot.SetupSlot(playerBlink);
        }
    }

    private void SetupAbilitySlot()
    {
        bool fireballUnlocked = GameManager.Instance != null 
        && GameManager.Instance.fireballUpgradesPurchased > 0;

        bool frostNovaUnlocked = GameManager.Instance != null 
        && GameManager.Instance.frostNovaUpgradesPurchased > 0;

        //Slot 1 = Fireball solo si está desbloqueada
        if (uiSlot1 != null)
        {
        if (fireballUnlocked)
            {
                uiSlot1.SetupSlot(abilitySlot1);
                abilitySlot1?.gameObject.SetActive(true);
            }
            else
            {
                uiSlot1.SetupSlot(null); // slot vacío
                abilitySlot1?.gameObject.SetActive(false);
            }
        }

        //Slot 2 = FrostNova solo si está desbloqueada
        if (uiSlot2 != null)
        {
            if (frostNovaUnlocked)
            {
                uiSlot2.SetupSlot(abilitySlot2);
                abilitySlot2?.gameObject.SetActive(true);
            }
            else
            {
                uiSlot2.SetupSlot(null); // slot vacío
                abilitySlot2?.gameObject.SetActive(false);
            }
        }
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
        if (abilitySlot1 == null || !abilitySlot1.gameObject.activeSelf) return;
        currentAttackID = 1;
        SelectAbility(abilitySlot1, MagicBook.BookState.Ability1);
    }

    private void OnAbility2(InputAction.CallbackContext context)
    {
        if (abilitySlot2 == null || !abilitySlot2.gameObject.activeSelf) return;
        currentAttackID = 2;
        SelectAbility(abilitySlot2, MagicBook.BookState.Ability2);
    }

    private void SelectAbility(BaseAbility ability, MagicBook.BookState bookState)
    {
        if (ability == null) return;
        if (ability.IsOnCooldown()) return;

        // --- CHEQUEO DE MANÁ AL SELECCIONAR ---
        if (playerMana != null && !playerMana.CanAfford(ability.manaCost))
        {
            Debug.Log("No tienes suficiente maná para seleccionar esta habilidad");
            return;
        }

        if (activeAbility != null && activeAbility != ability)
        {
            activeAbility.HideIndicator();
        }

        if (activeAbility == ability)
        {
            activeAbility.HideIndicator();
            activeAbility = null;
            currentAttackID = 0;
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

        // --- CHEQUEO DE MANÁ AL DISPARAR ---
        if (playerMana != null && !playerMana.CanAfford(activeAbility.manaCost))
        {
            Debug.Log("Maná insuficiente!");
            // Opcional: Ocultar indicador si ya no puede costearlo
            activeAbility.HideIndicator();
            activeAbility = null;
            return;
        }

        // --- DISPARAR ANIMACIÓN Y CONSUMIR RECURSOS ---
        if (anim != null && currentAttackID != 0)
        {
            anim.SetInteger("AttackType", currentAttackID);
            anim.SetTrigger("Attack");
        }

        // Restamos el maná antes de usar la habilidad
        if (playerMana != null)
        {
            playerMana.UseMana(activeAbility.manaCost);
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