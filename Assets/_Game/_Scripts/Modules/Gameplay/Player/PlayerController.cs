using DQHieu.Framework;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public PickupAndDropHandler pickupAndDropHandler;
    public FirstPersonController firstPersonController;

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
        DisableMovement();
        DisablePickup();
    }

    public void HandlePourLiquidComplete(PourLiquidComplete evt)
    {
        EnableMovement();
        EnablePickup();
    }

    public void DisableMovement()
    {
        firstPersonController.enabled = false;
    }

    public void EnableMovement()
    {
        firstPersonController.enabled = true;
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
