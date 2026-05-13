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
        else if (hit.collider.TryGetComponent<Ingredient>(out var ingredient))
        {
            if (ingredient.HandleInteractWithBambooTray(this))
            {

                EventBus.SendMessage<PickUpIngredientByTray>(new PickUpIngredientByTray());
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
                    EventBus.SendMessage<InteractWithShopItemEvent>(new InteractWithShopItemEvent(shopItem));
                }
            }
        }
        else if (hit.collider.TryGetComponent<Customer>(out var customer))
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
        base.InteractWith(hit, pickupAndDropHandler);
    }

    private IEnumerator WaitForIngredientPickedUpByTray(Ingredient ingredient)
    {
        yield return new WaitUntil(() => ingredient.isMoveToSurfaceCompleted);
        EventBus.SendMessage<ItemPickedUpComplete>(new ItemPickedUpComplete());
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
