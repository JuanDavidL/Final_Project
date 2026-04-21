using UnityEngine;

public class AbilityUpgradeTree : MonoBehaviour
{
    [Header("Habilidad objetivo")]
    public BaseAbility targetAbility;

    [Header("Mejoras en orden secuencial")]
    public AbilityUpgrade[] upgrades;

    // Cuántas mejoras están compradas
    private int _purchasedCount = 0;

    // Valores base originales para no perderlos
    private float _baseDamage;
    private float _baseManaCost;
    private float _baseExplosionRadius; // solo Fireball
    private float _baseNovaRadius;      // solo FrostNova

    void Awake()
    {
        // Guarda los valores base originales
        if (targetAbility != null)
        {
            _baseDamage = targetAbility.damage;
            _baseManaCost = targetAbility.manaCost;

            if (targetAbility is Fireball fb)
                _baseExplosionRadius = fb.explosionRadius;
            else if (targetAbility is FrostNova fn)
                _baseNovaRadius = fn.novaRadius;
        }
    }

    // ─── Comprar la siguiente mejora ──────────────────────────────

    public bool TryPurchaseNext()
    {
        if (!CanPurchaseNext())
        {
            Debug.LogWarning("No se puede comprar la siguiente mejora.");
            return false;
        }

        AbilityUpgrade upgrade = upgrades[_purchasedCount];

        // Descuenta créditos
        GameManager.Instance.totalCredits -= upgrade.cost;
        GameManager.Instance.SaveGlobalProgress();

        // Aplica la mejora
        ApplyUpgrade(upgrade);
        _purchasedCount++;

        Debug.Log($"Mejora '{upgrade.upgradeName}' aplicada! Mejoras compradas: {_purchasedCount}/{upgrades.Length}");
        return true;
    }

    public bool CanPurchaseNext()
    {
        // Ya compró todas
        if (_purchasedCount >= upgrades.Length) return false;

        // No hay créditos suficientes
        AbilityUpgrade next = upgrades[_purchasedCount];
        if (GameManager.Instance.totalCredits < next.cost) return false;

        return true;
    }

    // ─── Aplica la mejora al targetAbility ────────────────────────

    private void ApplyUpgrade(AbilityUpgrade upgrade)
    {
        switch (upgrade.upgradeType)
        {
            case AbilityUpgrade.UpgradeType.UnlockAbility:
                // Activa el GameObject de la habilidad
                targetAbility.gameObject.SetActive(true);
                Debug.Log($"{targetAbility.abilityName} desbloqueada!");
                break;

            case AbilityUpgrade.UpgradeType.ReduceManaCost:
                // Reduce en % sobre el valor base
                targetAbility.manaCost = _baseManaCost * (1f - upgrade.value / 100f);
                Debug.Log($"ManaCost reducido a: {targetAbility.manaCost}");
                break;

            case AbilityUpgrade.UpgradeType.IncreaseDamage:
                // Aumenta en % sobre el valor base
                targetAbility.damage = _baseDamage * (1f + upgrade.value / 100f);
                Debug.Log($"Damage aumentado a: {targetAbility.damage}");
                break;

            case AbilityUpgrade.UpgradeType.IncreaseRadius:
                if (targetAbility is Fireball fb)
                {
                    fb.explosionRadius = _baseExplosionRadius * (1f + upgrade.value / 100f);
                    Debug.Log($"ExplosionRadius aumentado a: {fb.explosionRadius}");
                }
                else if (targetAbility is FrostNova fn)
                {
                    fn.novaRadius = _baseNovaRadius * (1f + upgrade.value / 100f);
                    Debug.Log($"NovaRadius aumentado a: {fn.novaRadius}");
                }
                break;

            case AbilityUpgrade.UpgradeType.IncreaseQuantity:
                // Aumenta quantity para Fireball o maxCharge para FrostNova
                if (targetAbility is Fireball fireball)
                {
                    fireball.quantity += (int)upgrade.value;
                    Debug.Log($"Fireball quantity: {fireball.quantity}");
                }
                else if (targetAbility is FrostNova frostNova)
                {
                    frostNova.maxCharge += (int)upgrade.value;
                    Debug.Log($"FrostNova maxCharge: {frostNova.maxCharge   }");
                }
                break;
        }
    }

    // ─── Getters para la UI ───────────────────────────────────────

    public AbilityUpgrade GetNextUpgrade()
    {
        if (_purchasedCount >= upgrades.Length) return null;
        return upgrades[_purchasedCount];
    }

    public int GetPurchasedCount() => _purchasedCount;
    public int GetTotalUpgrades() => upgrades.Length;
    public bool IsFullyUpgraded() => _purchasedCount >= upgrades.Length;
}