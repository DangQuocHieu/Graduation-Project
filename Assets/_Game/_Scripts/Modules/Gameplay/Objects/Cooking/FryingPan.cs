using DG.Tweening;
using UnityEngine;

public class FryingPan : GrabbableObject
{
    public bool isHot = false;
    public CookingZone attachedCookingZone;
    public LiquidContainer oilContainer;
    public PlaceableSurface placeableSurface;

    void Update()
    {
        isHot = attachedCookingZone != null && attachedCookingZone.turnOn;
    }

    public override void InteractWith(RaycastHit hit, PickupAndDropHandler pickupAndDropHandler)
    {
        if (hit.collider.TryGetComponent<CookingZone>(out var cookingZone))
        {
            pickupAndDropHandler.DropObject();
            MoveToPlaceableSurface(cookingZone.placeableSurface, hit);
        }
        else
        {
            base.InteractWith(hit, pickupAndDropHandler);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<CookingZone>(out var cookingZone))
        {
            attachedCookingZone = cookingZone;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<CookingZone>(out var cookingZone))
        {
            if (attachedCookingZone != null && attachedCookingZone == cookingZone)
            {
                attachedCookingZone = null;
            }
        }
    }

    public Tween FillCookingOil()
    {
        Transform cookingOil = oilContainer.liquid;
        cookingOil.localScale = new Vector3(0.1f, 0f, 0.1f);
        Sequence oilSequence = DOTween.Sequence();
        oilSequence.Append(cookingOil.DOScaleX(1f, 1.2f).SetEase(Ease.OutCubic));
        oilSequence.Join(cookingOil.DOScaleZ(1f, 1.2f).SetEase(Ease.OutCubic));
        oilSequence.Join(cookingOil.DOScaleY(1f, 2f).SetEase(Ease.Linear));
        return oilSequence.SetLink(gameObject);
    }
}
