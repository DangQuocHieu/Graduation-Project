using UnityEngine;

public class Ingredient : GrabbaleObject
{
    public override void InteractWith(RaycastHit hit, PickupAndDropHandler pickupAndDropHandler)
    {
        if(hit.collider.TryGetComponent<CuttingBoard>(out var cuttingBoard))
        {
            Vector3 dropPosition = cuttingBoard.PlaceableSurface.SnapPoint == null ? hit.point : cuttingBoard.PlaceableSurface.SnapPoint.position;
            MoveToPlaceableSurface(dropPosition);
            pickupAndDropHandler.DropObject();
            //cuttingBoard.InteractWithIngredient(this);
        }
        else
        {
            base.InteractWith(hit, pickupAndDropHandler);
        }
    }
}
