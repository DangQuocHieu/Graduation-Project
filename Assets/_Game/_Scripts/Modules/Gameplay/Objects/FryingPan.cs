using UnityEngine;

public class FryingPan : GrabbableObject
{
    public bool isHot = false;
    public CookingZone attachedCookingZone;
    public Transform pourPoint;

    void Update()
    {
        isHot = attachedCookingZone != null && attachedCookingZone.turnOn;    
    }

    public override void InteractWith(RaycastHit hit, PickupAndDropHandler pickupAndDropHandler)
    {
        if(hit.collider.TryGetComponent<CookingZone>(out var cookingZone))
        {
            MoveToPlaceableSurface(cookingZone.placeableSurface, hit);
            pickupAndDropHandler.DropObject();
        }
        else
        {
            base.InteractWith(hit, pickupAndDropHandler);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.TryGetComponent<CookingZone>(out var cookingZone))
        {
            attachedCookingZone = cookingZone;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if(collision.gameObject.TryGetComponent<CookingZone>(out var cookingZone))
        {
            if(attachedCookingZone != null && attachedCookingZone == cookingZone)
            {
                attachedCookingZone = null;
            }
        }
    }
}
