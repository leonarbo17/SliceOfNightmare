using UnityEngine;

public class HighlihgtTarget : MonoBehaviour
{
    [HideInInspector] public int originalLayer;
    void Awake()
    {
        originalLayer = gameObject.layer;
    }
}
