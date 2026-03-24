using System.Collections.Generic;
using UnityEngine;

public class KnifeObject : GrabbaleObject
{
    [SerializeField] private List<SliceableObject> _currentSliceableObjects = new();
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
        cuttingBoard.ItemContainer.GetSliceableList(_currentSliceableObjects);
        foreach (var sliceableObject in _currentSliceableObjects)
        {
            cuttingBoard.ItemContainer.Remove(sliceableObject.grabbaleObject);
            sliceableObject.OnSlice(cuttingBoard);
        }
    }
}
