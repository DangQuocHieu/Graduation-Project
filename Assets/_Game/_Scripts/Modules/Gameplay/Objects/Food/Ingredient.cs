using System.Collections;
using UnityEngine;

public enum IngredientType
{
    Tofu,
    RiceNoodle,
    Cucumber,
    Herb
}
public class Ingredient : GrabbableObject
{
    public float weight;
    public IngredientAnchor attachedIngredientAnchor;
    public IngredientType ingredientType;
    public SliceableObject sliceableObject;
    public CookableObject cookableObject;

    public bool canSlice => sliceableObject != null;

    protected override void Awake()
    {
        base.Awake();
        sliceableObject = GetComponent<SliceableObject>();
        cookableObject = GetComponent<CookableObject>();
    }

    public override void InteractWith(RaycastHit hit, PickupAndDropHandler pickupAndDropHandler)
    {
        if (hit.collider.attachedRigidbody != null && hit.collider.attachedRigidbody.TryGetComponent<FryingPan>(out var fryingPan))
        {
            pickupAndDropHandler.DropObject();
            MoveToPlaceableSurface(fryingPan.placeableSurface, hit);
        }
        else if (hit.collider.TryGetComponent<CuttingBoard>(out var cuttingBoard))
        {
            pickupAndDropHandler.DropObject();
            MoveToPlaceableSurface(cuttingBoard.PlaceableSurface, hit);
        }
        else if (hit.collider.attachedRigidbody != null && hit.collider.attachedRigidbody.TryGetComponent<BambooTray>(out var bambooTray))
        {
            HandleInteractWithBambooTray(bambooTray, pickupAndDropHandler);
        }
        else
        {
            base.InteractWith(hit, pickupAndDropHandler);
        }
    }

    public bool HandleInteractWithBambooTray(BambooTray bambooTray, PickupAndDropHandler pickupAndDropHandler = null)
    {
        if(canSlice)
        {
            return false;
        }
        IngredientAnchor anchorPoint = bambooTray.GetIngredientAnchor(ingredientType);
        if (anchorPoint == null) return false;
        attachedIngredientAnchor = anchorPoint;
        attachedIngredientAnchor.isEmpty = false;
        pickupAndDropHandler?.DropObject();
        RemoveRigidbodyJoin();
        MoveToPlaceableSurface(bambooTray.placeableSurface, anchorPoint.transform.position, anchorPoint.transform.rotation);
        return true;
    }

    public void RemoveAttachedAnchorPoint()
    {
        if (attachedIngredientAnchor != null)
        {
            attachedIngredientAnchor.isEmpty = true;
            attachedIngredientAnchor = null;
        }
    }

    public override IEnumerator MoveToSurfaceCoroutine(Vector3 dropPosition, PlaceableSurface placeableSurface, Quaternion? targetRotation = null)
    {
        RemoveAttachedIngredientContainer();
        yield return base.MoveToSurfaceCoroutine(dropPosition, placeableSurface, targetRotation);
    }
    public void RemoveAttachedIngredientContainer()
    {
        if (attachedIngredientContainer != null)
        {
            attachedIngredientContainer.containedItems.Remove(this);
            attachedIngredientContainer = null;
        }
    }
    public override void OnPickUp(Transform grabObjectPoint)
    {
        base.OnPickUp(grabObjectPoint);
        RemoveAttachedAnchorPoint();
        RemoveAttachedIngredientContainer();
    }



}
