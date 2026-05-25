using DG.Tweening;
using DQHieu.Framework.Audio;
using UnityEngine;

public class HingedObject : MonoBehaviour, IInteractable
{
    public bool isOpened = false;
    public Vector3 localRotationWhenOpen;
    public Vector3 localRotationWhenClose;
    private Tween toggleTween;
    public AudioData openSfx;
    public AudioData closeSfx;
    public void Toggle()
    {
        isOpened = !isOpened;
        toggleTween?.Kill();
        if(isOpened)
        {
            toggleTween = Open();
            if(openSfx != null) AudioManager.Instance.PlaySFX(openSfx);
        }
        else
        {
            toggleTween = Close();
            if(closeSfx != null) AudioManager.Instance.PlaySFX(closeSfx);
        }
    }

    public Tween Open()
    {
        return transform.DOLocalRotate(localRotationWhenOpen, 0.3f).SetEase(Ease.OutQuad).SetLink(gameObject);
    }
    
    public Tween Close()
    {
        return transform.DOLocalRotate(localRotationWhenClose, 0.3f).SetEase(Ease.InQuad).SetLink(gameObject);
    }

    public void OnInteract()
    {
        Toggle();
    }
}
