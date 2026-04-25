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

        if (isTutorialDialogueActive)
        {
            PlayerPrefs.SetInt("TutorialVisto", 1);
            PlayerPrefs.Save();

            if (clientSystem != null)
            {
                // clientSystem.StartTutorialMode();
            }
        }
    }

    public void TryStartContextDialogue(string tagLookedAt)
    {
        if (dialoguePanel.activeSelf || stationsVisited.Contains(tagLookedAt))
            return;

        foreach (var station in stationDialogues)
        {
            if (station.stationTag == tagLookedAt)
            {
                stationsVisited.Add(tagLookedAt);

                currentActiveDialogue = station.dialogues;
                currentLineIndex = 0;
                isTutorialDialogueActive = false;

                dialoguePanel.SetActive(true);

                // Disparamos el evento de la primera línea de contexto
                currentActiveDialogue[currentLineIndex].onLineTrigger?.Invoke();
                typingCoroutine = StartCoroutine(
                    TypeLine(currentActiveDialogue[currentLineIndex].text)
                );
                return;
            }
        }
    }
}
