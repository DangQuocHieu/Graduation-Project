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
    public AnchorPoint attachedAnchorPoint;
    public IngredientType ingredientType;
    public SliceableObject sliceableObject;

    public bool canSlice => sliceableObject != null;

    protected override void Awake()
    {
        base.Awake();
        sliceableObject = GetComponent<SliceableObject>();
    }

    public override void InteractWith(RaycastHit hit, PickupAndDropHandler pickupAndDropHandler)
    {
        if (hit.collider.attachedRigidbody.TryGetComponent<FryingPan>(out var fryingPan))
        {
            pickupAndDropHandler.DropObject();
            MoveToPlaceableSurface(fryingPan.placeableSurface, hit);
        }
        else if (hit.collider.TryGetComponent<CuttingBoard>(out var cuttingBoard))
        {
            pickupAndDropHandler.DropObject();
            MoveToPlaceableSurface(cuttingBoard.PlaceableSurface, hit);
        }
        else if (hit.collider.attachedRigidbody.TryGetComponent<BambooTray>(out var bambooTray))
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
        AnchorPoint anchorPoint = bambooTray.GetAnchorPoint(ingredientType);
        if (anchorPoint == null) return false;
        attachedAnchorPoint = anchorPoint;
        attachedAnchorPoint.isEmpty = false;
        pickupAndDropHandler?.DropObject();
        RemoveRigidbodyJoin();
        MoveToPlaceableSurface(bambooTray.placeableSurface, attachedAnchorPoint.anchor.position, attachedAnchorPoint.anchor.rotation);

        return true;
    }
    public void RemoveAttachedAnchorPoint()
    {
        if (attachedAnchorPoint != null)
        {
            attachedAnchorPoint.isEmpty = true;
            attachedAnchorPoint = null;
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
