using UnityEngine;

public class ExitTrigger : MonoBehaviour
{
    [Header("Diálogo al intentar salir")]
    [TextArea(2, 4)] public string dialogueLine;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            DialogueManager.Instance.ShowDialogue(dialogueLine, null, false);
        }
    }
}
