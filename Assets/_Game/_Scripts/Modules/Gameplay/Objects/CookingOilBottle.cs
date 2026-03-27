using UnityEngine;

public class CookingOilBottle : GrabbableObject
{
    
    public override void InteractWith(RaycastHit hit, PickupAndDropHandler pickupAndDropHandler)
    {
        var fryingPan = hit.collider.GetComponentInParent<FryingPan>();
        if(fryingPan != null)
        {
            HandleInteractWithFryingPan(fryingPan);
        }
        else
        {
            base.InteractWith(hit, pickupAndDropHandler);
        }
    }

    private void HandleInteractWithFryingPan(FryingPan fryingPan)
    {
        
    }
}
