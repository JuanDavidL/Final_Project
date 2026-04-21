using UnityEngine;

public class AbilityUpgradeTree : MonoBehaviour
{
    public enum TreeType { Fireball, FrostNova }

    [Header("Tipo de árbol")]
    public TreeType treeType;

    [Header("Habilidad objetivo (solo en escena Planeta)")]
    public BaseAbility targetAbility;

    [Header("Mejoras en orden secuencial")]
    public AbilityUpgrade[] upgrades;

    [Header("Valores base Originales")]
    public float baseDamage;
    public float baseManaCost;
    public float baseRadius;

    private int _purchasedCount = 0;

    // Valores base
    private float _baseDamage;
    private float _baseManaCost;
    private float _baseRadius;

    void Awake()
    {
        // Carga cuántas mejoras ya compró del GameManager
        if (GameManager.Instance != null)
        {
            _purchasedCount = treeType == TreeType.Fireball
                ? GameManager.Instance.fireballUpgradesPurchased
                : GameManager.Instance.frostNovaUpgradesPurchased;
        }
    }

    void Start()
    {
        // Si hay habilidad asignada (estamos en el planeta), aplica todas las mejoras compradas
        if (targetAbility != null)
        {
            GuardarValoresBase();
            AplicarTodasLasMejoras();
        }
    }

    private void GuardarValoresBase()
    {
        _baseDamage = baseDamage > 0 ? baseDamage : targetAbility.damage;
        _baseManaCost = baseManaCost > 0 ? baseManaCost : targetAbility.manaCost;

        Debug.Log($"[{treeType}] Base guardado → ManaCost: {_baseManaCost} | Damage: {_baseDamage} | Radius: {_baseRadius}");

        if (targetAbility is Fireball fb)
            _baseRadius = baseRadius > 0 ? baseRadius : fb.explosionRadius;
        else if (targetAbility is FrostNova fn)
            _baseRadius = baseRadius > 0 ? baseRadius : fn.novaRadius;
    }

    // Aplica todas las mejoras compradas al cargar la escena planeta
    private void AplicarTodasLasMejoras()
    {
        for (int i = 0; i < _purchasedCount && i < upgrades.Length; i++)
            ApplyUpgrade(upgrades[i]);

        Debug.Log($"{treeType}: {_purchasedCount} mejoras aplicadas al cargar escena.");
    }

    // ─── Comprar ──────────────────────────────────────────────────

    public bool TryPurchaseNext()
    {
        if (!CanPurchaseNext()) return false;

        AbilityUpgrade upgrade = upgrades[_purchasedCount];
        GameManager.Instance.totalCredits -= upgrade.cost;
        _purchasedCount++;

        // Guarda en GameManager
        if (treeType == TreeType.Fireball)
            GameManager.Instance.fireballUpgradesPurchased = _purchasedCount;
        else
            GameManager.Instance.frostNovaUpgradesPurchased = _purchasedCount;

        GameManager.Instance.SaveGlobalProgress();

        Debug.Log($"{treeType}: mejora '{upgrade.upgradeName}' comprada. Total: {_purchasedCount}/{upgrades.Length}");
        return true;
    }

    public bool CanPurchaseNext()
    {
        if (_purchasedCount >= upgrades.Length) return false;
        if (GameManager.Instance == null) return false;
        return GameManager.Instance.totalCredits >= upgrades[_purchasedCount].cost;
    }

    // ─── Aplica una mejora ────────────────────────────────────────

    private void ApplyUpgrade(AbilityUpgrade upgrade)
    {
        // Si no hay habilidad asignada (escena nave) no aplica nada
        if (targetAbility == null) return;

        switch (upgrade.upgradeType)
        {
            case AbilityUpgrade.UpgradeType.UnlockAbility:
                targetAbility.gameObject.SetActive(true);
                break;

            case AbilityUpgrade.UpgradeType.ReduceManaCost:
                targetAbility.manaCost = _baseManaCost * (1f - upgrade.value / 100f);
                break;

            case AbilityUpgrade.UpgradeType.IncreaseDamage:
                targetAbility.damage = _baseDamage * (1f + upgrade.value / 100f);
                break;

            case AbilityUpgrade.UpgradeType.IncreaseRadius:
                if (targetAbility is Fireball fb)
                    fb.explosionRadius = _baseRadius * (1f + upgrade.value / 100f);
                else if (targetAbility is FrostNova fn)
                    fn.novaRadius = _baseRadius * (1f + upgrade.value / 100f);
                break;

            case AbilityUpgrade.UpgradeType.IncreaseQuantity:
                if (targetAbility is Fireball fireball)
                    fireball.quantity += (int)upgrade.value;
                else if (targetAbility is FrostNova frostNova)
                    frostNova.maxCharge += (int)upgrade.value;
                break;
        }
    }

    // ─── Getters para la UI ───────────────────────────────────────

    public AbilityUpgrade GetNextUpgrade()
    {
        if (_purchasedCount >= upgrades.Length) return null;
        return upgrades[_purchasedCount];
    }

    public AbilityUpgrade GetUpgradeAt(int index)
    {
        if (index < 0 || index >= upgrades.Length) return null;
        return upgrades[index];
    }

    public int GetPurchasedCount() => _purchasedCount;
    public int GetTotalUpgrades() => upgrades.Length;
    public bool IsFullyUpgraded() => _purchasedCount >= upgrades.Length;
}