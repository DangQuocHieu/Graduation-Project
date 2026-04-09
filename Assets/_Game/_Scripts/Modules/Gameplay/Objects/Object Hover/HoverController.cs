using Unity.VisualScripting;
using UnityEngine;

public class HoverController : MonoBehaviour
{
    public Camera mainCam;
    public float hoverDistance;
    public LayerMask hoverableLayer;
    public ObjectHoverPanel objectHoverPanel;
    public ObjectHover currentHoveredObject;

    void Update()
    {
        HandleHover();
    }

    private void HandleHover()
    {
        Ray ray = mainCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit[] hits = Physics.RaycastAll(ray, hoverDistance, hoverableLayer);

        ObjectHover closestObjectHover = null;
        float minDistance = float.MaxValue;

        foreach (RaycastHit hit in hits)
        {
            Debug.Log(hit.collider.name);
            ObjectHover objectHover = null;
            if (hit.collider.attachedRigidbody == null)
            {
                objectHover = hit.collider.GetComponent<ObjectHover>();
            }
            else
            {
                objectHover = hit.collider.attachedRigidbody.GetComponent<ObjectHover>();
            }

            if (objectHover != null && hit.distance < minDistance)
            {
                minDistance = hit.distance;
                closestObjectHover = objectHover;
            }
        }

        if (closestObjectHover != null)
        {
            if (currentHoveredObject == closestObjectHover)
            {
                return;
            }

            currentHoveredObject?.OnHoverExit();
            currentHoveredObject = closestObjectHover;
            currentHoveredObject?.OnHoverEnter();
            objectHoverPanel.ShowPanel(closestObjectHover.transform.position);

            if (closestObjectHover.attachedObject != null)
            {
                objectHoverPanel.SetUpUI(closestObjectHover.attachedObject.grabbableObjectSO);
            }
            else if (closestObjectHover.TryGetComponent<ShopItem>(out var shopItem))
            {
                if(shopItem.shopItemSO != null)
                objectHoverPanel.SetUpUI(shopItem.shopItemSO.grabbableObjectSO, displayPrice: true);
            }
            else
            {
                objectHoverPanel.HidePanel();
            }
        }
        else
        {
            if (currentHoveredObject != null)
            {
                currentHoveredObject.OnHoverExit();
                currentHoveredObject = null;
            }

            objectHoverPanel.HidePanel();
        }
    }
}