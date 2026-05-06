using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class OrderTicketUI : SerializedMonoBehaviour
{
    public Dictionary<IngredientType, RectTransform> ingredientImageDic = new();
    public RectTransform container;
    private Tween moveTween;
    public void ShowIngredients(List<IngredientType> ingredientTypes)
    {
        foreach (var ingredientImage in ingredientImageDic.Values)
        {
            ingredientImage.gameObject.SetActive(false);
        }
        foreach (var ingredientType in ingredientTypes)
        {
            ingredientImageDic[ingredientType].gameObject.SetActive(true);
        }
    }

    public void Show()
    {
        moveTween?.Kill();
        gameObject.SetActive(true);
        container.anchoredPosition = new Vector2(0, 500);
        moveTween = container.DOAnchorPosY(-500, 0.2f)
            .SetRelative(true)
            .SetEase(Ease.OutQuad);
    }

    public void Hide()
    {
        moveTween?.Kill();
        moveTween = container.DOAnchorPosY(500, 0.2f)
            .SetRelative(true)
            .SetEase(Ease.InQuad)
            .OnComplete(() => gameObject.SetActive(false));
    }


}
