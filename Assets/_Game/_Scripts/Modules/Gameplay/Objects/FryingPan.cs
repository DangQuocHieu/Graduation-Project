using DG.Tweening;
using UnityEngine;

public class FryingPan : GrabbableObject
{
    public bool isHot = false;
    public CookingZone attachedCookingZone;
    public Transform pourPoint;
    public CookingOil cookingOil;
    public PlaceableSurface placeableSurface;

    void Update()
    {
        isHot = attachedCookingZone != null && attachedCookingZone.turnOn;
    }

    public override void InteractWith(RaycastHit hit, PickupAndDropHandler pickupAndDropHandler)
    {
        if (hit.collider.TryGetComponent<CookingZone>(out var cookingZone))
        {
            MoveToPlaceableSurface(cookingZone.placeableSurface, hit);
            pickupAndDropHandler.DropObject();
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
        cookingOil.transform.localScale = new Vector3(0.1f, 0f, 0.1f);
        Sequence oilSequence = DOTween.Sequence();
        oilSequence.Append(cookingOil.transform.DOScaleX(1f, 1.2f).SetEase(Ease.OutCubic));
        oilSequence.Join(cookingOil.transform.DOScaleZ(1f, 1.2f).SetEase(Ease.OutCubic));
        oilSequence.Join(cookingOil.transform.DOScaleY(1f, 2f).SetEase(Ease.Linear));
        return oilSequence.SetLink(gameObject);
    }
}
