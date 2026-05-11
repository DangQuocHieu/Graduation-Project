using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public enum UISlideTweenDirection
{
    Up,
    Down,
    Left,
    Right
}

public class UISlideTween : MonoBehaviour
{
    private Vector2 initialAnchoredPosition; 
    public RectTransform rectTransform;

    [Title("Direction Config")]
    public UISlideTweenDirection slideDirection = UISlideTweenDirection.Down;
    [Tooltip("Khoảng cách trượt của UI")]
    public float slideDistance = 10000f;

    [Title("Slide In Config")]
    public float slideInDuration = 0.3f;
    public Ease slideInEase = Ease.OutQuad;
    
    [Title("Slide Out Config")]
    public float slideOutDuration = 0.3f;
    public Ease slideOutEase = Ease.InQuad;

    void Awake()
    {
   
        if (rectTransform == null) 
            rectTransform = GetComponent<RectTransform>();
            
        initialAnchoredPosition = rectTransform.anchoredPosition;
    }


    private Vector2 GetOffsetPosition()
    {
        Vector2 offsetPosition = initialAnchoredPosition;

        switch (slideDirection)
        {
            case UISlideTweenDirection.Up:
                offsetPosition.y += slideDistance;
                break;
            case UISlideTweenDirection.Down:
                offsetPosition.y -= slideDistance;
                break;
            case UISlideTweenDirection.Left:
                offsetPosition.x -= slideDistance;
                break;
            case UISlideTweenDirection.Right:
                offsetPosition.x += slideDistance;
                break;
        }

        return offsetPosition;
    }

    [Button("Test Slide In")]
    public Tween SlideIn()
    {
        rectTransform.gameObject.SetActive(true);
        rectTransform.DOKill();
        rectTransform.anchoredPosition = GetOffsetPosition();
        return rectTransform.DOAnchorPos(initialAnchoredPosition, slideInDuration)
            .SetEase(slideInEase);
    }

    [Button("Test Slide Out")]
    public Tween SlideOut()
    {
        rectTransform.DOKill();
        return rectTransform.DOAnchorPos(GetOffsetPosition(), slideOutDuration)
            .SetEase(slideOutEase)
            .OnComplete(() => 
            {
                rectTransform.gameObject.SetActive(false);
            });
    }
}