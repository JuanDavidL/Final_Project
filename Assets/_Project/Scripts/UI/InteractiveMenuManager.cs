using UnityEngine;

public class InteractiveMenuManager : MonoBehaviour
{
    public enum ActiveMode {Ninguno, TravelToOtherWorld, SkillTree}

    [Header("Active Mode")]
    public ActiveMode currentMode = ActiveMode.Ninguno;

    [Header("Scenes - Travel to World")]
    public string[] sceneNames;

    [Header("Skill Tree")]
    public BaseAbility[] abilities;
    public int[] abilityCosts;
    public bool[] abilityUnlocked;

    [Header("UI - Display")]
    public TMPro.TextMeshPro displayText;
    public TMPro.TextMeshPro creditsText;
    public TMPro.TextMeshPro descriptionText;

    private int _currentIndex = 0;

    void Start()
    {
        if (abilityUnlocked == null || abilityUnlocked.Length != abilities.Length)
        {
            abilityUnlocked = new bool[abilities.Length];
        }

        UpdateCreditsDisplay();
    }

    public void OnMainButtonPressed(WorldAndTreeButton3D.ButtonType tipo)
    {
        if (tipo == WorldAndTreeButton3D.ButtonType.TravelToOtherWorld)
        {
            if (currentMode == ActiveMode.TravelToOtherWorld)
            {
                TravelToScene();
                return;
            }
            currentMode = ActiveMode.TravelToOtherWorld;
            _currentIndex = 0;
        }
        else if (tipo == WorldAndTreeButton3D.ButtonType.SkillTree)
        {
            if (currentMode == ActiveMode.SkillTree)
            {
                TryUnlockAbility();
                return;
            }
            currentMode = ActiveMode.SkillTree;
            _currentIndex = 0;

        }
        UpdateDisplay();
    }
    
    // Arrow buttons

    public void OnArrowLeft()
    {
        if (currentMode == ActiveMode.Ninguno) return;
        int max = GetMaxIndex();
        _currentIndex = (_currentIndex - 1 + max) % max;
        UpdateDisplay();
    }

    public void OnArrowRight()
    {
        if (currentMode == ActiveMode.Ninguno) return;
        int max = GetMaxIndex();
        _currentIndex = (_currentIndex + 1) % max;
        UpdateDisplay();
    }

    // Travel tu other world

    private void TravelToScene()
    {
        if (sceneNames == null || sceneNames.Length == 0) return;
        GameManager.Instance?.SaveGlobalProgress(); // Guardar antes de cambiar de escena
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneNames[_currentIndex]);
    }

    private void TryUnlockAbility()
    {
        if (abilities.Length == 0) return;

        // Ya está desbloqueada
        if (abilityUnlocked[_currentIndex])
        {
            Debug.Log($"{abilities[_currentIndex].abilityName} ya está desbloqueada.");
            return;
        }

        int cost = abilityCosts[_currentIndex];

        if (GameManager.Instance.totalCredits >= cost)
        {
            GameManager.Instance.totalCredits -= cost;
            abilityUnlocked[_currentIndex] = true;

            // ✅ Activa el GameObject de la habilidad para que el jugador pueda usarla
            abilities[_currentIndex].gameObject.SetActive(true);

            GameManager.Instance.SaveGlobalProgress();
            UpdateCreditsDisplay();
            Debug.Log($"¡{abilities[_currentIndex].abilityName} desbloqueada!");
        }
        else
        {
            Debug.LogWarning($"Créditos insuficientes. Necesitas {cost}.");
        }

        UpdateDisplay();
    }

    // ─── Display ──────────────────────────────────────────────────

    private void UpdateDisplay()
    {
        if (currentMode == ActiveMode.TravelToOtherWorld)
        {
            if (displayText != null && sceneNames.Length > 0)
                displayText.text = sceneNames[_currentIndex];

            if (descriptionText != null)
                descriptionText.text = "";
        }
        else if (currentMode == ActiveMode.SkillTree)
        {
            if (abilities.Length == 0) return;

            BaseAbility ability = abilities[_currentIndex];
            int cost = abilityCosts[_currentIndex];
            bool unlocked = abilityUnlocked[_currentIndex];

            if (displayText != null)
                displayText.text = ability.abilityName;

            if (descriptionText != null)
                descriptionText.text = unlocked
                    ? "✓ Desbloqueada"
                    : $"Costo: {cost} créditos\nDaño: {ability.damage}\nCooldown: {ability.cooldown}s";
        }
    }

    private void UpdateCreditsDisplay()
    {
        if (creditsText != null && GameManager.Instance != null)
            creditsText.text = $"Créditos: {GameManager.Instance.totalCredits}";
    }

    private int GetMaxIndex()
    {
        return currentMode == ActiveMode.TravelToOtherWorld ? sceneNames.Length : abilities.Length;
    }

    
}
