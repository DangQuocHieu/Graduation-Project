using DG.Tweening;
using UnityEngine;

public class SauceBowl : GrabbableObject
{
    public LiquidContainer shrimpPasteContainer;
    public LiquidContainer fishSauceContainer;
    public BambooTray attachedBambooTray;
    public bool fill = false;
    public SauceType sauceType;

    public override void InteractWith(RaycastHit hit, PickupAndDropHandler pickupAndDropHandler)
    {
        if ( hit.collider.attachedRigidbody != null && hit.collider.attachedRigidbody.TryGetComponent<BambooTray>(out var tray))
        {
            attachedBambooTray = tray;
            tray.attachedBowl = this;
            Vector3 sauceAnchorPos = tray.sauceAnchor.position;
            pickupAndDropHandler.DropObject();
            MoveToPlaceableSurface(tray.placeableSurface, sauceAnchorPos);
        }
        else if(hit.collider.attachedRigidbody != null && hit.collider.attachedRigidbody.TryGetComponent<FryingPan>(out var fryingPan))
        {
            pickupAndDropHandler.DropObject();
            MoveToPlaceableSurface(fryingPan.placeableSurface, hit);
        }
        else if(hit.collider.attachedRigidbody != null && hit.collider.attachedRigidbody.TryGetComponent<CuttingBoard>(out var cuttingBoard))
        {
            pickupAndDropHandler.DropObject();
            MoveToPlaceableSurface(cuttingBoard.PlaceableSurface, hit);
        }
        else
        {
            base.InteractWith(hit, pickupAndDropHandler);
        }
    }

    public override void OnPickUp(Transform grabObjectPoint)
    {
        base.OnPickUp(grabObjectPoint);
        if(attachedBambooTray != null)
        {
            attachedBambooTray.attachedBowl = null;
            attachedBambooTray = null;
        }
    }

    public Tween FillSauce(SauceType sauceType)
    {
        if(fill) return null;
        fill = true;
        this.sauceType = sauceType;
        Transform liquid = GetLiquidBySauceType(sauceType);
        liquid.localScale = new Vector3(0.1f, 0f, 0.1f);
        Sequence fillSeq = DOTween.Sequence();
        fillSeq.Append(liquid.DOScaleX(1f, 0.6f).SetEase(Ease.OutCubic));
        fillSeq.Join(liquid.DOScaleZ(1f, 0.6f).SetEase(Ease.OutCubic));
        fillSeq.Join(liquid.DOScaleY(1f, 1f).SetEase(Ease.Linear));
        return fillSeq.SetLink(gameObject);
    }

    public Transform GetLiquidBySauceType(SauceType sauceType)
    {
        switch (sauceType)
        {
            case SauceType.FishSauce:
                return fishSauceContainer.liquid;
            case SauceType.ShrimpPaste:
                return shrimpPasteContainer.liquid;
        }
        return null;
    }
}
