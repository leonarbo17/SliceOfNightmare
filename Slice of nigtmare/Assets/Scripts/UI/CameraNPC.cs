using UnityEngine;

namespace CameraNPCScript
{
    public class CameraNPC : MonoBehaviour
    {
        public float interactionDistance = 3f;
        public GameObject eKeyUI;
        public GameObject interactImage;
        public GameObject otherImage;

        private ExitTrigger currentDialogue;

        void Start()
        {
            eKeyUI.SetActive(false);
            interactImage.SetActive(false);
            if (otherImage != null)
                otherImage.SetActive(true);
        }

        void Update()
        {
            RaycastHit hit;

            if (Physics.Raycast(transform.position, transform.forward, out hit, interactionDistance))
            {
                if (currentDialogue == null || hit.transform.gameObject != currentDialogue.gameObject)
                {
                    currentDialogue = hit.transform.GetComponentInChildren<ExitTrigger>();
                }

                if (currentDialogue != null)
                {
                    if (!eKeyUI.activeSelf)
                    {
                        eKeyUI.SetActive(true);
                        interactImage.SetActive(true);
                        if (otherImage != null)
                            otherImage.SetActive(false);
                    }

                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        currentDialogue.SendMessage("StartDialogue");
                    }
                }
                else
                {
                    if (eKeyUI.activeSelf)
                    {
                        eKeyUI.SetActive(false);
                        interactImage.SetActive(false);
                        if (otherImage != null)
                            otherImage.SetActive(true);
                    }
                }
            }
            else
            {
                if (currentDialogue != null)
                    currentDialogue = null;

                if (eKeyUI.activeSelf)
                {
                    eKeyUI.SetActive(false);
                    interactImage.SetActive(false);
                    if (otherImage != null)
                        otherImage.SetActive(true);
                }
            }
        }
    }
}
