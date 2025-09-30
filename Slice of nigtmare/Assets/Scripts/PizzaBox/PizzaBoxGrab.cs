using UnityEngine;
using System.Collections;

public class PizzaBoxGrab : MonoBehaviour
{
    [Header("Movimiento de la caja")]
    public float moveTime = 1f;
    public float moveX = 1f;

    [Header("Sonido")]
    public AudioClip grabSound;
    private AudioSource audioSource;

    [Header("Prefab jugador")]
    public GameObject playerPrefabHolder;

    [Header("Interacción")]
    public Camera playerCamera;
    public float interactDistance = 3f;
    public string pizzaBoxTag = "PizzaBox";

    private bool isMoving = false;

    // ? Variable pública para que otros scripts la lean
    public static bool HasPizzaInHand = false;
    private Collider boxCollider;

    void Start()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;

        audioSource = gameObject.AddComponent<AudioSource>();
        boxCollider = GetComponent<Collider>();
    }

    void Update()
    {
        if (HasPizzaInHand)
            return;

        if (Input.GetKeyDown(KeyCode.E) && !isMoving)
        {
            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, interactDistance))
            {
                if (hit.transform.CompareTag(pizzaBoxTag) && hit.transform == transform)
                {
                    StartCoroutine(MoveBox());
                }
            }
        }
    }

    IEnumerator MoveBox()
    {
        isMoving = true;
        HasPizzaInHand = true; // Bloquea agarrar otra pizza

        if (boxCollider != null)
            boxCollider.enabled = false;

        if (grabSound != null)
            audioSource.PlayOneShot(grabSound);

        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + new Vector3(moveX, 0f, 0f);

        float elapsed = 0f;

        while (elapsed < moveTime)
        {
            transform.position = Vector3.Lerp(startPos, endPos, elapsed / moveTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = endPos;

        // Desactivar la caja original
        gameObject.SetActive(false);

        // Activar la pizza (prefab)
        if (playerPrefabHolder != null)
        {
            playerPrefabHolder.SetActive(true);
        }

        isMoving = false;
    }

    // Método para "soltar" la pizza y poder agarrar otra
    public static void RemovePizzaFromHand(GameObject pizzaPrefab)
    {
        if (pizzaPrefab != null)
            pizzaPrefab.SetActive(false);

        HasPizzaInHand = false;
    }
}
