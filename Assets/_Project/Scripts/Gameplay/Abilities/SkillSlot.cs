using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillSlot : MonoBehaviour
{
    [Header("UI Elements")]
    public Image skillIcon; // La imagen normal
    public Image cooldownOverlay; // La imagen cooldown
    public TextMeshProUGUI manaText;
    public TextMeshProUGUI nameText; // En caso de agregarle nombre a la habilidad luego

    private BaseAbility linkedAbility;
    //private bool isCooldownActive = false;

    // Configura el slot con la habilidad seleccionada
    public void SetupSlot(BaseAbility ability)
    {
        linkedAbility = ability;

        if (ability != null)
        {
            skillIcon.sprite = ability.abilityIcon;
            cooldownOverlay.sprite = ability.abilityIcon;
            manaText.text = ability.manaCost.ToString();
            skillIcon.enabled = true;
            cooldownOverlay.enabled = true;
            cooldownOverlay.fillAmount = 1;
        }
    }

    void Update()
    {
        if (linkedAbility == null) return;

        // Lógica de Cooldown Visual
        if (linkedAbility.IsOnCooldown())
        {
            // Calculamos el progreso (de 0 a 1)
            // cooldownRemaining va de 'Max' a '0'
            float remaining = linkedAbility.GetCooldownRemaining();
            float total = linkedAbility.cooldown;

            // Queremos que el Fill suba gradualmente hasta 1
            cooldownOverlay.fillAmount = 1 - (remaining / total);
        }
        else
        {
            cooldownOverlay.fillAmount = 1;
        }
    }
}