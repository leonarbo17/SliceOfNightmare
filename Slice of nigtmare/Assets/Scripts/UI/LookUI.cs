using UnityEngine;
using UnityEngine.AI;

public class LookInteractionUI : MonoBehaviour
{
    public float interactDistance = 5f;
    public Camera playerCamera;

    [Header("UI")]
    public GameObject defaultImage;
    public GameObject interactImage;
    public GameObject eToInteractImage;

    public string interactTag = "Interactable";

    void Start()
    {
        if (defaultImage != null) defaultImage.SetActive(true);
        if (interactImage != null) interactImage.SetActive(false);
        if (eToInteractImage != null) eToInteractImage.SetActive(false);
    }

    void Update()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance))
        {
            if (hit.collider.CompareTag(interactTag))
            {
                NavMeshAgent agent = hit.collider.GetComponent<NavMeshAgent>();

                if (agent != null && agent.velocity.magnitude > 0.01f)
                {
                    SetDefaultImage();
                }
                else
                {
                    SetInteractImage();
                }
            }
            else
            {
                SetDefaultImage();
            }
        }
        else
        {
            SetDefaultImage();
        }
    }

    void SetInteractImage()
    {
        if (interactImage != null) interactImage.SetActive(true);
        if (eToInteractImage != null) eToInteractImage.SetActive(true);
        if (defaultImage != null) defaultImage.SetActive(false);
    }

    void SetDefaultImage()
    {
        if (interactImage != null) interactImage.SetActive(false);
        if (eToInteractImage != null) eToInteractImage.SetActive(false);
        if (defaultImage != null) defaultImage.SetActive(true);
    }
}
