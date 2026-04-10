using DG.Tweening;
using DQHieu.Framework;
using UnityEngine;

public class LadleObject : GrabbableObject
{
    public Transform oilLiquidVisual;
    public bool containCookingOil = false;
    private Sequence pourSeq;

    protected override void Awake()
    {
        base.Awake();
        oilLiquidVisual.gameObject.SetActive(false);
    }
    public override void InteractWith(RaycastHit hit, PickupAndDropHandler pickupAndDropHandler)
    {
        if (hit.collider.attachedRigidbody != null && hit.collider.attachedRigidbody.TryGetComponent<FryingPan>(out var fryingPan))
        {
            if (fryingPan.containCookingOil)
            {
                FillCookingOil();
            }
        }
        else if (hit.collider.TryGetComponent<SauceBowl>(out var sauceBowl))
        {
            if (containCookingOil)
            {
                PourOilIntoBowl(sauceBowl, pickupAndDropHandler);
            }
        }
        base.InteractWith(hit, pickupAndDropHandler);
    }

    public void FillCookingOil()
    {
        containCookingOil = true;
        oilLiquidVisual.gameObject.SetActive(true);
    }

    public void PourOilIntoBowl(SauceBowl bowl, PickupAndDropHandler pickupAndDropHandler)
    {
        pourSeq?.Kill();
        Vector3 originalRotation = transform.eulerAngles;
        pickupAndDropHandler.DropObject();
        rb.isKinematic = true;
        EventBus.SendMessage<PourLiquid>(new PourLiquid());
        pourSeq = DOTween.Sequence().SetLink(gameObject);
        pourSeq.Append(transform.DOMove(bowl.shrimpPasteContainer.pourPoint.position + Vector3.up * 0.2f, 0.2f).SetEase(Ease.Linear));
        pourSeq.Append(transform.DORotate(new Vector3(60f, originalRotation.y, originalRotation.z), 0.2f).SetEase(Ease.Linear));
        pourSeq.AppendCallback(() =>
        {
           oilLiquidVisual.gameObject.SetActive(false); 
           containCookingOil = false;
        });
        pourSeq.AppendInterval(0.2f);
        pourSeq.Append(transform.DORotate(originalRotation, 0.2f));
        pourSeq.OnComplete(() =>
        {
            pickupAndDropHandler.PickupObject(this);
            EventBus.SendMessage<PourLiquidComplete>(new PourLiquidComplete());
        });
    }



}
