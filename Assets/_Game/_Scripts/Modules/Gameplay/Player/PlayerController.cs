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

    }

    void OnDisable()
    {
        EventBus.UnSubcribe<PourLiquid>(HandlePourLiquidMessage);
        EventBus.UnSubcribe<PourLiquidComplete>(HandlePourLiquidComplete);

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


}
