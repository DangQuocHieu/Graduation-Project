using Unity.Jobs.LowLevel.Unsafe;
using UnityEngine;

public class PickupAndDropHandler : MonoBehaviour
{
    [SerializeField] private float _pickUpRange = 3f;
    [SerializeField] private Transform _camera;
    [SerializeField] private Transform _grabObjectPoint;
    [SerializeField] private Collider _collider;
    [SerializeField] private float _autoDropDistance = 8f;
    private GrabbaleObject _objectInHand;
    void Update()
    {
        HandlePickUpAndDropObject();
        HandleAutoDropObject();
    }

    private void HandlePickUpAndDropObject()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (_objectInHand == null)
            {
                if (Physics.Raycast(_camera.position, _camera.forward, out RaycastHit hit, _pickUpRange))
                {
                    if (hit.collider.TryGetComponent(out GrabbaleObject interactableObject))
                    {
                        _objectInHand = interactableObject;
                        _objectInHand.OnPickUp(_grabObjectPoint, _collider);
                    }
                }
            }
            else
            {
                if (Physics.Raycast(_camera.position, _camera.forward, out RaycastHit hit, _pickUpRange))
                {
                    if (hit.collider.gameObject == _objectInHand.gameObject)
                    {
                        return;
                    }
                    else
                    {
                        _objectInHand.InteractWith(hit, this);
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
