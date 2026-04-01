using DG.Tweening;
using UnityEngine;

public class SauceBowl : GrabbableObject
{
    public LiquidContainer shrimpPasteContainer;
    public LiquidContainer fishSauceContainer;

    public override void InteractWith(RaycastHit hit, PickupAndDropHandler pickupAndDropHandler)
    {
        BambooTray tray = hit.collider.GetComponentInParent<BambooTray>();
        if(tray != null)
        {
            Vector3 sauceAnchorPos = tray.sauceAnchor.position;
            pickupAndDropHandler.DropObject();
            MoveToPlaceableSurface(tray.placeableSurface, sauceAnchorPos);  
        }
        else
        {
            base.InteractWith(hit, pickupAndDropHandler);
        }
    }

    public Tween FillSauce(SauceType sauceType)
    {
        
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
        switch(sauceType)
        {
            case SauceType.FishSauce:
                return fishSauceContainer.liquid;
            case SauceType.ShrimpPaste:
                return shrimpPasteContainer.liquid;
        }
        return null;
    }
}
