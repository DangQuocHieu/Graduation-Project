using UnityEngine;

public class ObjectHover : MonoBehaviour
{
    public Outline objectOutline;
    public GrabbableObject attachedObject;

    void Awake()
    {
        SetUpOutline();
    }

    private void SetUpOutline()
    {
        if (objectOutline != null)
        {
            objectOutline.OutlineMode = Outline.Mode.OutlineAll;
            objectOutline.OutlineColor = Color.yellow;
            objectOutline.OutlineWidth = 5f;
            
            // Tắt component mặc định để tiết kiệm hiệu năng render
            objectOutline.enabled = false; 
        }
    }

    public void OnHoverEnter()
    {
        if (objectOutline != null)
        {
            objectOutline.enabled = true; // Bật script Outline lên

        }
        else
        {
            // Báo vàng ở Console để bạn dễ phát hiện lỗi quên gán script
            Debug.LogWarning($"[ObjectHover] Object '{gameObject.name}' đang bị thiếu Reference ở biến ObjectOutline!", gameObject);
        }
    }

    public void OnHoverExit()
    {
        if (objectOutline != null)
        {
            objectOutline.enabled = false; // Tắt script Outline đi
        }
    }
}