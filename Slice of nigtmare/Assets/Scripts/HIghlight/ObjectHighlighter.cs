using UnityEngine;

public class ObjectHighlighter : MonoBehaviour
{
    [SerializeField] private float raycastDistance = 3f;
    [SerializeField] private string outlineLayerName = "Outline";

    private int outlineLayer;
    private GameObject lasthiglightedObject;
    void Start()
    {
        outlineLayer = LayerMask.NameToLayer(outlineLayerName);
    }
    void Update()
    {
        highlightRaycastCheck();
    }
    void highlightRaycastCheck()
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector2(Screen.width * 0.5f, Screen.height * 0.5f));

        if(Physics.Raycast(ray, out RaycastHit Hit, raycastDistance))
        {
            if(Hit.collider.TryGetComponent(out HighlihgtTarget target))
            {
                GameObject targetObject = target.gameObject;
                if(lasthiglightedObject  != targetObject)
                {
                    clearHighlight();
                    targetObject.layer = outlineLayer;
                    lasthiglightedObject = targetObject;

                }
            }   return;
        }
        clearHighlight();
    }   
   
    void clearHighlight()
    {
        if (lasthiglightedObject != null)
        {
            if (lasthiglightedObject.TryGetComponent(out HighlihgtTarget target))
            {
                lasthiglightedObject.layer = target.originalLayer;
            }
        }   lasthiglightedObject= null;
    }
}