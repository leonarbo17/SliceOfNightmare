using UnityEngine;

public class DoorToggle : MonoBehaviour
{
    [Header("Interacción")]
    public Camera playerCamera;
    public float interactDistance = 3f;
    public LayerMask doorLayer;
    public string requiredTag = "Door";
    public KeyCode interactKey = KeyCode.E;

    [Header("Rotación")]
    public float rotationAmount = 90f; // grados sobre X
    public float smoothSpeed = 5f;

    [Header("Sonidos")]
    public AudioSource audioSource;
    public AudioClip openClip;
    public AudioClip closeClip;

    private bool isOpen = false;
    private Transform currentDoor;
    private Quaternion targetRotation;

    private void Update()
    {
        if (playerCamera == null) return;

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, doorLayer))
        {
            if (hit.collider.CompareTag(requiredTag))
            {
                currentDoor = hit.collider.transform;

                if (Input.GetKeyDown(interactKey))
                {
                    ToggleDoor();
                }
            }
        }

        // Rotación suave hacia el target
        if (currentDoor != null)
        {
            currentDoor.rotation = Quaternion.Slerp(currentDoor.rotation, targetRotation, Time.deltaTime * smoothSpeed);
        }
    }

    private void ToggleDoor()
    {
        if (currentDoor == null) return;

        isOpen = !isOpen;

        // Rotación sobre X relativa a la rotación actual
        float xRotation = isOpen ? rotationAmount : -rotationAmount;
        targetRotation = currentDoor.rotation * Quaternion.Euler(xRotation, 0f, 0f);

        // Sonidos
        if (audioSource != null)
            audioSource.PlayOneShot(isOpen ? openClip : closeClip);
    }
}
