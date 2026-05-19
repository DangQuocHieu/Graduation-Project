using Unity.VisualScripting;
using UnityEngine;

public class HoverController : MonoBehaviour
{
    public Camera mainCam;
    public float hoverDistance;
    public LayerMask hoverableLayer;
    public ObjectHoverPanel objectHoverPanel;
    public ObjectHover currentHoveredObject;
    public GrabbableObject currentGrabbableObject;

    void Update()
    {
        HandleHover();
    }

    private void HandleHover()
    {
        // Tạo tia ray từ trung tâm Camera (Viewport 0.5, 0.5)
        Ray ray = mainCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, hoverDistance, hoverableLayer))
        {
            ObjectHover objectHover = null;

            if (hit.collider.attachedRigidbody == null)
            {
                objectHover = hit.collider.GetComponent<ObjectHover>();
            }
            else
            {
                objectHover = hit.collider.attachedRigidbody.GetComponent<ObjectHover>();
            }

            if (objectHover != null)
            {
                if (currentHoveredObject == objectHover)
                {
                    return;
                }

                ResetCurrentHover();

                currentHoveredObject = objectHover;
                currentGrabbableObject = objectHover.attachedGrabbableObject;
                currentHoveredObject.OnHoverEnter();

                objectHoverPanel.ShowPanel(currentHoveredObject.transform.position);

                if (currentHoveredObject.grabbableObjectSO != null)
                {
                    objectHoverPanel.SetUpUI(currentHoveredObject.grabbableObjectSO);
                }
                else if (currentHoveredObject.TryGetComponent<ShopItem>(out var shopItem))
                {
                    if (shopItem.shopItemSO != null)
                        objectHoverPanel.SetUpUI(shopItem.shopItemSO.grabbableObjectSO, displayPrice: true);
                }
                else
                {
                    objectHoverPanel.HidePanel();
                }
            }
            else
            {

                ClearAllHoverState();
            }
        }
        else
        {

            ClearAllHoverState();
        }
    }

    private void ResetCurrentHover()
    {
        if (currentHoveredObject != null)
        {
            currentHoveredObject.OnHoverExit();
            currentGrabbableObject = null;
        }
    }

    private void ClearAllHoverState()
    {
        if (currentHoveredObject != null)
        {
            currentHoveredObject.OnHoverExit();
            currentHoveredObject = null;
            currentGrabbableObject = null;
        }
        objectHoverPanel.HidePanel();
    }
}