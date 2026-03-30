using System;
using Unity.Jobs.LowLevel.Unsafe;
using UnityEngine;

public class PickupAndDropHandler : MonoBehaviour
{
    [SerializeField] private float _pickUpRange = 3f;
    [SerializeField] private Transform _camera;
    [SerializeField] private Transform _grabObjectPoint;
    [SerializeField] private Collider _collider;
    [SerializeField] private float _autoDropDistance = 8f;
    private GrabbableObject _objectInHand;
    public LayerMask liquidLayer;

    void Update()
    {
        HandlePickUpAndDropObject();
        HandleAutoDropObject();
    }

    public void PickupObject(GrabbableObject grabbableObject)
    {
        _objectInHand = grabbableObject;
        _objectInHand.OnPickUp(_grabObjectPoint, _collider);
    }
    
    private void HandlePickUpAndDropObject()
    {
        if (Input.GetMouseButtonDown(0))
        {
            int mask = ~liquidLayer;
            if (_objectInHand == null)
            {
                if (Physics.Raycast(_camera.position, _camera.forward, out RaycastHit hit, _pickUpRange, mask))
                {
                    if (hit.collider.GetComponentInParent<GrabbableObject>() != null)
                    {
                        var interactableObject = hit.collider.GetComponentInParent<GrabbableObject>();
                        _objectInHand = interactableObject;
                        _objectInHand.OnPickUp(_grabObjectPoint, _collider);
                    }
                    else if (hit.collider.TryGetComponent<StoveSwitch>(out var stoveSwitch))
                    {
                        stoveSwitch.OnInteract();
                    }
                }
            }
            else
            {
                // Thay vì Raycast thường, dùng RaycastAll để xuyên qua vật đang cầm
                RaycastHit[] hits = Physics.RaycastAll(_camera.position, _camera.forward, _pickUpRange, mask);

                // Sắp xếp lại danh sách hit theo khoảng cách từ camera (từ gần đến xa)
                System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

                foreach (RaycastHit hit in hits)
                {
                    if (hit.collider.gameObject == _objectInHand.gameObject)
                    {
                        // Nếu trúng chính vật đang cầm thì bỏ qua, xét tiếp vật phía sau
                        continue;
                    }
                    else
                    {
                        // Tương tác với vật đầu tiên trúng được (mà không phải vật đang cầm)
                        _objectInHand.InteractWith(hit, this);
                        
                        // Đã tìm thấy và tương tác xong thì thoát khỏi vòng lặp/hàm
                        return; 
                    }
                }
            }
        }
    }

    public void DropObject()
    {
        _objectInHand.OnDrop();
        _objectInHand = null;
    }

    private void HandleAutoDropObject()
    {
        if (_objectInHand != null)
        {
            float distanceToCamera = Vector3.Distance(_objectInHand.transform.position, _camera.position);
            if (distanceToCamera >= _autoDropDistance)
            {
                DropObject();
            }
        }
    }
}