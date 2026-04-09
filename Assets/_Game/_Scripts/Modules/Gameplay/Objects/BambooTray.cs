using System.Collections;
using System.Collections.Generic;
using DQHieu.Framework;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

public class BambooTray : GrabbableObject
{
    private Dictionary<IngredientType, List<IngredientAnchor>> IngredientAnchorsDic = new(); 
    public PlaceableSurface placeableSurface;

    [Title("Sauce")]
    public Transform sauceAnchor;

    [Title("Tofu")]
    public List<IngredientAnchor> tofuIngredientAnchors = new();

    [Title("Rice Noodle")]
    public List<IngredientAnchor> riceNoodleIngredientAnchors = new();

    [Title("Cucumber")]
    public List<IngredientAnchor> cucumberIngredientAnchors = new();

    [Title("Herb")]
    public List<IngredientAnchor> herbIngredientAnchors = new();

    protected override void Awake()
    {
        base.Awake();
        IngredientAnchorsDic.Add(IngredientType.Tofu, tofuIngredientAnchors);
        IngredientAnchorsDic.Add(IngredientType.RiceNoodle, riceNoodleIngredientAnchors);
        IngredientAnchorsDic.Add(IngredientType.Cucumber, cucumberIngredientAnchors);
        IngredientAnchorsDic.Add(IngredientType.Herb, herbIngredientAnchors);

    }

   
    public IngredientAnchor GetIngredientAnchor(IngredientType ingredientType)
    {
        var IngredientAnchor = IngredientAnchorsDic[ingredientType];
        return IngredientAnchor.Find(T => T.isEmpty);
    }

    public override void InteractWith(RaycastHit hit, PickupAndDropHandler pickupAndDropHandler)
    {
        if (hit.collider.attachedRigidbody != null && hit.collider.attachedRigidbody.TryGetComponent<FryingPan>(out var fryingPan))
        {
            var cookableObjects = placeableSurface.ingredientContainer.GetCookableList();
            StartCoroutine(fryingPan.placeableSurface.ingredientContainer.FillCoroutine(cookableObjects));
        }
        else if(hit.collider.TryGetComponent<Ingredient>(out var ingredient))
        {
            ingredient.HandleInteractWithBambooTray(this);
        }
        else if (hit.collider.TryGetComponent<ShopItem>(out var shopItem))
        {
            var ingredientSO = shopItem.shopItemSO.grabbableObjectSO as IngredientSO;
            if (ingredientSO != null)
            {
                if ((ingredientSO.ingredientType is IngredientType.Herb || ingredientSO.ingredientType is IngredientType.RiceNoodle)
                && (GetIngredientAnchor(ingredientSO.ingredientType) != null))
                {
                    EventBus.SendMessage<InteractWithShopItemEvent>(new InteractWithShopItemEvent(shopItem));
                }
            }
        }
        base.InteractWith(hit, pickupAndDropHandler);
    }

    public IEnumerator FillCookableObjectCoroutine(List<Ingredient> cookableObjects)
    {
        foreach (var cookableobject in cookableObjects)
        {
            if (cookableobject.HandleInteractWithBambooTray(this))
            {
                yield return new WaitForSeconds(0.1f);
            }
        }
    }

}
