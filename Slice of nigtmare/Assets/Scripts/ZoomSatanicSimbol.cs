using System.Collections;
using UnityEngine;

public class RitualCameraEvent : MonoBehaviour
{
    [Header("Cámara")]
    public Camera playerCamera;
    public Transform ritualTarget;         // Objeto que quieres enfocar (ritual)
    public float focusDuration = 2f;       // Tiempo para girar la cámara
    public float zoomFOV = 40f;            // FOV durante el zoom
    private float originalFOV;

    [Header("Jugador")]
    public playterMove playerMove;         // Script de movimiento

    [Header("Diálogo")]
    [TextArea(2, 4)] public string dialogueLine;
    [TextArea(2, 4)] public string[] replyLines;

    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;

        if (other.CompareTag("Player"))
        {
            triggered = true;

            if (playerMove == null) playerMove = other.GetComponent<playterMove>();
            if (playerCamera != null && ritualTarget != null)
            {
                StartCoroutine(FocusOnRitual());
            }
        }
    }

    private IEnumerator FocusOnRitual()
    {
        // Bloquear controles
        if (playerMove != null)
            playerMove.SetPlayerControl(false);

        // Guardar valores originales
        Quaternion originalRot = playerCamera.transform.rotation;
        originalFOV = playerCamera.fieldOfView;

        // Calcular la rotación hacia el ritual
        Vector3 dirToTarget = (ritualTarget.position - playerCamera.transform.position).normalized;
        Quaternion targetRot = Quaternion.LookRotation(dirToTarget, Vector3.up);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / focusDuration;

            // Interpolar rotación
            playerCamera.transform.rotation = Quaternion.Slerp(originalRot, targetRot, t);

            // Interpolar FOV
            playerCamera.fieldOfView = Mathf.Lerp(originalFOV, zoomFOV, t);

            yield return null;
        }

        // Lanzar diálogo
        if (DialogueManager.Instance != null && !string.IsNullOrEmpty(dialogueLine))
        {
            DialogueManager.Instance.ShowDialogue(dialogueLine, replyLines, false);
        }

        // Esperar a que termine el diálogo
        while (DialogueManager.Instance != null && DialogueManager.Instance.IsDialoguePanelOpen())
        {
            yield return null;
        }

        // Regresar cámara a estado original
        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / focusDuration;
            playerCamera.transform.rotation = Quaternion.Slerp(targetRot, originalRot, t);
            playerCamera.fieldOfView = Mathf.Lerp(zoomFOV, originalFOV, t);
            yield return null;
        }

        // Reactivar controles
        if (playerMove != null)
            playerMove.SetPlayerControl(true);

        // Desactivar trigger
        gameObject.SetActive(false);
    }
}
