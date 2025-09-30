using UnityEngine;

public class RaycastWarmup : MonoBehaviour
{
    [Header("Configuración")]
    public float distance = 1f; // Distancia corta, solo para inicializar

    void Start()
    {
        WarmupRaycast();
    }

    void WarmupRaycast()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        // Lanzamos un raycast corto hacia delante
        Physics.Raycast(ray, out hit, distance);

        Debug.Log("Raycast warmup ejecutado"); // Opcional, solo para verificar
    }
}
