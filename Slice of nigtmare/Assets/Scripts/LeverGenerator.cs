using UnityEngine;
using System.Collections;

public class LeverInteraction : MonoBehaviour
{
    [Header("Referencias de la palanca")]
    public GameObject leverHandle;
    public float targetRotationX = -93.5f;     // Rotación final
    public float rotationSpeed = 120f;
    public AudioSource leverSound;             // Sonido de la palanca moviéndose

    [Header("Cámara")]
    public Camera playerCamera;
    public Transform cameraTarget;             // Punto al que se moverá la cámara
    public float cameraMoveDuration = 1.5f;    // Tiempo de transición

    [Header("Activaciones extra")]
    public AudioSource audio1;
    public AudioSource audio2;
    public GameObject lightsParent;
    public Collider colliderToEnable;
    public GameObject satanicSymbol;

    [Header("Diálogo inicial")]
    [TextArea(2, 4)] public string dialogueLine;
    [TextArea(2, 4)] public string[] replyLines;

    private bool activated = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !activated)
        {
            activated = true;
            StartCoroutine(HandleLeverSequence());
        }
    }

    private IEnumerator HandleLeverSequence()
    {
        // 1. Lanzar diálogo
        if (DialogueManager.Instance != null && !string.IsNullOrEmpty(dialogueLine))
        {
            DialogueManager.Instance.ShowDialogue(dialogueLine, replyLines, false);
        }

        // 2. Mover cámara al objetivo
        if (playerCamera != null && cameraTarget != null)
        {
            Transform camTransform = playerCamera.transform;
            Vector3 startPos = camTransform.position;
            Quaternion startRot = camTransform.rotation;

            Vector3 endPos = cameraTarget.position;
            Quaternion endRot = cameraTarget.rotation;

            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime / cameraMoveDuration;
                camTransform.position = Vector3.Lerp(startPos, endPos, t);
                camTransform.rotation = Quaternion.Slerp(startRot, endRot, t);
                yield return null;
            }
        }

        // --- Al terminar la transición empieza la activación ---
        yield return new WaitForSeconds(0.5f);

        // 3. Reproducir sonido de la palanca
        if (leverSound != null)
            leverSound.Play();

        // 4. Rotar palanca
        if (leverHandle != null)
        {
            Quaternion startRot = leverHandle.transform.rotation;
            Quaternion endRot = Quaternion.Euler(targetRotationX, startRot.eulerAngles.y, startRot.eulerAngles.z);

            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime * rotationSpeed / 90f;
                leverHandle.transform.rotation = Quaternion.Slerp(startRot, endRot, t);
                yield return null;
            }
            leverHandle.transform.rotation = endRot;
        }

        // 5. Activar audios extra
        if (audio1 != null) audio1.Play();
        if (audio2 != null) audio2.Play();

        // 6. Activar luces
        if (lightsParent != null) lightsParent.SetActive(true);

        // 7. Activar collider
        if (colliderToEnable != null) colliderToEnable.enabled = true;

        // 8. Activar símbolo satánico
        if (satanicSymbol != null) satanicSymbol.SetActive(true);
    }
}
