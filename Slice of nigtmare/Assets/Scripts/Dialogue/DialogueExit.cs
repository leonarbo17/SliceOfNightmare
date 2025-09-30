using UnityEngine;

public class DialogueExit : MonoBehaviour
{
    [Header("Diálogo normal (NPC)")]
    [TextArea(2, 4)] public string dialogueLine;
    [TextArea(2, 4)] public string[] replyLines;

    [Header("Diálogo con pizza en mano (opcional)")]
    [TextArea(2, 4)] public string dialogueWithPizza;
    [TextArea(2, 4)] public string[] replyWithPizza;
}
