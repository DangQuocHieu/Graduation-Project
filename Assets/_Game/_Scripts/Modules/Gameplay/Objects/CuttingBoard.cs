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
        if (hit.collider.GetComponentInParent<FryingPan>() != null)
        {
            var fryingPan = hit.collider.GetComponentInParent<FryingPan>();
            //Chuyen cac cookableobject tu cuttingboard -> fryingpan
            StartCoroutine(fryingPan.placeableSurface.ingredientContainer.FillCoroutine(PlaceableSurface.ingredientContainer.GetCookableList()));
        }
        else if(hit.collider.GetComponentInParent<BambooTray>() != null)
        {
            var bambooTray = hit.collider.GetComponentInParent<BambooTray>();
            StartCoroutine(bambooTray.FillCookableObjectCoroutine(PlaceableSurface.ingredientContainer.GetIngredientList()));
        }
        base.InteractWith(hit, pickupAndDropHandler);
    }

}
