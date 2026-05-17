using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class OrderTicketUI : SerializedMonoBehaviour
{
    public Dictionary<IngredientType, RectTransform> ingredientImageDic = new();
    public Dictionary<SauceType, RectTransform> sauceImageDic = new();
    public CustomerOrder currentOrder;

    public void ShowIngredients(CustomerOrder order)
    {
        currentOrder = order;
        var ingredientTypes = order.ingredientTypes;
        var sauceType = order.sauceType;
        foreach (var ingredientImage in ingredientImageDic.Values)
        {
            ingredientImage.gameObject.SetActive(false);
        }
        foreach (var ingredientType in ingredientTypes)
        {
            ingredientImageDic[ingredientType].gameObject.SetActive(true);
        }

        foreach(var sauceImage in sauceImageDic.Values)
        {
            sauceImage.gameObject.SetActive(false);
        }

        sauceImageDic[sauceType].gameObject.SetActive(true);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }


}
