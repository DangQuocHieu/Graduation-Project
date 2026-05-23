using System.Collections;
using System.Collections.Generic;
using DQHieu.Framework;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

public class BambooTray : GrabbableObject
{
    public Dictionary<IngredientType, List<IngredientAnchor>> IngredientAnchorsDic = new();
    public PlaceableSurface placeableSurface;
    public bool applyFullPortionCheck = true;

    [Title("Sauce")]
    public Transform sauceAnchor;
    public SauceBowl attachedBowl;

    [Title("Tofu")]
    public List<IngredientAnchor> tofuIngredientAnchors = new();

    [Title("Rice Noodle")]
    public List<IngredientAnchor> riceNoodleIngredientAnchors = new(); 

    [Title("Cucumber")]
    public List<IngredientAnchor> cucumberIngredientAnchors = new();

    [Title("Herb")]
    public List<IngredientAnchor> herbIngredientAnchors = new();

    public Dictionary<IngredientType, int> CurrentIngredientsCount { get; private set; } = new();

    protected override void Awake()
    {
        base.Awake();
        IngredientAnchorsDic.Add(IngredientType.Tofu, tofuIngredientAnchors);
        IngredientAnchorsDic.Add(IngredientType.RiceNoodle, riceNoodleIngredientAnchors);
        IngredientAnchorsDic.Add(IngredientType.Cucumber, cucumberIngredientAnchors);
        IngredientAnchorsDic.Add(IngredientType.Herb, herbIngredientAnchors);

        foreach (var kvp in IngredientAnchorsDic)
        {
            int count = 0;
            foreach (var anchor in kvp.Value)
            {
                if (anchor.attachedIngredient != null)
                {
                    count++;
                }
            }
            CurrentIngredientsCount.Add(kvp.Key, count);
        }
    }


    public IngredientAnchor GetIngredientAnchor(IngredientType ingredientType)
    {
        var IngredientAnchor = IngredientAnchorsDic[ingredientType];
        return IngredientAnchor.Find(T => T.isEmpty);
    }

    public void AddIngredientCount(IngredientType ingredientType)
    {
        if (CurrentIngredientsCount.ContainsKey(ingredientType))
        {
            CurrentIngredientsCount[ingredientType]++;
        }
    }

    public void RemoveIngredientCount(IngredientType ingredientType)
    {
        if (CurrentIngredientsCount.ContainsKey(ingredientType) && CurrentIngredientsCount[ingredientType] > 0)
        {
            CurrentIngredientsCount[ingredientType]--;
        }
    }

    public override void InteractWith(RaycastHit hit, PickupAndDropHandler pickupAndDropHandler)
    {
        if (hit.collider.attachedRigidbody != null && hit.collider.attachedRigidbody.TryGetComponent<FryingPan>(out var fryingPan))
        {
            var cookableObjects = placeableSurface.ingredientContainer.GetCookableList();
            StartCoroutine(fryingPan.placeableSurface.ingredientContainer.FillCoroutine(cookableObjects));
        }
        else if (hit.collider.TryGetComponent<Ingredient>(out var ingredient))
        {
            if (ingredient.HandleInteractWithBambooTray(this))
            {

                EventBus.Raise<PickUpIngredientByTray>(new PickUpIngredientByTray());
                StartCoroutine(WaitForIngredientPickedUpByTray(ingredient));
            }

        }
        else if (hit.collider.TryGetComponent<ShopItem>(out var shopItem))
        {
            var ingredientSO = shopItem.shopItemSO.grabbableObjectSO as IngredientSO;
            if (ingredientSO != null)
            {
                if ((ingredientSO.ingredientType is IngredientType.Herb || ingredientSO.ingredientType is IngredientType.RiceNoodle)
                && (GetIngredientAnchor(ingredientSO.ingredientType) != null))
                {
                    EventBus.Raise<InteractWithShopItemEvent>(new InteractWithShopItemEvent(shopItem));
                }
            }
        }
        else if (hit.collider.TryGetComponent<Customer>(out var customer))
        {
            if (customer.currentState is CustomerState.WaitingForFood || customer.currentState is CustomerState.ServiceDelayed)
            {
                if (!applyFullPortionCheck)
                {
                    OnServed();
                    pickupAndDropHandler.DropObject();
                    ChairObject chairObject = customer.attachedChairObject;
                    MoveToPlaceableSurface(chairObject.attachedTableSurface, chairObject.dishPlacePoint.position, onComplete: () =>
                    {
                        customer.HandleFoodServed(this);
                    });
                }
                else if (IsFullPortion(customer.customerOrder.ingredientTypes))
                {
                    OnServed();
                    pickupAndDropHandler.DropObject();
                    ChairObject chairObject = customer.attachedChairObject;
                    MoveToPlaceableSurface(chairObject.attachedTableSurface, chairObject.dishPlacePoint.position, onComplete: () =>
                    {
                        customer.HandleFoodServed(this);
                    });
                }
            }

        }
        base.InteractWith(hit, pickupAndDropHandler);
    }

    private IEnumerator WaitForIngredientPickedUpByTray(Ingredient ingredient)
    {
        yield return new WaitUntil(() => ingredient.isMoveToSurfaceCompleted);
        EventBus.Raise<ItemPickedUpComplete>(new ItemPickedUpComplete());
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

    public bool IsIngredientAnchorsFull(List<IngredientAnchor> anchors)
    {
        foreach (var anchor in anchors)
        {
            if (anchor.isEmpty)
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Kiểm tra xem một loại nguyên liệu (IngredientType) đã đủ định lượng (đầy các điểm đặt) trên mẹt chưa.
    /// </summary>
    public bool IsIngredientEnough(IngredientType ingredientType)
    {
        if (IngredientAnchorsDic.TryGetValue(ingredientType, out var anchors))
        {
            return IsIngredientAnchorsFull(anchors);
        }
        return false;
    }

    public bool IsFullPortion(List<IngredientType> ingredientTypes)
    {
        foreach (var type in ingredientTypes)
        {
            var anchors = IngredientAnchorsDic[type];
            if (!IsIngredientAnchorsFull(anchors)) return false;
        }
        return attachedBowl != null && attachedBowl.fill;
    }

    public void DestroyIngredient(List<IngredientAnchor> anchors)
    {
        foreach (var anchor in anchors)
        {
            if (anchor.attachedIngredient != null)
                Destroy(anchor.attachedIngredient.gameObject);
        }
    }

    public void ReleseDish()
    {
        if (attachedBowl != null)
        {
            Destroy(attachedBowl.gameObject);
        }
        foreach (var anchor in IngredientAnchorsDic.Values)
        {
            DestroyIngredient(anchor);
        }
        Destroy(gameObject);
    }

    public void OnServed()
    {
        canBePickedUp = false;
        foreach (var anchors in IngredientAnchorsDic.Values)
        {
            foreach (var anchor in anchors)
            {
                if (anchor.attachedIngredient != null)
                {
                    anchor.attachedIngredient.canBePickedUp = false;
                }
            }
        }
    }

}
