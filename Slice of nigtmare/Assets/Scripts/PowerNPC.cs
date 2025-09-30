using UnityEngine;

public class NpcLeaveEvent : MonoBehaviour
{
    [Header("Luces")]
    public GameObject lightsParent;

    [Header("Sonidos")]
    public AudioSource soundOnBlackout;
    public AudioSource soundToStop1;
    public AudioSource soundToStop2;

    [Header("Timer")]
    public float waitTime = 25f;

    [Header("Diálogo post-apagón")]
    [TextArea(2, 4)] public string blackoutDialogueLine;
    [TextArea(2, 4)] public string[] blackoutReplies;

    [Header("Objetos a cambiar de tag (incluyendo hijos)")]
    public GameObject objectToChangeTag1;
    public string newTag1;
    public GameObject objectToChangeTag2;
    public string newTag2;

    private static NpcLeaveEvent instance;

    private void Awake()
    {
        instance = this;
    }

    public static void TriggerBlackoutCountdown()
    {
        if (instance != null)
            instance.StartCoroutine(instance.StartBlackoutEvent());
    }

    private System.Collections.IEnumerator StartBlackoutEvent()
    {
        // Espera desde que el NPC empieza a caminar
        yield return new WaitForSeconds(waitTime);

        // Apagar luces
        if (lightsParent != null)
            lightsParent.SetActive(false);

        // Detener otros sonidos
        if (soundToStop1 != null) soundToStop1.Stop();
        if (soundToStop2 != null) soundToStop2.Stop();

        // Reproducir sonido de apagón
        if (soundOnBlackout != null)
        {
            soundOnBlackout.Play();
            yield return new WaitForSeconds(soundOnBlackout.clip.length);
        }

        // Cambiar tags del primer objeto y todos sus hijos
        if (objectToChangeTag1 != null && !string.IsNullOrEmpty(newTag1))
        {
            foreach (Transform t in objectToChangeTag1.GetComponentsInChildren<Transform>(true))
            {
                t.gameObject.tag = newTag1;
            }
        }

        // Cambiar tags del segundo objeto y todos sus hijos
        if (objectToChangeTag2 != null && !string.IsNullOrEmpty(newTag2))
        {
            foreach (Transform t in objectToChangeTag2.GetComponentsInChildren<Transform>(true))
            {
                t.gameObject.tag = newTag2;
            }
        }

        // Mostrar diálogo después del apagón
        if (!string.IsNullOrEmpty(blackoutDialogueLine))
        {
            DialogueManager.Instance.ShowDialogue(blackoutDialogueLine, blackoutReplies, false);
        }
    }
}
