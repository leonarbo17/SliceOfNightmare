using UnityEngine;
using System.Collections;

public class LeverGenerator : MonoBehaviour
{
    [Header("Configuración de la palanca")]
    public GameObject leverHandle;
    public float rotationSpeed = 120f;
    public float targetRotationX = -93.5f;

    [Header("Activaciones")]
    public AudioSource audio1;
    public AudioSource audio2;
    public GameObject lightsParent;
    public Collider colliderToEnable;
    public GameObject satanicSymbol;

    [Header("Diálogo")]
    [TextArea(2, 4)] public string dialogueLine;
    [TextArea(2, 4)] public string[] replyLines;

    private bool isInteracting = false;

    public void ActivateLever()
    {
        if (!isInteracting)
            StartCoroutine(DoLeverAction());
    }

    private IEnumerator DoLeverAction()
    {
        isInteracting = true;

        // 1. Rotar la palanca
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

        // 2. Activar audios
        if (audio1 != null) audio1.Play();
        if (audio2 != null) audio2.Play();

        // 3. Activar luces
        if (lightsParent != null) lightsParent.SetActive(true);

        // 4. Activar collider
        if (colliderToEnable != null) colliderToEnable.enabled = true;

        // 5. Activar símbolo satánico
        if (satanicSymbol != null) satanicSymbol.SetActive(true);

        // 6. Lanzar diálogo
        yield return new WaitForEndOfFrame();
        if (DialogueManager.Instance != null && !string.IsNullOrEmpty(dialogueLine))
            DialogueManager.Instance.ShowDialogue(dialogueLine, replyLines, false);

        isInteracting = false;
    }
}
