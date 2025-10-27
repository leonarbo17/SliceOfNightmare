using UnityEngine;
using UnityEngine.AI;

public class NpcLousine : MonoBehaviour
{
    [Header("Puntos de movimiento")]
    public Transform puntoA;
    public Transform puntoB;

    [Header("Referencias")]
    public GameObject npcVisual;
    public Animator animator;
    public NavMeshAgent agente;
    public Transform jugador;

    [Header("Diálogos")]
    public Dialogue dialogueSinPizza;
    public Dialogue dialogueConPizza;
    public DialogueManager dialogueManager;

    [Header("Configuración")]
    public float distanciaInteraccion = 3f;
    public bool jugadorTienePizza = false;

    private bool caminando = false;
    private bool llegoADestino = false;
    private bool interactuado = false;
    private bool volviendo = false;

    void Start()
    {
        if (agente == null) agente = GetComponent<NavMeshAgent>();
        if (jugador == null) jugador = GameObject.FindGameObjectWithTag("Player").transform;

        agente.isStopped = true;
        animator.SetBool("Walk", false);
    }

    void Update()
    {
        if (caminando && !agente.pathPending && agente.remainingDistance <= agente.stoppingDistance)
        {
            agente.isStopped = true;
            animator.SetBool("Walk", false);
            caminando = false;

            if (!volviendo)
                llegoADestino = true;
            else
                DesaparecerNPC();
        }

        if (llegoADestino && !interactuado)
        {
            float distancia = Vector3.Distance(jugador.position, transform.position);
            if (distancia <= distanciaInteraccion && Input.GetKeyDown(KeyCode.E))
            {
                Interactuar();
            }
        }
    }

    /// <summary>
    /// Este método se llama desde un trigger externo con la tag correcta.
    /// </summary>
    public void ActivarMovimientoDesdeTrigger(string tagDelTrigger)
    {
        if (tagDelTrigger == "TriggerLousine" && !caminando && !llegoADestino)
        {
            IrAPuntoB();
        }
    }

    void IrAPuntoB()
    {
        agente.isStopped = false;
        agente.SetDestination(puntoB.position);
        caminando = true;
        animator.SetBool("Walk", true);
    }

    void Interactuar()
    {
        interactuado = true;
        agente.isStopped = true;
        animator.SetBool("Walk", false);
        animator.SetTrigger("Talk");

        if (jugadorTienePizza)
        {
            if (dialogueManager != null && dialogueConPizza != null)
                dialogueManager.StartDialogue(dialogueConPizza);
            else
                Debug.Log("🍕 Lousine: 'Gracias por la pizza.'");

            StartCoroutine(EsperarFinDialogo(true));
        }
        else
        {
            if (dialogueManager != null && dialogueSinPizza != null)
                dialogueManager.StartDialogue(dialogueSinPizza);
            else
                Debug.Log("😕 Lousine: '¿Y mi pizza?'");

            StartCoroutine(EsperarFinDialogo(false));
        }
    }

    System.Collections.IEnumerator EsperarFinDialogo(bool teniaPizza)
    {
        yield return new WaitUntil(() => dialogueManager == null || !dialogueManager.dialogueActive);

        if (teniaPizza)
            TerminarDialogoConPizza();
        else
            interactuado = false;
    }

    void TerminarDialogoConPizza()
    {
        volviendo = true;
        caminando = true;
        animator.SetBool("Walk", true);
        agente.isStopped = false;
        agente.SetDestination(puntoA.position);
        Debug.Log("👣 Lousine regresa al punto A...");
    }

    void DesaparecerNPC()
    {
        Debug.Log("💨 Lousine desaparece.");
        if (npcVisual != null)
            npcVisual.SetActive(false);
        else
            gameObject.SetActive(false);
    }
}
