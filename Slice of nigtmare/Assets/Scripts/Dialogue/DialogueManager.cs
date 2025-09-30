using System.Collections;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMP_Text dialogueText;

    [Header("Opciones")]
    [SerializeField] private float typingSpeed = 0.05f; // Velocidad de aparición de letras

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;   // AudioSource para clip de pizza
    [SerializeField] private AudioClip pizzaRemoveClip;

    [Header("Sonido de tipeo")]
    [SerializeField] private AudioSource typingAudioSource;
    [SerializeField] private AudioClip[] typingClips;
    [SerializeField, Range(0f, 1f)] private float typingVolume = 0.2f;
    [SerializeField] private float typingInterval = 0.05f;

    private Coroutine typingCoroutine;
    private bool isTyping;

    private string currentLine;
    private string[] currentReplies;
    private int currentReplyIndex = 0;
    private string lineBeingTyped;

    private bool removePizzaAfterFirstLine = false;
    private bool pizzaRemoved = false;
    private bool waitingForNextLine = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (dialoguePanel != null) dialoguePanel.SetActive(false);
    }

    private void Update()
    {
        if (dialoguePanel != null && dialoguePanel.activeSelf && Input.GetMouseButtonDown(0))
        {
            if (isTyping)
            {
                CompleteCurrentLine();
                return;
            }

            if (waitingForNextLine && typingCoroutine != null) return;

            if (currentReplies != null && currentReplyIndex < currentReplies.Length)
            {
                string next = currentReplies[currentReplyIndex];
                currentReplyIndex++;
                waitingForNextLine = true;
                ShowDialogueInternal(next);
            }
            else
            {
                EndDialogue();
            }
        }
    }

    public void ShowDialogue(string line, string[] replies = null, bool removePizza = false)
    {
        currentLine = line ?? "";
        currentReplies = replies;
        currentReplyIndex = 0;

        removePizzaAfterFirstLine = removePizza;
        pizzaRemoved = false;

        ShowDialogueInternal(currentLine);
    }

    private void ShowDialogueInternal(string line)
    {
        StopTypingIfAny();
        typingCoroutine = StartCoroutine(TypeLineCoroutine(line ?? ""));
    }

    private IEnumerator TypeLineCoroutine(string line)
    {
        isTyping = true;
        waitingForNextLine = true;
        lineBeingTyped = line;

        if (dialoguePanel != null) dialoguePanel.SetActive(true);
        dialogueText.text = "";

        foreach (char c in line.ToCharArray())
        {
            dialogueText.text += c;

            // Sonido de tipeo aleatorio
            if (typingAudioSource != null && typingClips.Length > 0)
            {
                AudioClip clip = typingClips[Random.Range(0, typingClips.Length)];
                typingAudioSource.PlayOneShot(clip, typingVolume);
            }

            yield return new WaitForSeconds(typingInterval);
        }

        isTyping = false;
        typingCoroutine = null;

        RemovePizzaIfNeeded();

        waitingForNextLine = false;
    }

    private void CompleteCurrentLine()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        dialogueText.text = lineBeingTyped ?? "";
        isTyping = false;

        RemovePizzaIfNeeded();
        waitingForNextLine = false;
    }

    private void RemovePizzaIfNeeded()
    {
        if (removePizzaAfterFirstLine && !pizzaRemoved && PizzaBoxGrab.HasPizzaInHand)
        {
            GameObject pizzaInHand = GameObject.FindWithTag("PizzaInHand");
            if (pizzaInHand != null)
            {
                PizzaBoxGrab.RemovePizzaFromHand(pizzaInHand);

                if (audioSource != null && pizzaRemoveClip != null)
                    audioSource.PlayOneShot(pizzaRemoveClip);
            }
            pizzaRemoved = true;
        }
    }

    private void StopTypingIfAny()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }
        isTyping = false;
    }

    public void EndDialogue()
    {
        StopTypingIfAny();
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
        if (dialogueText != null) dialogueText.text = "";

        currentLine = null;
        currentReplies = null;
        currentReplyIndex = 0;
        lineBeingTyped = null;
        removePizzaAfterFirstLine = false;
        pizzaRemoved = false;
        waitingForNextLine = false;
    }

    public bool IsTyping() => isTyping;
    public bool IsDialoguePanelOpen() => dialoguePanel != null && dialoguePanel.activeSelf;
}
