using CoreGame.Movement;
using DQHieu.Framework;
using UnityEngine;

public class PlayerEventHandler : MonoBehaviour
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
        kccManager.BlockInput();
        DisablePickup();
    }

    public void HandlePourLiquidComplete(PourLiquidComplete evt)
    {
        kccManager.UnblockInput();
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
        kccManager.BlockInput();
    }

    private void HandlePurchaseShopItemPickedUp(ItemPickedUpComplete evt)
    {
        kccManager.UnblockInput();
    }

    private void HandlePickUpIngredientByTray(PickUpIngredientByTray evt)
    {
        kccManager.BlockInput();
    }

}
