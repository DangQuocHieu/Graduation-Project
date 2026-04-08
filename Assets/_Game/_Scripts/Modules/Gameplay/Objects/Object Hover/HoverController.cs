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
            if(hit.collider.attachedRigidbody == null)
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

        // Xử lý logic nếu tìm thấy ít nhất 1 object hợp lệ
        if (closestObjectHover != null)
        {
            // Nếu vẫn đang nhìn vào object hiện tại thì không làm gì cả
            if (currentHoveredObject == closestObjectHover)
            {
                return;
            }

            if (closestObjectHover.attachedObject != null) 
            {
                objectHoverPanel.SetUpUI(closestObjectHover.attachedObject.grabbableObjectSO);
            }

            currentHoveredObject?.OnHoverExit();
            currentHoveredObject = closestObjectHover;
            currentHoveredObject?.OnHoverEnter();
            objectHoverPanel.ShowPanel(closestObjectHover.transform.position); 
        }
        else
        {
            // Nếu không quét trúng vật nào hoặc không có vật nào có component ObjectHover
            if (currentHoveredObject != null)
            {
                currentHoveredObject.OnHoverExit();
                currentHoveredObject = null;
            }
            
            // Luôn ẩn panel khi không nhìn vào object nào
            objectHoverPanel.HidePanel();
        }
    }
}