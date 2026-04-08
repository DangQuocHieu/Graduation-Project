using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class KnifeObject : GrabbableObject
{

    public override void InteractWith(RaycastHit hit, PickupAndDropHandler pickupAndDropHandler)
    {
        if(hit.collider.TryGetComponent<Ingredient>(out var ingredient) && ingredient.canSlice)
        {
            if(ingredient.attachedIngredientContainer != null && ingredient.attachedIngredientContainer.TryGetComponent<CuttingBoard>(out var cuttingBoard))
            {
                ingredient.sliceableObject.OnSlice(cuttingBoard);
            }
        }
        else
        {
            base.InteractWith(hit, pickupAndDropHandler);
        }
    }


}
