using UnityEngine;

public enum IngredientType
{
    Tofu,
    RiceNoodle
}
public class Ingredient : GrabbableObject
{
    public AnchorPoint attachedAnchorPoint;
    public IngredientType ingredientType;
    public override void InteractWith(RaycastHit hit, PickupAndDropHandler pickupAndDropHandler)
    {
        if(hit.collider.attachedRigidbody.TryGetComponent<FryingPan>(out var fryingPan))
        {
            MoveToPlaceableSurface(fryingPan.placeableSurface, hit);
            pickupAndDropHandler.DropObject();
        }
        else if(hit.collider.TryGetComponent<CuttingBoard>(out var cuttingBoard))
        {
            MoveToPlaceableSurface(cuttingBoard.PlaceableSurface, hit);
            pickupAndDropHandler.DropObject();
            cuttingBoard.itemContainer.Add(this);
        }
        else if(hit.collider.attachedRigidbody.TryGetComponent<BambooTray>(out var bambooTray))
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
        if(attachedAnchorPoint != null)
        {
            attachedAnchorPoint.isEmpty = true;
            attachedAnchorPoint = null;
        }
    }



}
