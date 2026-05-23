using CoreGame.Movement;
using DQHieu.Framework;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public PickupAndDropHandler pickupAndDropHandler;
    public KCCManager kccManager;


    void OnEnable()
    {
        EventBus.Subcribe<PourLiquid>(HandlePourLiquidMessage);
        EventBus.Subcribe<PourLiquidComplete>(HandlePourLiquidComplete);
        EventBus.Subcribe<PurchaseShopItemSucess>(HandlePurchaseShopItemSuccessEvent);
        EventBus.Subcribe<ItemPickedUpComplete>(HandlePurchaseShopItemPickedUp);
        EventBus.Subcribe<PickUpIngredientByTray>(HandlePickUpIngredientByTray);

    }

    void OnDisable()
    {
        EventBus.UnSubcribe<PourLiquid>(HandlePourLiquidMessage);
        EventBus.UnSubcribe<PourLiquidComplete>(HandlePourLiquidComplete);
        EventBus.UnSubcribe<PurchaseShopItemSucess>(HandlePurchaseShopItemSuccessEvent);
        EventBus.UnSubcribe<ItemPickedUpComplete>(HandlePurchaseShopItemPickedUp);
        EventBus.UnSubcribe<PickUpIngredientByTray>(HandlePickUpIngredientByTray);

    }

    public void HandlePourLiquidMessage(PourLiquid message)
    {
        kccManager.SetAllInputBlocked(true);
        DisablePickup();
    }

    public void HandlePourLiquidComplete(PourLiquidComplete evt)
    {
        kccManager.SetAllInputBlocked(false);
        EnablePickup();
    }


    public void DisablePickup()
    {
        pickupAndDropHandler.enabled = false;
    }

    public void EnablePickup()
    {
        pickupAndDropHandler.enabled = true;
    }

    private void HandlePurchaseShopItemSuccessEvent(PurchaseShopItemSucess evt)
    {
        kccManager.SetAllInputBlocked(true);
    }

    private void HandlePurchaseShopItemPickedUp(ItemPickedUpComplete evt)
    {
        kccManager.SetAllInputBlocked(false);
    }

    private void HandlePickUpIngredientByTray(PickUpIngredientByTray evt)
    {
        kccManager.SetMoveInputBlocked(false);
    }

}
