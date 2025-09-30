using UnityEngine;
using System.Collections;

public class RaycastInteraction : MonoBehaviour
{
    [Header("Raycast")]
    public Camera playerCamera;
    public float interactDistance = 3f;
    public LayerMask targetLayer;          // Capa que debe tener el objeto detectado
    public string targetTag = "Interact";  // Tag que debe tener el objeto detectado

    [Header("Objeto a rotar (no el golpeado)")]
    public GameObject objectToRotate;      // <- aquí arrastras el objeto en el Inspector
    public float rotationSpeed = 120f;
    public float targetRotationX = -93.5f; // Ángulo final en X

    [Header("Sonido")]
    public AudioSource audioSource;
    public AudioClip interactionClip;

    [Header("Objetos para cambiar Tag")]
    public GameObject object1;
    public string newTag1 = "Untagged";
    public GameObject object2;
    public string newTag2 = "Untagged";

    private bool isInteracting = false;

    void Update()
    {
        if (isInteracting) return;

        if (playerCamera == null) return;

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, targetLayer))
        {
            if (hit.collider.CompareTag(targetTag))
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    if (objectToRotate != null)
                        StartCoroutine(DoInteraction(objectToRotate));
                }
            }
        }
    }

    private IEnumerator DoInteraction(GameObject target)
    {
        isInteracting = true;

        // 1. Reproducir sonido
        if (audioSource != null && interactionClip != null)
            audioSource.PlayOneShot(interactionClip);

        // 2. Rotar objeto desde su rotación actual hasta targetRotationX
        Quaternion startRot = target.transform.rotation;
        Quaternion endRot = Quaternion.Euler(targetRotationX, startRot.eulerAngles.y, startRot.eulerAngles.z);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * rotationSpeed / 90f;
            target.transform.rotation = Quaternion.Slerp(startRot, endRot, t);
            yield return null;
        }
        target.transform.rotation = endRot; // fijar ángulo exacto al final

        // 3. Cambiar tags de los objetos
        if (object1 != null) object1.tag = newTag1;
        if (object2 != null) object2.tag = newTag2;

        isInteracting = false;
    }
}
 