using System;
using System.Collections;
using DG.Tweening;
using DQHieu.Framework;
using Sirenix.OdinInspector;
using UnityEngine;
using DQHieu.Framework.Audio;

public abstract class PourableBottle : GrabbableObject
{
    [Title("Pourable Properties")]
    public ParticleSystem pourEffect;
    public Transform topNap;
    public AudioData pouringSfx;
    private AudioEmitter pouringAudioEmitter;

    protected IEnumerator PourRoutine(LiquidContainer liquidContainer, PickupAndDropHandler pickupAndDropHandler, Func<Tween> fillLiquidTween)
    {
        EventBus.Raise<PourLiquid>(new PourLiquid());
        rb.isKinematic = true;
        yield return OpenCap().WaitForCompletion();
        transform.DORotate(new Vector3(0, 0f, 180f), 0.3f, RotateMode.LocalAxisAdd).SetEase(Ease.InOutSine).SetLink(gameObject);
        yield return transform.DOMove(liquidContainer.pourPoint.position, 0.3f).SetEase(Ease.InOutSine).SetLink(gameObject).WaitForCompletion();
        if (pouringAudioEmitter == null)
        {
            pouringAudioEmitter = AudioManager.Instance.PlaySFX(pouringSfx, fadeInDuration: 1f);
        }
        pourEffect.Play();
        yield return new WaitForSeconds(0.3f);

        yield return fillLiquidTween?.Invoke().WaitForCompletion();

        pourEffect.Stop();
        if (pouringAudioEmitter != null)
        {
            AudioManager.Instance.StopSFX(pouringAudioEmitter, fadeDuration: 1f);
        }
        pickupAndDropHandler.PickupObject(this);
        StopWaitForPickupCompleteCoroutine();
        yield return WaitForPickupComplete();
        yield return CloseCap().WaitForCompletion();
        EventBus.Raise<PourLiquidComplete>(new PourLiquidComplete());

    }

    protected abstract Tween OpenCap();
    protected abstract Tween CloseCap();

}
