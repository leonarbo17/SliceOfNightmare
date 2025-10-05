using UnityEngine;
using UnityEngine.AI;

public class RouteNPC1 : MonoBehaviour
{
    [SerializeField] Transform destination;
    [SerializeField] AudioSource footstepAudio; // arrastra aquí el AudioSource en el Inspector

    private NavMeshAgent navMeshAgent;
    private Animator animator;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        if (navMeshAgent == null)
        {
            Debug.LogError("Nav Mesh Agent component not attached");
        }
        else
        {
            SetDestination();
        }

        if (footstepAudio != null)
        {
            footstepAudio.loop = true; // el sonido se repetirá
            footstepAudio.playOnAwake = false;
        }
    }

    void Update()
    {
        if (navMeshAgent != null && animator != null)
        {
            float speed = navMeshAgent.velocity.magnitude;
            animator.SetFloat("Speed", speed);

            HandleFootsteps(speed);
        }
    }

    private void SetDestination()
    {
        if (destination != null)
        {
            Vector3 targetVector = destination.position;
            navMeshAgent.SetDestination(targetVector);
        }
    }

    private void HandleFootsteps(float speed)
    {
        if (footstepAudio == null) return;

        if (speed > 0.1f && !footstepAudio.isPlaying)
        {
            footstepAudio.Play(); // empieza a sonar al caminar
        }
        else if (speed <= 0.1f && footstepAudio.isPlaying)
        {
            footstepAudio.Stop(); // se detiene cuando está quieto
        }
    }
}
