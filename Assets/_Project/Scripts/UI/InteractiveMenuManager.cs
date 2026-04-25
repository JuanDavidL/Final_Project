using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InteractiveMenuManager : MonoBehaviour
{
    // ─── Texto de créditos siempre visible ────────────────────────
    [Header("Créditos (siempre visible)")]
    public TextMeshProUGUI creditsText;

    // ─── Pantalla inicial ─────────────────────────────────────────
    [Header("Pantalla Inicial")]
    public TextMeshProUGUI statusText;

    // ─── Viaja Mundos ─────────────────────────────────────────────
    [Header("Viaja Mundos")]
    public GameObject imagePlanet;
    public GameObject arrowRightPlanet;
    public GameObject arrowLeftPlanet;
    public GameObject buttonTravel;
    public GameObject buttonCancel;
    public TextMeshProUGUI planetNameText; // puede ser el statusText si quieres

    [Header("Configuración de Audio")]
    public AudioClip buyButtonSFX; // Sonido al comprar una mejora en el árbol de habilidades
    [System.Serializable]
    public class PlanetData
    {
        public string planetName;
        public string difficulty;
        public Sprite planetSprite;
        public string sceneName;
    }

    public PlanetData[] planets;

    // ─── Árbol Habilidades - Selección ───────────────────────────
    [Header("Árbol Habilidades - Selección")]
    public GameObject buttonSelectTreeFireball;
    public GameObject buttonSelectTreeFrostNova;
    public GameObject imageFireball; // sprite de fireball
    public GameObject imageFrostNova; // sprite de frostnova
    public GameObject buttonComeBack;
    public GameObject buttonCancelSkillTree; // vuelve al inicio

    // ─── Árbol Habilidades - Detalle ──────────────────────────────
    [Header("Árbol Habilidades - Detalle")]
    public GameObject imageAbility;
    public TextMeshProUGUI textTitle;
    public TextMeshProUGUI textDescription;
    public GameObject buttonBuy;
    public GameObject arrowRightAbility;
    public GameObject arrowLeftAbility;

    // ─── Referencias lógicas ──────────────────────────────────────
    [Header("Referencias")]
    public AbilityUpgradeTree upgradeTreeFireball;
    public AbilityUpgradeTree upgradeTreeFrostNova;

    // ─── Estado interno ───────────────────────────────────────────
    private enum MenuState
    {
        Inicio,
        ViajaMundos,
        ArbolSeleccion,
        ArbolDetalle,
    }

    private MenuState _currentState = MenuState.Inicio;
    private int _currentPlanetIndex = 0;
    private AbilityUpgradeTree _currentTree = null;
    private int _currentUpgradeIndex = 0;

    // ─── Unity ───────────────────────────────────────────────────

    void Start()
    {
        MostrarInicio();
    }

    void Update()
    {
        UpdateCreditsDisplay();
    }

    // ─── Pantalla Inicio ──────────────────────────────────────────

    private void MostrarInicio()
    {
        _currentState = MenuState.Inicio;
        statusText.text = "Select the mood: \n Left Button = Travel, \n Right Button = Skill Tree";

        // Viaja mundos — todo oculto
        imagePlanet.SetActive(false);
        buttonTravel.SetActive(false);
        buttonCancel.SetActive(false);

        // Árbol selección — todo oculto
        buttonSelectTreeFireball.SetActive(false);
        buttonSelectTreeFrostNova.SetActive(false);
        imageFireball.SetActive(false);
        imageFrostNova.SetActive(false);
        buttonComeBack.SetActive(false);
        buttonCancelSkillTree.SetActive(false);

        // Árbol detalle — todo oculto
        OcultarDetalle();
    }

    // ─── Botones 3D principales ───────────────────────────────────

    // Llama esto desde WorldButton3D cuando ButtonType = ViajaMundos
    public void OnViajaMundosPressed()
    {
        _currentState = MenuState.ViajaMundos;
        _currentPlanetIndex = 0;

        // Oculta árbol
        buttonSelectTreeFireball.SetActive(false);
        buttonSelectTreeFrostNova.SetActive(false);
        imageFireball.SetActive(false);
        imageFrostNova.SetActive(false);
        buttonComeBack.SetActive(false);
        OcultarDetalle();

        // Muestra viaja mundos
        imagePlanet.SetActive(true);
        arrowRightPlanet.SetActive(true);
        arrowLeftPlanet.SetActive(true);
        buttonTravel.SetActive(true);
        buttonCancel.SetActive(true);
        buttonCancelSkillTree.SetActive(false);

        ActualizarPlaneta();
    }

    // Llama esto desde WorldButton3D cuando ButtonType = ArbolHabilidades
    public void OnArbolHabilidadesPressed()
    {
        _currentState = MenuState.ArbolSeleccion;

        // Oculta viaja mundos
        imagePlanet.SetActive(false);
        buttonTravel.SetActive(false);
        OcultarDetalle();

        // Muestra selección de habilidad
        buttonSelectTreeFireball.SetActive(true);
        buttonSelectTreeFrostNova.SetActive(true);
        imageFireball.SetActive(true);
        imageFrostNova.SetActive(true);
        buttonComeBack.SetActive(false);
        buttonCancelSkillTree.SetActive(true);

        statusText.text = "Choose ability";
    }

    // ─── Flechas 3D ───────────────────────────────────────────────

    public void OnArrowLeft()
    {
        if (_currentState == MenuState.ViajaMundos)
        {
            _currentPlanetIndex--;
            if (_currentPlanetIndex < 0)
                _currentPlanetIndex = planets.Length - 1;
            ActualizarPlaneta();
        }
        else if (_currentState == MenuState.ArbolDetalle)
        {
            _currentUpgradeIndex--;
            if (_currentUpgradeIndex < 0)
                _currentUpgradeIndex = _currentTree.GetTotalUpgrades() - 1;
            ActualizarDetalleUpgrade();
        }
    }

    public void OnArrowRight()
    {
        if (_currentState == MenuState.ViajaMundos)
        {
            _currentPlanetIndex++;
            if (_currentPlanetIndex >= planets.Length)
                _currentPlanetIndex = 0;
            ActualizarPlaneta();
        }
        else if (_currentState == MenuState.ArbolDetalle)
        {
            _currentUpgradeIndex++;
            if (_currentUpgradeIndex >= _currentTree.GetTotalUpgrades())
                _currentUpgradeIndex = 0;
            ActualizarDetalleUpgrade();
        }
    }

    public void OnCancelTravelPressed()
    {
        MostrarInicio();
    }

    // ─── Viaja Mundos ─────────────────────────────────────────────

    private void ActualizarPlaneta()
    {
        if (planets.Length == 0)
            return;

        PlanetData planet = planets[_currentPlanetIndex];
        statusText.text = $"{planet.planetName}: {planet.difficulty}";

        // Cambia el sprite del planeta
        Image img = imagePlanet.GetComponent<Image>();
        if (img != null && planet.planetSprite != null)
            img.sprite = planet.planetSprite;
    }

    // Llama esto desde el ButtonTravel
    public void OnTravelPressed()
    {
        if (_currentState != MenuState.ViajaMundos)
            return;
        if (planets.Length == 0)
            return;

        GameManager.Instance?.SaveGlobalProgress();
        SceneManager.LoadScene(planets[_currentPlanetIndex].sceneName);
    }

    // ─── Árbol Habilidades ────────────────────────────────────────

    // Llama esto desde ButtonSelectTree-FireBall
    public void OnSelectFireball()
    {
        _currentTree = upgradeTreeFireball;
        MostrarDetalleArbol();
    }

    // Llama esto desde ButtonSelectTree-FrostNova
    public void OnSelectFrostNova()
    {
        _currentTree = upgradeTreeFrostNova;
        MostrarDetalleArbol();
    }

    private void MostrarDetalleArbol()
    {
        _currentState = MenuState.ArbolDetalle;
        _currentUpgradeIndex = 0;

        // Oculta selección
        buttonSelectTreeFireball.SetActive(false);
        buttonSelectTreeFrostNova.SetActive(false);
        imageFireball.SetActive(false);
        imageFrostNova.SetActive(false);
        buttonComeBack.SetActive(false);

        // Muestra detalle
        imageAbility.SetActive(true);
        textTitle.gameObject.SetActive(true);
        textDescription.gameObject.SetActive(true);
        buttonBuy.SetActive(true);

        ActualizarDetalleUpgrade();
    }

    private void ActualizarDetalleUpgrade()
    {
        if (_currentTree == null)
            return;

        // Obtiene la mejora en el índice actual (no solo la siguiente)
        AbilityUpgrade upgrade = _currentTree.GetUpgradeAt(_currentUpgradeIndex);
        if (upgrade == null)
            return;

        bool isAlreadyPurchased = _currentUpgradeIndex < _currentTree.GetPurchasedCount();
        bool isNext = _currentUpgradeIndex == _currentTree.GetPurchasedCount();
        bool canAfford = GameManager.Instance.totalCredits >= upgrade.cost;

        //Nombre y descripción siempre visibles
        textTitle.text = upgrade.upgradeName;
        textDescription.text = $"{upgrade.description}\n\nCosto: {upgrade.cost} créditos";

        //StatusText según estado
        if (isAlreadyPurchased)
            statusText.text = "You got it already!";
        else if (canAfford)
            statusText.text = "You don't have it yet";
        else
            statusText.text = "You don't have it yet";

        //Sprite
        Image img = imageAbility.GetComponent<Image>();
        if (img != null && upgrade.upgradeIcon != null)
            img.sprite = upgrade.upgradeIcon;

        //ButtonBuy siempre visible
        buttonBuy.SetActive(true);
    }

    // Llama esto desde ButtonBuy
    public void OnBuyPressed()
    {
        if (_currentTree == null)
            return;

        bool isAlreadyPurchased = _currentUpgradeIndex < _currentTree.GetPurchasedCount();
        bool isNext = _currentUpgradeIndex == _currentTree.GetPurchasedCount();

        // Ya está comprada
        if (isAlreadyPurchased)
        {
            statusText.text = "You already have this!";
            return;
        }

        // No es la siguiente en el árbol secuencial
        if (!isNext)
        {
            statusText.text = "Buy previous upgrades first!";
            return;
        }

        // No hay créditos
        if (!_currentTree.CanPurchaseNext())
        {
            statusText.text = "Not enough Star Credits!";
            return;
        }

        //Compra exitosa

        if (AudioManager.Instance != null && buyButtonSFX != null)
        {
            AudioManager.Instance.PlaySFXRandomPitch(buyButtonSFX, 0.95f, 1.05f);
        }
        _currentTree.TryPurchaseNext();
        statusText.text = "Upgrade purchased!";
        ActualizarDetalleUpgrade();
        UpdateCreditsDisplay();
    }

    // ─── Botón volver ─────────────────────────────────────────────

    // Llama esto desde ButtonComeBackToSelectAbility
    public void OnComeBackPressed()
    {
        if (_currentState == MenuState.ArbolDetalle)
        {
            // Vuelve a selección de habilidad
            OnArbolHabilidadesPressed();
        }
        else
        {
            // Vuelve al inicio
            MostrarInicio();
        }
    }

    // ─── Helpers ──────────────────────────────────────────────────

    private void OcultarDetalle()
    {
        imageAbility.SetActive(false);
        textTitle.gameObject.SetActive(false);
        textDescription.gameObject.SetActive(false);
        buttonBuy.SetActive(false);
    }

    private void UpdateCreditsDisplay()
    {
        if (creditsText != null && GameManager.Instance != null)
            creditsText.text = $"Star Credits: {GameManager.Instance.totalCredits}";
    }
}
