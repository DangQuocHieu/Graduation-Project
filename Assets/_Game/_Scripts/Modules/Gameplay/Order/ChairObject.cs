using UnityEngine;

public class ChairObject : MonoBehaviour
{
    public bool isEmpty = true;
    public Transform sittingPoint;
    public Vector3 sittingRotation;
    public Transform dishPlacePoint;
    public Transform leavePoint;
    public PlaceableSurface attachedTableSurface;

    private void OnDrawGizmos()
    {
        // Kiểm tra nếu dishPlacePoint không bị null để tránh lỗi Reference
        if (dishPlacePoint != null)
        {
            // Thiết lập màu cho Gizmos (màu xanh lá chẳng hạn)
            Gizmos.color = Color.green;

            // Vẽ khối lập phương tại vị trí của dishPlacePoint
            // Tham số thứ 2 là kích thước khối (ví dụ 0.2f cho mỗi chiều)
            Gizmos.DrawWireCube(dishPlacePoint.position, new Vector3(0.2f, 0.2f, 0.2f));

            // Nếu bạn muốn vẽ khối đặc thì dùng:
            // Gizmos.DrawCube(dishPlacePoint.position, new Vector3(0.2f, 0.2f, 0.2f));
        }
    }
}
