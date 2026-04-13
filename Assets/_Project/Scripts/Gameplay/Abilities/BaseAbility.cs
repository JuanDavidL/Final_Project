using UnityEngine;

public abstract class BaseAbility : MonoBehaviour
{
    [Header("Habilidad")]
    public string abilityName;
    public Sprite abilityIcon;
    public float cooldown = 2f;
    //public float manaCost = 10f;
    public float damage = 10f;

    protected float lastUsedTime = -10f;
    protected bool isIndicatorActive = false;

    public bool IsOnCooldown()
    {
        return Time.time - lastUsedTime < cooldown;
    }

    public float GetCooldownRemaining()
    {
        return Mathf.Max(0, cooldown - (Time.time - lastUsedTime));
    }

    public virtual void ShowIndicator()
    {
        // Implementar lógica para mostrar el indicador de área de efecto
        isIndicatorActive = true;
    }

    public virtual void HideIndicator()
    {
        // Implementar lógica para ocultar el indicador de área de efecto
        isIndicatorActive = false;
    }

    public abstract void Use();

}
