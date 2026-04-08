using System;
using DQHieu.Framework;
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

    void Update()
    {
        HandlePickUpAndDropObject();
        HandleAutoDropObject();
    }

    void OnEnable()
    {
        EventBus.Subcribe<PurchaseShopItemSucess>(HandlePurchaseShopItemSuccessEvent);
    }

    void OnDisable()
    {
        EventBus.UnSubcribe<PurchaseShopItemSucess>(HandlePurchaseShopItemSuccessEvent);
    }

    public void PickupObject(GrabbableObject grabbableObject)
    {
        _objectInHand = grabbableObject;
        _objectInHand.OnPickUp(_grabObjectPoint);
    }
    
    private void HandlePickUpAndDropObject()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (_objectInHand == null)
            {
                if (Physics.Raycast(_camera.position, _camera.forward, out RaycastHit hit, _pickUpRange))
                {
                    if (hit.collider.attachedRigidbody!=null && hit.collider.attachedRigidbody.TryGetComponent<GrabbableObject>(out var grabbableObject))
                    {
                        _objectInHand = grabbableObject;
                        _objectInHand.OnPickUp(_grabObjectPoint);
                    }
                    else if (hit.collider.TryGetComponent<StoveSwitch>(out var stoveSwitch))
                    {
                        stoveSwitch.OnInteract();
                    }
                    else if(hit.collider.TryGetComponent<HingedObject>(out var hingedObject))
                    {
                        hingedObject.Toggle();
                    }
                    else if(hit.collider.TryGetComponent<ShopItem>(out var shopItem))
                    {
                        EventBus.SendMessage<InteractWithShopItemEvent>(new InteractWithShopItemEvent(shopItem));
                    }
                }
            }
            else
            {
                RaycastHit[] hits = Physics.RaycastAll(_camera.position, _camera.forward, _pickUpRange);
                System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

                foreach (RaycastHit hit in hits)
                {
                    if (hit.collider.gameObject == _objectInHand.gameObject)
                    {
                        continue;
                    }
                    else
                    {
                        _objectInHand.InteractWith(hit, this);
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

    public void HandlePurchaseShopItemSuccessEvent(PurchaseShopItemSucess evt)
    {
        _objectInHand = evt.purchasedObject;
        evt.purchasedObject.OnPickUp(_grabObjectPoint);
    }
}