using UnityEngine;
using DG.Tweening;

public class CookingOilBottle : PourableBottle
{
    public override void InteractWith(RaycastHit hit, PickupAndDropHandler pickupAndDropHandler)
    {
        if( hit.collider.attachedRigidbody != null && hit.collider.attachedRigidbody.TryGetComponent<FryingPan>(out var fryingPan))
        {
            pickupAndDropHandler.DropObject();
            StartCoroutine(PourRoutine(fryingPan.oilContainer, pickupAndDropHandler, fryingPan.FillCookingOil));
            
        }
        else
        {
            base.InteractWith(hit, pickupAndDropHandler);
        }
    }

    protected override Tween OpenCap()
    {
        return topNap.DOLocalRotate(new Vector3(0f, 0f, 90f), 0.3f).SetEase(Ease.InOutSine).SetLink(gameObject);
    }

    protected override Tween CloseCap()
    {
        Debug.Log("CLOSE CAP");
        return topNap.DOLocalRotate(new Vector3(0f, 0f, 0f), 0.3f).SetEase(Ease.InOutSine).SetLink(gameObject);
    }
}
