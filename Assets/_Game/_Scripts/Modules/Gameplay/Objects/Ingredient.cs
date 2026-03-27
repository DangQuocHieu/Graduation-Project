using UnityEngine;

public class Ingredient : GrabbableObject
{
    public override void InteractWith(RaycastHit hit, PickupAndDropHandler pickupAndDropHandler)
    {
        if(hit.collider.TryGetComponent<CuttingBoard>(out var cuttingBoard))
        {
            MoveToPlaceableSurface(cuttingBoard.PlaceableSurface, hit);
            pickupAndDropHandler.DropObject();
            cuttingBoard.InteractWithIngredient(this);      
        }
        else
        {
            base.InteractWith(hit, pickupAndDropHandler);
        }
    }
}
