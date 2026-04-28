using UnityEngine;
using UnityEngine.InputSystem;

public class MenuController : MonoBehaviour
{
    [Header("UI del Menú")]
    public GameObject menuPanel;
    public GameObject MusicPanel;

    [Header("Interfaz de Juego (HUD)")]
    [SerializeField]
    private GameObject hudGameContainer; // Arrastra aquí el HUD_Game_Container

    [Header("Opciones")]
    public OptionsController optionsController;

    [Header("SFX Menu")]
    public GameObject playButton;
    public GameObject optionsButton;
    public GameObject exitButton;

    [Header("Configuración")]
    public bool enableEscapeKey = true; // ✅ desactívalo en la escena del planeta
    

    void Start()
    {
        // Al empezar, nos aseguramos de que el menú esté ON y el HUD esté OFF
        if (GameManager.Instance != null && GameManager.Instance.hasGameStarted)
        {
            ResumeGame();
        }
        else
        {
            ShowMainMenu();
        }
    }

    void Update()
    {
        if (!enableEscapeKey) return;
        
        if (GameManager.Instance != null && GameManager.Instance.hasGameStarted)
        {
            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                if (menuPanel.activeSelf)
                {
                    ResumeGame();
                }
                else
                {
                    ShowMainMenu();
                }
            }
        }
    }

    public void OnPlayButtonClicked()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.hasGameStarted = true;
        }
        ResumeGame();
    }

    private void ShowMainMenu()
    {
        menuPanel.SetActive(true);
        Time.timeScale = 0f;

        // Apagamos todo el HUD de juego mientras estemos en el menú
        if (hudGameContainer != null)
            hudGameContainer.SetActive(false);
    }

    public void ResumeGame()
    {
        menuPanel.SetActive(false);
        Time.timeScale = 1f;

        // ¡Encendemos el HUD de juego!
        if (hudGameContainer != null)
            hudGameContainer.SetActive(true);
    }

    public void ShowOptions()
    {
        playButton.SetActive(false);
        optionsButton.SetActive(false);
        exitButton.SetActive(false);
        MusicPanel.SetActive(true);
    }

    public void HideOptions()
    {
        playButton.SetActive(true);
        optionsButton.SetActive(true);
        exitButton.SetActive(true);
        MusicPanel.SetActive(false);
    }

    public void OnOptionsButtonClicked()
    {
        optionsController.ShowOptions();
    }

    public void OnOptionsBackClicked()
    {
        // ✅ Vuelve al menú
        optionsController.HideOptions();
    }

    public void OnExitButtonClicked()
    {
        GameManager.Instance?.SaveGlobalProgress();

        // ✅ En el Editor detiene el Play, en el juego cierra la aplicación
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
