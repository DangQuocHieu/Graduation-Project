using UnityEngine;

public class ShrimpPasteBottle : GrabbableObject
{
    public ParticleSystem pourEffect;
    public Transform topNap;

    public override void InteractWith(RaycastHit hit, PickupAndDropHandler pickupAndDropHandler)
    {
        if(hit.collider.TryGetComponent<Bowl>(out var bowl))
        {
            pickupAndDropHandler.DropObject();
            HandleInteractWithBowl(bowl, pickupAndDropHandler);
        }
        base.InteractWith(hit, pickupAndDropHandler);
    }

    private void HandleInteractWithBowl(Bowl bowl, PickupAndDropHandler pickupAndDropHandler)
    {
        StartCoroutine(InteractWithBowlCoroutine(bowl, pickupAndDropHandler));
    }

    private System.Collections.IEnumerator InteractWithBowlCoroutine(Bowl bowl, PickupAndDropHandler pickupAndDropHandler)
    {
        yield return null;
    }
}
