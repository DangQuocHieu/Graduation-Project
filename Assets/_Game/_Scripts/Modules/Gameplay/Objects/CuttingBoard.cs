using System.Collections;
using System.Collections.Generic;
using DQHieu.Framework;
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
            StartCoroutine(InteractWithBambooTrayCoroutine(bambooTray));
        }
        base.InteractWith(hit, pickupAndDropHandler);
    }

    private IEnumerator InteractWithBambooTrayCoroutine(BambooTray bambooTray)
    {
        EventBus.Raise<PourLiquid>(new PourLiquid());

        yield return StartCoroutine(bambooTray.FillCookableObjectCoroutine(PlaceableSurface.ingredientContainer.GetIngredientList()));

        EventBus.Raise<PourLiquidComplete>(new PourLiquidComplete());
    }

}
