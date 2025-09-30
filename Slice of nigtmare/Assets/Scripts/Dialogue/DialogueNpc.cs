using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class DialogueNpc : MonoBehaviour
{
    public Camera playerCamera;
    public float interactDistance = 3f;
    public LayerMask npcLayer;
    public GameObject eToInteractUI;
    public Animator npcAnimator;                // Animator del NPC
    public string rotate180Trigger = "Rotate180";
    public bool smoothRotation = true;
    public float smoothDuration = 0.2f;

    [Header("Movimiento después del giro")]
    public Transform nextDestination;       // Destino al que irá después de girar
    public NavMeshAgent agent;              // NavMeshAgent del NPC

    private DialogueExit currentDialogue;

    private void Start()
    {
        if (eToInteractUI != null)
            eToInteractUI.SetActive(false);
    }

    private void Update()
    {
        if (playerCamera == null || DialogueManager.Instance == null)
            return;

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, npcLayer))
        {
            DialogueExit dialogue = hit.collider.GetComponent<DialogueExit>();
            if (dialogue != null)
            {
                currentDialogue = dialogue;
                bool hasPizza = PizzaBoxGrab.HasPizzaInHand;

                bool isWithPizza = hasPizza && !string.IsNullOrEmpty(dialogue.dialogueWithPizza);
                string npcLine = isWithPizza ? dialogue.dialogueWithPizza : dialogue.dialogueLine;
                string[] playerReplies = isWithPizza ? dialogue.replyWithPizza : dialogue.replyLines;

                if (!DialogueManager.Instance.IsDialoguePanelOpen())
                {
                    if (eToInteractUI != null) eToInteractUI.SetActive(true);

                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        DialogueManager.Instance.ShowDialogue(npcLine, playerReplies, isWithPizza);
                        if (eToInteractUI != null) eToInteractUI.SetActive(false);

                        if (isWithPizza)
                            StartCoroutine(Do180AfterDialogue());
                    }
                }
                else
                {
                    if (eToInteractUI != null) eToInteractUI.SetActive(false);
                }

                return;
            }
        }

        if (eToInteractUI != null) eToInteractUI.SetActive(false);
        currentDialogue = null;
    }

    private IEnumerator Do180AfterDialogue()
    {
        while (DialogueManager.Instance.IsDialoguePanelOpen())
            yield return null;

        yield return new WaitForSeconds(0.2f);

        if (npcAnimator != null)
            npcAnimator.SetTrigger(rotate180Trigger);

        Quaternion startRot = transform.rotation;
        Quaternion endRot = startRot * Quaternion.Euler(0f, 180f, 0f);

        if (smoothRotation)
        {
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime / smoothDuration;
                transform.rotation = Quaternion.Slerp(startRot, endRot, t);
                yield return null;
            }
        }
        else
        {
            transform.rotation = endRot;
        }

        // Inicia el movimiento hacia el destino
        if (agent != null && nextDestination != null)
        {
            agent.SetDestination(nextDestination.position);

            if (npcAnimator != null)
                npcAnimator.SetFloat("Speed", agent.speed);

            // 🚀 Dispara el conteo de apagón desde aquí
            NpcLeaveEvent.TriggerBlackoutCountdown();
        }
    }
}
