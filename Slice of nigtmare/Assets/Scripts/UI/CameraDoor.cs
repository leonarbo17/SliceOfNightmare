using UnityEngine;

namespace CameraDoorScript
{
    public class CameraDoor : MonoBehaviour
    {
        public float DistanceOpen = 3f;

        private DoorScript.Door currentDoor;

        void Start()
        {
            var doors = FindObjectsOfType<DoorScript.Door>();
            foreach (var d in doors)
            {
                bool dummy = d.enabled;
            }
        }

        void Update()
        {
            RaycastHit hit;

            if (Physics.Raycast(transform.position, transform.forward, out hit, DistanceOpen))
            {
                if (currentDoor == null || hit.transform.gameObject != currentDoor.gameObject)
                {
                    currentDoor = hit.transform.GetComponent<DoorScript.Door>();
                }

                if (currentDoor != null)
                {
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        currentDoor.OpenDoor();
                    }
                }
            }
            else
            {
                if (currentDoor != null)
                    currentDoor = null;
            }
        }
    }
}
