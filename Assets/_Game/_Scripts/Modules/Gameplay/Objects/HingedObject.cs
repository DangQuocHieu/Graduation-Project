using DG.Tweening;
using UnityEngine;

public class HingedObject : MonoBehaviour, IInteractable
{
    public bool isOpened = false;
    public Vector3 localRotationWhenOpen;
    public Vector3 localRotationWhenClose;

    private Tween toggleTween;
    public void Toggle()
    {
        isOpened = !isOpened;
        toggleTween?.Kill();
        if(isOpened)
        {
            toggleTween = Open();
        }
        else
        {
            toggleTween = Close();
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
