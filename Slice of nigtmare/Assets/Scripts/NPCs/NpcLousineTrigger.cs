using UnityEngine;

public class NpcLousineTrigger : MonoBehaviour
{
    public NpcLousine npcLousine; // Arrastra tu NPC Lousine aquí

    private void OnTriggerEnter(Collider other)
    {
        // Si el jugador entra y este trigger tiene la tag correcta
        if (other.CompareTag("Player") && CompareTag("TriggerLousine"))
        {
            npcLousine.ActivarMovimientoDesdeTrigger(gameObject.tag);
        }
    }
}
