using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events; // <-- LIBRERÍA NUEVA E INDISPENSABLE

public class DialogueController : MonoBehaviour
{
    [Header("UI del Diálogo")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;
    public GameObject navigationArrows;

    [Header("Activadores de Estaciones")]
    public GameObject stationTriggersParent;

    [Header("Conexiones")]
    public ClientSystem clientSystem;

    [Header("Ajustes")]
    public float typingSpeed = 0.04f;

    // --- NUEVA ESTRUCTURA: TEXTO + EVENTO ---
    [System.Serializable]
    public struct DialogueLine
    {
        [TextArea(2, 4)]
        public string text;
        public UnityEvent onLineTrigger; // El "botón" mágico en el Inspector
    }

    [Header("Diálogo Inicial (Tutorial)")]
    public DialogueLine[] tutorialDialogues; // Ahora es un array de la nueva estructura

    [Header("Saludos Aleatorios (Al regresar)")]
    public DialogueLine[] welcomeDialogues;

    [System.Serializable]
    public struct ContextDialogue
    {
        public string stationTag;
        public DialogueLine[] dialogues; // También actualizado aquí
    }

    [Header("Diálogos Contextuales")]
    public ContextDialogue[] stationDialogues;

    [Header("Audio de B.E.L.")]
    public AudioClip belBeepSound;

    private int currentLineIndex = 0;
    private bool isTyping = false;
    private Coroutine typingCoroutine;

    private DialogueLine[] currentActiveDialogue; // Comodín actualizado
    private bool isTutorialDialogueActive = false;
    private HashSet<string> stationsVisited = new HashSet<string>();

    void Start()
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
        if (navigationArrows != null)
            navigationArrows.SetActive(true);

        // Validamos el estado global del tutorial
        if (GameManager.Instance != null && GameManager.Instance.isTutorialCompleted)
        {
            // El jugador ya fue al mundo, el tutorial acabó. Apagamos TODOS los activadores.
            if (stationTriggersParent != null)
            {
                stationTriggersParent.SetActive(false);
            }

            PlayRandomWelcome();
        }
        else
        {
            // Es la primera vez. Mantenemos los activadores encendidos.
            if (stationTriggersParent != null)
            {
                stationTriggersParent.SetActive(true);
            }

            StartTutorialDialogue();
        }
    }

    public void OnDialogueClicked()
    {
        if (isTyping)
        {
            StopCoroutine(typingCoroutine);
            dialogueText.text = currentActiveDialogue[currentLineIndex].text; // Accedemos al .text
            isTyping = false;
        }
        else
        {
            NextLine();
        }
    }

    public void StartTutorialDialogue()
    {
        dialoguePanel.SetActive(true);
        currentLineIndex = 0;

        currentActiveDialogue = tutorialDialogues;
        isTutorialDialogueActive = true;
        if (navigationArrows != null)
        {
            navigationArrows.SetActive(false); // Oculta las flechas para que el jugador se centre en leer
        }

        if (currentActiveDialogue.Length > 0)
        {
            // Disparamos el evento de la PRIMERA línea
            currentActiveDialogue[currentLineIndex].onLineTrigger?.Invoke();
            typingCoroutine = StartCoroutine(
                TypeLine(currentActiveDialogue[currentLineIndex].text)
            );
        }
    }

    private IEnumerator TypeLine(string line)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char letter in line.ToCharArray())
        {
            dialogueText.text += letter;

            // TRUCO: Solo hacemos el "beep" si la letra NO es un espacio vacío
            if (letter != ' ' && belBeepSound != null)
            {
                AudioManager.Instance.PlaySFXRandomPitch(belBeepSound, 0.95f, 1.05f);
            }

            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    private void NextLine()
    {
        currentLineIndex++;

        if (currentLineIndex < currentActiveDialogue.Length)
        {
            // Disparamos el evento de la NUEVA línea
            currentActiveDialogue[currentLineIndex].onLineTrigger?.Invoke();
            typingCoroutine = StartCoroutine(
                TypeLine(currentActiveDialogue[currentLineIndex].text)
            );
        }
        else
        {
            EndDialogue();
        }
    }

    private void EndDialogue()
    {
        dialoguePanel.SetActive(false);

        if (navigationArrows != null)
        {
            navigationArrows.SetActive(true);
        }

        if (isTutorialDialogueActive)
        {
            // Usamos directamente el GameManager para mantener una sola fuente de la verdad
            if (GameManager.Instance != null)
            {
                GameManager.Instance.isTutorialCompleted = true;
                GameManager.Instance.SaveGlobalProgress();
            }

            if (stationTriggersParent != null)
            {
                //stationTriggersParent.SetActive(false);
            }
        }
    }

    public void TryStartContextDialogue(string tagLookedAt)
    {
        // Tu lógica original que funciona perfecto:
        if (dialoguePanel.activeSelf || stationsVisited.Contains(tagLookedAt))
            return;

        foreach (var station in stationDialogues)
        {
            if (station.stationTag == tagLookedAt)
            {
                stationsVisited.Add(tagLookedAt); // Anota que ya la vio en esta sesión

                currentActiveDialogue = station.dialogues;
                currentLineIndex = 0;
                isTutorialDialogueActive = false;

                dialoguePanel.SetActive(true);
                currentActiveDialogue[currentLineIndex].onLineTrigger?.Invoke();
                typingCoroutine = StartCoroutine(
                    TypeLine(currentActiveDialogue[currentLineIndex].text)
                );
                return;
            }
        }
    }

    private void PlayRandomWelcome()
    {
        // Nos aseguramos de que haya saludos escritos en el Inspector
        if (welcomeDialogues != null && welcomeDialogues.Length > 0)
        {
            int randomIndex = Random.Range(0, welcomeDialogues.Length);

            // Configuramos la interfaz
            dialoguePanel.SetActive(true);
            if (navigationArrows != null)
            {
                navigationArrows.SetActive(false);
            }

            // Engañamos a tu sistema haciéndole creer que el diálogo actual es solo esta frase
            currentActiveDialogue = new DialogueLine[] { welcomeDialogues[randomIndex] };
            currentLineIndex = 0;
            isTutorialDialogueActive = false; // No es el tutorial, es solo un saludo

            // Disparamos el evento (por si quieres que B.E.L. parpadee, por ejemplo)
            currentActiveDialogue[currentLineIndex].onLineTrigger?.Invoke();

            typingCoroutine = StartCoroutine(
                TypeLine(currentActiveDialogue[currentLineIndex].text)
            );
        }
    }
}
