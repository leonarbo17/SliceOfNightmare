using UnityEngine;
using UnityEngine.AI;

public class LousineScript : MonoBehaviour
{
    [Header("Configuración del NPC")]
    [SerializeField] Transform destination;       // Destino del NPC
    [SerializeField] AudioSource footstepAudio;   // Audio de pasos
    [SerializeField] Animator animator;           // Animator del NPC

    [Header("Trigger externo")]
    [SerializeField] GameObject triggerObject;    // Objeto con collider (isTrigger activado)
    [SerializeField] string playerTag = "Player"; // Tag del jugador

    private NavMeshAgent navMeshAgent;
    private bool canWalk = false;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();

        if (footstepAudio != null)
        {
            footstepAudio.loop = true;
            footstepAudio.playOnAwake = false;
        }

        // Aseguramos que el trigger tenga script controlador
        if (triggerObject != null && triggerObject.GetComponent<LousineTrigger>() == null)
        {
            var trigger = triggerObject.AddComponent<LousineTrigger>();
            trigger.Initialize(this, playerTag);
        }
    }

    void Update()
    {
        if (!canWalk) return;

        if (navMeshAgent != null && animator != null)
        {
            float speed = navMeshAgent.velocity.magnitude;
            animator.SetFloat("Speed", speed);
            HandleFootsteps(speed);
        }
    }

    public void ActivateNPC()
    {
        if (destination != null && navMeshAgent != null)
        {
            navMeshAgent.SetDestination(destination.position);
            canWalk = true;
        }
    }

    private void HandleFootsteps(float speed)
    {
        if (footstepAudio == null) return;

        if (speed > 0.1f && !footstepAudio.isPlaying)
        {
            footstepAudio.Play();
        }
        else if (speed <= 0.1f && footstepAudio.isPlaying)
        {
            footstepAudio.Stop();
        }
    }
}

// ----------------- SCRIPT DEL TRIGGER -----------------
public class LousineTrigger : MonoBehaviour
{
    private LousineScript lousineNPC;
    private string playerTag;

    public void Initialize(LousineScript npc, string tag)
    {
        lousineNPC = npc;
        playerTag = tag;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            lousineNPC.ActivateNPC();
            gameObject.SetActive(false); // opcional: desactivar trigger después de usarlo
        }
    }
}
