using System.Collections.Generic;
using UnityEngine;

public class KnifeObject : GrabbableObject
{

    public override void InteractWith(RaycastHit hit, PickupAndDropHandler pickupAndDropHandler)
    {
        if (hit.collider.TryGetComponent<CuttingBoard>(out var cuttingBoard))
        {
            InteractWithCuttingBoard(cuttingBoard);
        }

        else
        {
            base.InteractWith(hit, pickupAndDropHandler);
        }
    }

    public void InteractWithCuttingBoard(CuttingBoard cuttingBoard)
    {
        var currentSliceableObjects = cuttingBoard.PlaceableSurface.ingredientContainer.GetSliceableList();
        foreach (var sliceableObject in currentSliceableObjects)
        {
        
            sliceableObject.OnSlice(cuttingBoard);
        }
    }
}
