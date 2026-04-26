using UnityEngine;
using UnityEngine.UI;

public class AbilitiesPageUI : MonoBehaviour
{
    [Header("Left Page - Slots")]
    public Button buttonFireball;
    public Button buttonFrostNova;

    [Header("Right Page - Info Panels")]
    public GameObject fireballAbilityInfo;
    public GameObject frostNovaAbilityInfo;

    void Start()
    {
        buttonFireball.onClick.AddListener(OnSelectFireball);
        buttonFrostNova.onClick.AddListener(OnSelectFrostNova);

        // ✅ Selecciona Fireball por defecto
        OnSelectFireball();
    }

    public void RefreshUI()
    {
        OnSelectFireball();
    }

    public void OnSelectFireball()
    {
        fireballAbilityInfo.SetActive(true);
        frostNovaAbilityInfo.SetActive(false);
        buttonFireball.Select();
    }

    public void OnSelectFrostNova()
    {
        fireballAbilityInfo.SetActive(false);
        frostNovaAbilityInfo.SetActive(true);
        buttonFrostNova.Select();
    }
}