using UnityEngine;

public enum IngredientType
{
    Tofu,
    RiceNoodle
}
public class Ingredient : GrabbableObject
{
    public float weight;
    public AnchorPoint attachedAnchorPoint;
    public IngredientType ingredientType;
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
            cuttingBoard.itemContainer.Add(this);
        }
        else if (hit.collider.attachedRigidbody.TryGetComponent<BambooTray>(out var bambooTray))
        {
            attachedAnchorPoint = bambooTray.GetAnchorPoint(ingredientType);
            attachedAnchorPoint.isEmpty = false;
            pickupAndDropHandler.DropObject();
            MoveToPlaceableSurface(bambooTray.placeableSurface, attachedAnchorPoint.anchor.position);
        }
        else
        {
            base.InteractWith(hit, pickupAndDropHandler);
        }
    }

    public override void OnPickUp(Transform grabObjectPoint, Collider collider)
    {
        base.OnPickUp(grabObjectPoint, collider);
        if (attachedAnchorPoint != null)
        {
            attachedAnchorPoint.isEmpty = true;
            attachedAnchorPoint = null;
        }
    }



}
