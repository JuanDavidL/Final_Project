using UnityEngine;
using UnityEngine.InputSystem;

public class MenuController : MonoBehaviour
{
    [Header("UI del Menú")]
    public GameObject menuPanel;

    [Header("Elementos de la Escena (Opcionales)")]
    [SerializeField]
    private GameObject shipArrowsContainer; 
    void Start()
    {
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
                    PauseGame();
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

        // --- APAGAMOS LAS FLECHAS ---
        if (shipArrowsContainer != null)
            shipArrowsContainer.SetActive(false);
    }

    private void PauseGame()
    {
        menuPanel.SetActive(true);
        Time.timeScale = 0f;

        // --- APAGAMOS LAS FLECHAS ---
        if (shipArrowsContainer != null)
            shipArrowsContainer.SetActive(false);
    }

    public void ResumeGame()
    {
        menuPanel.SetActive(false);
        Time.timeScale = 1f;

        // --- ENCENDEMOS LAS FLECHAS ---
        if (shipArrowsContainer != null)
            shipArrowsContainer.SetActive(true);
    }
}
