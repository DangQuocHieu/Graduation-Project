using DG.Tweening;
using UnityEngine;

public enum SauceType
{
    FishSauce, ShrimpPaste
}
public class SauceBottle : PourableBottle
{
    public SauceType sauceType;
    public override void InteractWith(RaycastHit hit, PickupAndDropHandler pickupAndDropHandler)
    {
        if (hit.collider.TryGetComponent<SauceBowl>(out var bowl))
        {
            pickupAndDropHandler.DropObject();
            StartCoroutine(PourRoutine(bowl.shrimpPasteContainer, pickupAndDropHandler, () => bowl.FillSauce(sauceType)));
        }
        else
        {
            base.InteractWith(hit, pickupAndDropHandler);
        }
    }

    protected override Tween CloseCap()
    {
        return topNap.DOLocalMoveY(topNap.localPosition.y - 10f, 0.3f).SetEase(Ease.InOutSine).SetLink(gameObject);
    }

    protected override Tween OpenCap()
    {
        return topNap.DOLocalMoveY(topNap.localPosition.y + 10f, 0.3f).SetEase(Ease.InOutSine).SetLink(gameObject);
    }
}