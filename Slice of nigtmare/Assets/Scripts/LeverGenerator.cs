using System.Collections;
using UnityEngine;

public class LeverGenerator : MonoBehaviour
{
    [Header("Raycast Settings")]
    public float interactDistance = 3f;
    public LayerMask leverLayer;        // Capa de la palanca
    public string leverTag = "Lever";   // Tag de la palanca

    [Header("Palanca")]
    public Transform leverHandle;       // La palanca que debe rotar
    public AudioSource leverSound;      // Sonido cuando se mueve la palanca
    [Tooltip("Ángulo X al que rotará la palanca (editable en Inspector)")]
    public float targetRotationX = -83.5f;  // Editable en Inspector
    public float rotationSpeed = 2f;        // Velocidad de rotación de la palanca

    [Header("Eventos al activar palanca")]
    public AudioSource blackoutSound;   // Sonido cuando se va la luz
    public AudioSource satanicSound;    // Sonido del símbolo satánico
    public GameObject satanicSymbol;    // Símbolo satánico
    public GameObject lightsParent;     // Objeto con luces a activar
    public GameObject colliderObject;   // Objeto que contiene el collider

    [Header("Diálogo")]
    [TextArea(2, 4)]
    public string dialogueLine;         // Línea que dirá el NPC
    public string[] replyLines;         // Opciones de respuesta del jugador

    [Header("Cambio de Tags")]
    public GameObject objectToChangeTag;   // Objeto al que se le cambiará el tag
    public string newTag = "Untagged";     // Nuevo tag

    private bool activated = false;

    void Update()
    {
        if (activated) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, interactDistance, leverLayer))
            {
                if (hit.collider.CompareTag(leverTag))
                {
                    StartCoroutine(ActivateLever());
                }
            }
        }
    }

    private IEnumerator ActivateLever()
    {
        activated = true;

        // Sonido de palanca
        if (leverSound != null) leverSound.Play();

        // Rotación de palanca
        if (leverHandle != null)
        {
            Quaternion startRot = leverHandle.localRotation;
            Quaternion endRot = Quaternion.Euler(targetRotationX, startRot.eulerAngles.y, startRot.eulerAngles.z);
            float t = 0f;

            while (t < 1f)
            {
                t += Time.deltaTime * rotationSpeed;
                leverHandle.localRotation = Quaternion.Slerp(startRot, endRot, t);
                yield return null;
            }
            leverHandle.localRotation = endRot;
        }

        // Activar eventos
        if (blackoutSound != null) blackoutSound.Play();
        if (satanicSound != null) satanicSound.Play();
        if (satanicSymbol != null) satanicSymbol.SetActive(true);
        if (lightsParent != null) lightsParent.SetActive(true);
        if (colliderObject != null) colliderObject.SetActive(true);

        // Cambiar tag de objeto y sus hijos
        if (objectToChangeTag != null)
        {
            ChangeTagRecursive(objectToChangeTag.transform, newTag);
        }

        // Lanzar diálogo usando DialogueManager
        if (DialogueManager.Instance != null && !string.IsNullOrEmpty(dialogueLine))
        {
            DialogueManager.Instance.ShowDialogue(dialogueLine, replyLines, false);
        }
    }

    private void ChangeTagRecursive(Transform parent, string newTag)
    {
        parent.tag = newTag;
        foreach (Transform child in parent)
        {
            ChangeTagRecursive(child, newTag);
        }
    }
}
