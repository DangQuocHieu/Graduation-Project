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
        // Tạo tia ray từ trung tâm Camera (Viewport 0.5, 0.5)
        Ray ray = mainCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;

        // Physics.Raycast sẽ chỉ trả về vật thể ĐẦU TIÊN bị bắn trúng
        if (Physics.Raycast(ray, out hit, hoverDistance, hoverableLayer))
        {
            ObjectHover objectHover = null;

            // Kiểm tra script ObjectHover trên vật thể hoặc Rigidbody của nó
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
                // Nếu vẫn đang nhìn vào vật cũ thì không cần chạy lại logic bên dưới
                if (currentHoveredObject == objectHover)
                {
                    return;
                }

                // Thoát hover vật cũ (nếu có)
                ResetCurrentHover();

                // Gán vật mới và kích hoạt OnHoverEnter
                currentHoveredObject = objectHover;
                currentHoveredObject.OnHoverEnter();

                // Hiển thị và cập nhật UI Panel
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
                // Bắn trúng vật thuộc layer nhưng không có component ObjectHover
                ClearAllHoverState();
            }
        }
        else
        {
            // Không bắn trúng bất cứ thứ gì
            ClearAllHoverState();
        }
    }

    // Hàm hỗ trợ reset trạng thái hover cũ
    private void ResetCurrentHover()
    {
        if (currentHoveredObject != null)
        {
            currentHoveredObject.OnHoverExit();
        }
    }

    // Hàm dọn dẹp sạch sẽ khi không hover vào đâu
    private void ClearAllHoverState()
    {
        if (currentHoveredObject != null)
        {
            currentHoveredObject.OnHoverExit();
            currentHoveredObject = null;
        }
        objectHoverPanel.HidePanel();
    }
}