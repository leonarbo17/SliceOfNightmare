using UnityEngine;
using UnityEngine.AI;

public class TriggerMoveNPC : MonoBehaviour
{
    [Header("NPC y destino")]
    public NavMeshAgent npcAgent;        // Arrastra el NavMeshAgent del NPC aquí
    public Transform destination;        // Destino al que irá el NPC

    [Header("Opcional: sonido de pasos")]
    public AudioSource footstepAudio;    // Sonido de pasos del NPC

    private Animator npcAnimator;

    private void Start()
    {
        if (npcAgent == null)
        {
            Debug.LogError("No se asignó NavMeshAgent al NPC.");
            enabled = false;
            return;
        }

        npcAnimator = npcAgent.GetComponent<Animator>();

        if (footstepAudio != null)
        {
            footstepAudio.loop = true;
            footstepAudio.playOnAwake = false;
        }
    }

    private void Update()
    {
        if (npcAgent != null && npcAnimator != null)
        {
            float speed = npcAgent.velocity.magnitude;
            npcAnimator.SetFloat("Speed", speed);

            HandleFootsteps(speed);
        }
    }

    private void HandleFootsteps(float speed)
    {
        if (footstepAudio == null) return;

        if (speed > 0.1f && !footstepAudio.isPlaying)
            footstepAudio.Play();
        else if (speed <= 0.1f && footstepAudio.isPlaying)
            footstepAudio.Stop();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && npcAgent != null && destination != null)
        {
            npcAgent.SetDestination(destination.position);
        }
    }
}
