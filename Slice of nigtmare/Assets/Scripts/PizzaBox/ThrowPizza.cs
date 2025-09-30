using UnityEngine;

public class PizzaRaycastInteract : MonoBehaviour
{
    [Header("Raycast Settings")]
    public float rayDistance = 3f;
    public LayerMask trashLayer;

    [Header("Pizza Settings")]
    public GameObject pizzaBox;           // Caja de pizza que el jugador puede lanzar

    [Header("Trash Settings")]
    public string targetTag = "PizzaTarget";   // Tag que se asignará al bote mientras se mira
    public AudioSource trashAudioSource;       // AudioSource que se reproducirá al interactuar

    private GameObject detectedTrash;
    private string originalTag;

    void Update()
    {
        // Solo funciona si la caja de pizza está activa
        if (!pizzaBox.activeInHierarchy)
            return;

        DetectTrash();

        if (detectedTrash != null && Input.GetKeyDown(KeyCode.E))
        {
            InteractWithTrash();
        }
    }

    private void DetectTrash()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, rayDistance, trashLayer))
        {
            GameObject trash = hit.collider.gameObject;

            if (detectedTrash != trash)
            {
                // Restauramos la tag del bote anterior
                if (detectedTrash != null)
                    detectedTrash.tag = originalTag;

                detectedTrash = trash;
                originalTag = detectedTrash.tag;

                // Cambiamos la tag mientras lo mira
                detectedTrash.tag = targetTag;
            }

            Debug.DrawLine(ray.origin, hit.point, Color.green);
        }
        else
        {
            // Restauramos la tag si deja de mirar
            if (detectedTrash != null)
                detectedTrash.tag = originalTag;

            detectedTrash = null;
        }
    }

    private void InteractWithTrash()
    {
        if (detectedTrash != null)
        {
            // Reproducir el AudioSource asignado en el inspector
            if (trashAudioSource != null)
                trashAudioSource.Play();
            else
                Debug.LogWarning("No se ha asignado AudioSource en el inspector!");

            // Desactivar la caja de pizza
            pizzaBox.SetActive(false);

            // Restaurar la tag del bote
            detectedTrash.tag = originalTag;
            detectedTrash = null;

            // Avisar al sistema de agarre que la pizza ya no está en la mano
            PizzaBoxGrab.RemovePizzaFromHand(pizzaBox);
        }
    }
}
