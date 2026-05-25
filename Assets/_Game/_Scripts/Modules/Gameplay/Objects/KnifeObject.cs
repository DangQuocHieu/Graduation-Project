using System;
using System.Collections.Generic;
using DQHieu.Framework.Audio;
using Unity.VisualScripting;
using UnityEngine;

public class KnifeObject : GrabbableObject
{
    public PlacePoint attachedPlacepoint;
    public AudioData cutSfx;

    protected override void SetUpRigidbody()
    {
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.isKinematic = true;
    }
    
    public override void InteractWith(RaycastHit hit, PickupAndDropHandler pickupAndDropHandler)
    {
        if (hit.collider.TryGetComponent<Ingredient>(out var ingredient) && ingredient.canSlice)
        {
            if (ingredient.attachedIngredientContainer != null && ingredient.attachedIngredientContainer.TryGetComponent<CuttingBoard>(out var cuttingBoard))
            {
                ingredient.sliceableObject.OnSlice(cuttingBoard);
                AudioManager.Instance.PlaySFX(cutSfx);
            }
        }
        else if (hit.collider.TryGetComponent<KnifeBlockObject>(out var knifeBlockObject))
        {
            pickupAndDropHandler.DropObject();
            HandleInteractWithKnifeBlockObject(knifeBlockObject);
        }
        else
        {
            base.InteractWith(hit, pickupAndDropHandler);
        }
    }

    private void HandleInteractWithKnifeBlockObject(KnifeBlockObject knifeBlockObject)
    {
        var placePoint = knifeBlockObject.GetAvaiablePlacePoint();
        if (placePoint != null)
        {
            placePoint.isEmpty = false;
            this.attachedPlacepoint = placePoint;
            MoveToPositionAndRotation(placePoint.point.position, placePoint.point.rotation, keepKinematic: true, onComplete: () =>
            {

            });
        }
    }

    public override void OnPickUp(Transform grabObjectPoint)
    {
        if(attachedPlacepoint != null)
        {
            attachedPlacepoint.isEmpty = true;
            attachedPlacepoint = null;
        }
        base.OnPickUp(grabObjectPoint);
    }


}
