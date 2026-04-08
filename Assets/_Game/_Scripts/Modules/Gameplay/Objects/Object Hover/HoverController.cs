using Unity.VisualScripting;
using UnityEngine;

public class HoverController : MonoBehaviour
{
    public Camera mainCam;
    public float hoverDistance;
    public LayerMask hoverableLayer;
    public ObjectHoverPanel objectHoverPanel;
    private GrabbableObject currentHoveredObject;

    void Update()
    {
        HandleHover();
    }

    private void HandleHover()
    {
        Ray ray = mainCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if (Physics.Raycast(ray, out RaycastHit hit, hoverDistance, hoverableLayer))
        {
            var grabbableObject = hit.collider.GetComponentInParent<GrabbableObject>();
            if (grabbableObject != null)
            {
                if(currentHoveredObject == grabbableObject)
                {
                    return;
                }

                objectHoverPanel.SetUpUI(grabbableObject.grabbableObjectSO);
                currentHoveredObject?.OnHoverExit();
                currentHoveredObject = grabbableObject;
                currentHoveredObject?.OnHoverEnter();
                objectHoverPanel.ShowPanel(grabbableObject.transform.position);
            }
            else
            {
                currentHoveredObject?.OnHoverExit();
                currentHoveredObject = null;
                objectHoverPanel.HidePanel();
            }
        }
        else
        {
            currentHoveredObject?.OnHoverExit();
            currentHoveredObject = null;
            objectHoverPanel.HidePanel();
        }
    }
}
