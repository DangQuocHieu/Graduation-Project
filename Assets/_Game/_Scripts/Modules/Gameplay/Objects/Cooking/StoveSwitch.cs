using UnityEngine;
using DG.Tweening; // Đừng quên khai báo thư viện này nhé!

public class StoveSwitch : MonoBehaviour, IInteractable
{
    public CookingZone cookingZone;
    private bool isOn = false;
    private Tween rotateTween;

    
    public void OnInteract()
    {
        rotateTween?.Kill();
        isOn = !isOn;

        if (isOn)
        {
            rotateTween = transform.DOLocalRotate(new Vector3(0, 90, 0), 0.3f)
                     .SetEase(Ease.OutBack); 
        }
        else
        {
            rotateTween = transform.DOLocalRotate(new Vector3(0, 0, 0), 0.3f)
                     .SetEase(Ease.OutBack);
        }

        if (cookingZone != null)
        {
            cookingZone.Toggle();
        }
    }
}