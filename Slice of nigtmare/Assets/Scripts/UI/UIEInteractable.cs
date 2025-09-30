using System.Collections.Generic;
using UnityEngine;

public class LookAtShowImageMultiple : MonoBehaviour
{
    public Camera playerCamera;
    public List<GameObject> prefabInstances; // Lista de todas las instancias del prefab
    public float maxDistance = 5f;

    void Update()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxDistance))
        {
            // Verifica si el objeto golpeado está en nuestra lista de instancias
            if (prefabInstances.Contains(hit.transform.gameObject))
            {
                // Aquí podrías agregar otra acción, por ejemplo animación o interacción
                // Por ahora solo detecta si estás mirando al objeto
            }
        }
    }
}
