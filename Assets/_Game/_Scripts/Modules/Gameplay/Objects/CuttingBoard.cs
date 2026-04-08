using System.Collections.Generic;
using UnityEngine;

public class CuttingBoard : GrabbableObject
{
    public PlaceableSurface PlaceableSurface;


    protected override void Awake()
    {
        base.Awake();
    }
    public override void InteractWith(RaycastHit hit, PickupAndDropHandler pickupAndDropHandler)
    {
        if(hit.collider.attachedRigidbody != null && hit.collider.attachedRigidbody.TryGetComponent<BambooTray>(out var bambooTray))
        {
            StartCoroutine(bambooTray.FillCookableObjectCoroutine(PlaceableSurface.ingredientContainer.GetIngredientList()));
        }
        base.InteractWith(hit, pickupAndDropHandler);
    }

}
