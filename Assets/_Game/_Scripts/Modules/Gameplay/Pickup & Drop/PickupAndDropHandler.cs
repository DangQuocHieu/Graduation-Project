using System.Collections;
using DQHieu.Framework;
using UnityEngine;
using CoreGame.Movement;
using UnityEngine.EventSystems;
using Sirenix.OdinInspector;

public class PickupAndDropHandler : MonoBehaviour
{
    [SerializeField] private float _pickUpRange = 3f;
    [SerializeField] private Transform _camera;
    [SerializeField] private Transform _grabObjectPoint;
    [SerializeField] private Collider _collider;
    [SerializeField] private float _autoDropDistance = 8f;
    public Material validGhostMaterial;
    public Material invalidGhostMaterial;
    public GrabbableObject _objectInHand;
    private Coroutine waitForPurchasedObjectPickedUpCoroutine;
    private float _currentCrouchOffset = 0f;
    private GhostObjectHandler _ghostObjectHandler = new GhostObjectHandler();

    [ShowInInspector] private bool _isPickupBlocked = false;
    [ShowInInspector] private bool _isDropBlocked = false;

    public bool IsPickupBlocked => _isPickupBlocked;
    public bool IsDropBlocked => _isDropBlocked;

    public void SetBlockPickup(bool block)
    {
        _isPickupBlocked = block;
    }

    public void SetBlockDrop(bool block)
    {
        _isDropBlocked = block;
    }

    void Update()
    {
        HandlePickUpAndDropObject();
        HandleAutoDropObject();
        HandleInteractObject();
        _ghostObjectHandler.UpdateGhostPosition(_objectInHand, _camera, _pickUpRange);
    }

    void OnEnable()
    {
        EventBus.Subcribe<PurchaseShopItemSucess>(HandlePurchaseShopItemSuccessEvent);
        EventBus.Subcribe<PlayerCrouchEvent>(HandlePlayerCrouchEvent);
    }

    void OnDisable()
    {
        EventBus.UnSubcribe<PurchaseShopItemSucess>(HandlePurchaseShopItemSuccessEvent);
        EventBus.UnSubcribe<PlayerCrouchEvent>(HandlePlayerCrouchEvent);
    }

    private void HandlePlayerCrouchEvent(PlayerCrouchEvent evt)
    {
        _currentCrouchOffset = evt.IsCrouching ? evt.CrouchOffset : 0f;
        if (_objectInHand != null)
        {
            _objectInHand.SetCrouchOffset(_currentCrouchOffset);
        }
    }

    public void PickupObject(GrabbableObject grabbableObject, bool ignoreBlock = false)
    {
        if (!ignoreBlock && _isPickupBlocked) return;
        _objectInHand = grabbableObject;
        _objectInHand.OnPickUp(_grabObjectPoint);
        _objectInHand.SetCrouchOffset(_currentCrouchOffset);
        _ghostObjectHandler.CreateGhostObject(grabbableObject, validGhostMaterial, invalidGhostMaterial);
    }

    private void HandleInteractObject()
    {
        if (Input.GetMouseButtonDown(0))
        {

            if (Physics.Raycast(_camera.position, _camera.forward, out RaycastHit hit, _pickUpRange))
            {
                if (hit.collider.TryGetComponent<IInteractable>(out var interactable))
                {
                    interactable.OnInteract();
                }
            }


        }
    }
    private void HandlePickUpAndDropObject()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

            if (_objectInHand == null)
            {
                if (_isPickupBlocked) return;

                if (Physics.Raycast(_camera.position, _camera.forward, out RaycastHit hit, _pickUpRange))
                {
                    if (hit.collider.TryGetComponent<IngredientAnchor>(out var ingredientAnchor))
                    {
                        ingredientAnchor.OnInteract(this);
                    }
                    else if (hit.collider.attachedRigidbody != null && hit.collider.attachedRigidbody.TryGetComponent<GrabbableObject>(out var grabbableObject))
                    {
                        if (grabbableObject.canBePickedUp)
                        {
                            // _objectInHand = grabbableObject;
                            // _objectInHand.OnPickUp(_grabObjectPoint);
                            // _objectInHand.SetCrouchOffset(_currentCrouchOffset);
                            PickupObject(grabbableObject);
                        }
                    }
                    else if (hit.collider.TryGetComponent<ShopItem>(out var shopItem))
                    {
                        EventBus.Raise<InteractWithShopItemEvent>(new InteractWithShopItemEvent(shopItem));
                    }
                    else if(hit.collider.TryGetComponent<GrabbableObjectSpawner>(out var grabbableObjectSpawner))
                    {
                        grabbableObjectSpawner.OnInteract(this);
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
                        if (hit.collider.TryGetComponent<KitchenArea>(out var kitchenArea))
                        {
                            if (_isDropBlocked)
                            {
                                return; // Block manual drop on KitchenArea
                            }

                            if (!_ghostObjectHandler.IsPlacementValid)
                            {
                                return; // Block drop
                            }
                        }
                        
                        _objectInHand.InteractWith(hit, this);
                        return;
                    }
                }
            }
        }
    }

    public void DropObject(bool ignoreBlock = false)
    {
        if (_objectInHand == null) return;
        _objectInHand.OnDrop();
        _objectInHand = null;
        _ghostObjectHandler.DestroyGhostObject();
    }

    private void HandleAutoDropObject()
    {
        if (_objectInHand != null)
        {
            float distanceToCamera = Vector3.Distance(_objectInHand.transform.position, _camera.position);
            if (distanceToCamera >= _autoDropDistance)
            {
                DropObject(true);
            }
        }
    }

    public void HandlePurchaseShopItemSuccessEvent(PurchaseShopItemSucess evt)
    {
        StopAllCoroutines();
        if (_objectInHand == null)
        {
            if (_isPickupBlocked) return;
            _objectInHand = evt.purchasedObject;
            evt.purchasedObject.OnPickUp(_grabObjectPoint);
            _objectInHand.SetCrouchOffset(_currentCrouchOffset);
            _ghostObjectHandler.CreateGhostObject(evt.purchasedObject, validGhostMaterial, invalidGhostMaterial);
            waitForPurchasedObjectPickedUpCoroutine = StartCoroutine(WaitForPurchasedObjectPickedUpByHand(evt.purchasedObject));
        }
        else
        {
            if (_objectInHand is BambooTray bambooTray)
            {
                if (evt.purchasedObject is Ingredient ingredient)
                {
                    ingredient.HandleInteractWithBambooTray(bambooTray);
                    waitForPurchasedObjectPickedUpCoroutine = StartCoroutine(WaitForPurchasedObjectPickedUpByTray(evt.purchasedObject));

                }
            }
        }
    }

    private IEnumerator WaitForPurchasedObjectPickedUpByHand(GrabbableObject purchasedObject)
    {
        yield return new WaitUntil(() => purchasedObject.isPickupCompleted);
        EventBus.Raise<ItemPickedUpComplete>(new ItemPickedUpComplete());
    }

    private IEnumerator WaitForPurchasedObjectPickedUpByTray(GrabbableObject purchasedObject)
    {
        yield return new WaitUntil(() => purchasedObject.isMoveToSurfaceCompleted);
        EventBus.Raise<ItemPickedUpComplete>(new ItemPickedUpComplete());
    }
}
