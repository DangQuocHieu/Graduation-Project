using UnityEngine;

public class FryingPan : GrabbableObject
{
    public bool isHot = false;
    public CookingZone attachedCookingZone;

    void Update()
    {
        isHot = attachedCookingZone != null && attachedCookingZone.turnOn;    
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
