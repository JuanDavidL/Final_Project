using UnityEngine;

[CreateAssetMenu(fileName = "NewUpgrade", menuName = "Abilities/AbilityUpgrade")]
public class AbilityUpgrade : ScriptableObject
{
    [Header("Info")]
    public string upgradeName;
    public string description;
    public Sprite upgradeIcon;
    public int cost;

    [Header("Tipo de mejora")]
    public UpgradeType upgradeType;
    public float value; // el valor que se aplica

    public enum UpgradeType
    {
        UnlockAbility,      // desbloquea la habilidad
        ReduceManaCost,     // reduce manaCost en %
        IncreaseDamage,     // aumenta damage en %
        IncreaseRadius,     // aumenta explosionRadius / novaRadius en %
        IncreaseQuantity    // aumenta quantity (lanza más bolas / más cargas)
    }
}