using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class ZoomSatanicSimbol : MonoBehaviour
{
    [Header("Cámara")]
    public Camera playerCamera;
    public Transform ritualTarget;
    public float focusDuration = 2f;
    public float zoomFOV = 40f;
    private float originalFOV;

    [Header("Jugador")]
    public playterMove playerMove;

    [Header("Diálogo")]
    [TextArea(2, 4)] public string dialogueLine;
    [TextArea(2, 4)] public string[] replyLines;

    [Header("NPC Luis")]
    public NavMeshAgent luisAgent;
    public Transform luisDestinationA;
    public Animator luisAnimator;
    public AudioSource luisFootstepAudio;

    [Header("Opciones pasos")]
    public float walkSoundSpeedThreshold = 0.1f;

    private bool triggered = false;

    private void Start()
    {
        if (luisFootstepAudio != null)
        {
            luisFootstepAudio.loop = true;
            luisFootstepAudio.playOnAwake = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;

        if (other.CompareTag("Player"))
        {
            triggered = true;

            if (playerMove == null)
                playerMove = other.GetComponent<playterMove>();

            if (luisAgent != null && luisDestinationA != null)
            {
                luisAgent.isStopped = false;
                luisAgent.SetDestination(luisDestinationA.position);
                StartCoroutine(WatchLuisArrival());
            }

            if (playerCamera != null && ritualTarget != null)
            {
                StartCoroutine(FocusOnRitual());
            }
        }
    }

    private void Update()
    {
        if (luisAgent != null && luisAnimator != null)
        {
            float speed = luisAgent.velocity.magnitude;
            luisAnimator.SetFloat("Speed", speed);

            if (luisFootstepAudio != null)
            {
                if (speed > walkSoundSpeedThreshold)
                {
                    if (!luisFootstepAudio.isPlaying)
                        luisFootstepAudio.Play();
                }
                else
                {
                    if (luisFootstepAudio.isPlaying)
                        luisFootstepAudio.Stop();
                }
            }
        }
    }

    private IEnumerator WatchLuisArrival()
    {
        while (luisAgent.pathPending)
            yield return null;

        while (true)
        {
            if (!luisAgent.pathPending &&
                luisAgent.remainingDistance <= luisAgent.stoppingDistance)
            {
                luisAgent.isStopped = true;
                luisAgent.velocity = Vector3.zero;

                if (luisAnimator != null)
                    luisAnimator.SetFloat("Speed", 0f);

                if (luisFootstepAudio != null && luisFootstepAudio.isPlaying)
                    luisFootstepAudio.Stop();

                yield break;
            }

            yield return null;
        }
    }

    private IEnumerator FocusOnRitual()
    {
        if (playerMove != null)
            playerMove.SetPlayerControl(false);

        Quaternion originalRot = playerCamera.transform.rotation;
        originalFOV = playerCamera.fieldOfView;

        Vector3 dirToTarget = (ritualTarget.position - playerCamera.transform.position).normalized;
        Quaternion targetRot = Quaternion.LookRotation(dirToTarget, Vector3.up);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / focusDuration;
            playerCamera.transform.rotation = Quaternion.Slerp(originalRot, targetRot, t);
            playerCamera.fieldOfView = Mathf.Lerp(originalFOV, zoomFOV, t);
            yield return null;
        }

        if (DialogueManager.Instance != null && !string.IsNullOrEmpty(dialogueLine))
        {
            DialogueManager.Instance.ShowDialogue(dialogueLine, replyLines, false);
        }

        while (DialogueManager.Instance != null &&
               DialogueManager.Instance.IsDialoguePanelOpen())
        {
            yield return null;
        }

        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / focusDuration;
            playerCamera.transform.rotation = Quaternion.Slerp(targetRot, originalRot, t);
            playerCamera.fieldOfView = Mathf.Lerp(zoomFOV, originalFOV, t);
            yield return null;
        }

        if (playerMove != null)
            playerMove.SetPlayerControl(true);

        gameObject.SetActive(false);
    }
}
