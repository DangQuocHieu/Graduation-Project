using UnityEngine;

public class PickupAndDropHandler : MonoBehaviour
{
    [SerializeField] private float _pickUpRange = 3f;
    [SerializeField] private Transform _camera;
    [SerializeField] private Transform _grabObjectPoint;
    [SerializeField] private Collider _collider;
    private InteractableObject _objectInHand;
    void Update()
    {
        HandlePickUpAndDropObject();
    }

    private void HandlePickUpAndDropObject()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if(_objectInHand == null)
            {
                if(Physics.Raycast(_camera.position, _camera.forward, out RaycastHit hit, _pickUpRange))
                {
                    if(hit.collider.TryGetComponent(out InteractableObject interactableObject))
                    {
                        _objectInHand = interactableObject;
                        _objectInHand.OnPickUp(_grabObjectPoint, _collider);
                    }
                }
            }
            else
            {
                
            }
        }
    }

    private void DropObject()
    {
        _objectInHand = null;
    }
}
