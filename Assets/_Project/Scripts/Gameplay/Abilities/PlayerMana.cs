using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerMana : MonoBehaviour
{
    [Header("Mana Settings")]
    public float maxMana = 60f;
    public float currentMana;
    public float regenRate = 1f; 

    [Header("UI Integration")]
    public Image manaFillImage; 
    public TextMeshProUGUI ManaNumbers;

    void Awake()
    {
        currentMana = maxMana;
        ActualizarUI();
    }

    void Update()
    {
        if (currentMana < maxMana)
        {
            currentMana += regenRate * Time.deltaTime;
            currentMana = Mathf.Min(currentMana, maxMana);
            ActualizarUI();
        }
    }

    public bool CanAfford(float cost)
    {
        return currentMana >= cost;
    }

    public void UseMana(float amount)
    {
        currentMana -= amount;
        currentMana = Mathf.Max(0, currentMana);
        ActualizarUI();
    }

    private void ActualizarUI()
    {
        if (manaFillImage != null)
        {
            manaFillImage.fillAmount = currentMana / maxMana;
        }

        // Mostrar solo números enteros (ej: 10, 20, 35)
        if (ManaNumbers != null)
        {
            ManaNumbers.text = Mathf.FloorToInt(currentMana).ToString();
        }
    }
}