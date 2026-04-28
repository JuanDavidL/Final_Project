using UnityEngine;
using UnityEngine.InputSystem;

public class MenuController : MonoBehaviour
{
    [Header("UI del Menú")]
    public GameObject menuPanel;

    [Header("Interfaz de Juego (HUD)")]
    [SerializeField]
    private GameObject hudGameContainer; // Arrastra aquí el HUD_Game_Container

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
}
