using UnityEngine;
using System.Collections;
using DG.Tweening;
using DQHieu.Framework;
using UnityEditor.Callbacks;
using UnityEditor;

public class CookingOilBottle : GrabbableObject
{
    public ParticleSystem pourEffect;
    public override void InteractWith(RaycastHit hit, PickupAndDropHandler pickupAndDropHandler)
    {
        var fryingPan = hit.collider.GetComponentInParent<FryingPan>();
        if(fryingPan != null)
        {
            pickupAndDropHandler.DropObject();
            HandleInteractWithFryingPan(fryingPan, pickupAndDropHandler);
        }
        else
        {
            base.InteractWith(hit, pickupAndDropHandler);
        }
    }

    private void HandleInteractWithFryingPan(FryingPan fryingPan, PickupAndDropHandler pickupAndDropHandler)
    {
        StartCoroutine(InteractWithFryingPanCoroutine(fryingPan, pickupAndDropHandler));
    }

    private IEnumerator InteractWithFryingPanCoroutine(FryingPan fryingPan, PickupAndDropHandler pickupAndDropHandler)
    {
        EventBus.SendMessage<PourOilIntoPan>(new PourOilIntoPan());
        rb.isKinematic = true;  
        transform.DOMove(fryingPan.pourPoint.position, 0.5f).SetEase(Ease.InOutSine).SetLink(gameObject);
        yield return transform.DORotate(new Vector3(-100f, 0f, 0f), 0.5f).SetEase(Ease.InOutSine).SetLink(gameObject).WaitForCompletion();
        pourEffect.Play();
        // yield return new WaitForSeconds(2f);
        // pickupAndDropHandler.PickupObject(this);
        // yield return new WaitForSeconds(0.5f);
        // EventBus.SendMessage<PourOilIntoPanComplete>(new PourOilIntoPanComplete());
    }
}
