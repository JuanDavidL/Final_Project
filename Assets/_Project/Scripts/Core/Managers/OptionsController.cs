using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionsController : MonoBehaviour
{
    [Header("Panel de Opciones")]
    public GameObject optionsPanel;

    [Header("Sliders")]
    public Slider musicSlider;
    public Slider sfxSlider;

    [Header("Textos de porcentaje")]
    public TextMeshProUGUI musicPercentText;
    public TextMeshProUGUI sfxPercentText;

    void Start()
    {
        // ✅ Inicializa los sliders con los valores guardados
        if (AudioManager.Instance != null)
        {
            musicSlider.minValue = 0;
            musicSlider.maxValue = 100;
            sfxSlider.minValue = 0;
            sfxSlider.maxValue = 100;

            musicSlider.value = AudioManager.Instance.GetMusicVolume();
            sfxSlider.value = AudioManager.Instance.GetSFXVolume();

            UpdateMusicText(musicSlider.value);
            UpdateSFXText(sfxSlider.value);
        }

        // ✅ Escucha cambios en los sliders
        musicSlider.onValueChanged.AddListener(OnMusicSliderChanged);
        sfxSlider.onValueChanged.AddListener(OnSFXSliderChanged);

        // ✅ Empieza oculto
        optionsPanel.SetActive(false);
    }

    // ─── Abrir / Cerrar ───────────────────────────────────────────

    public void ShowOptions()
    {
        optionsPanel.SetActive(true);
    }

    public void HideOptions()
    {
        optionsPanel.SetActive(false);
    }

    // ─── Sliders ──────────────────────────────────────────────────

    private void OnMusicSliderChanged(float value)
    {
        AudioManager.Instance?.SetMusicVolume(value);
        UpdateMusicText(value);
    }

    private void OnSFXSliderChanged(float value)
    {
        AudioManager.Instance?.SetSFXVolume(value);
        UpdateSFXText(value);
    }

    private void UpdateMusicText(float value)
    {
        if (musicPercentText != null)
            musicPercentText.text = $"{Mathf.RoundToInt(value)}%";
    }

    private void UpdateSFXText(float value)
    {
        if (sfxPercentText != null)
            sfxPercentText.text = $"{Mathf.RoundToInt(value)}%";
    }
}