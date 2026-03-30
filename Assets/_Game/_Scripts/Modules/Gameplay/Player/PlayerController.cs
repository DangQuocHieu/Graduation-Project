using DQHieu.Framework;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public PickupAndDropHandler pickupAndDropHandler;
    public FirstPersonController firstPersonController;

    void OnEnable()
    {
        EventBus.Subcribe<PourOilIntoPan>(HandlePourOilIntoPanMessage);
        EventBus.Subcribe<PourOilIntoPanComplete>(HandlePourOilIntoPanCompleteEvent);
    }

    void OnDisable()
    {
        EventBus.UnSubcribe<PourOilIntoPan>(HandlePourOilIntoPanMessage);
        EventBus.UnSubcribe<PourOilIntoPanComplete>(HandlePourOilIntoPanCompleteEvent);
    }

    public void HandlePourOilIntoPanMessage(PourOilIntoPan message)
    {
        DisableMovement();
        DisablePickup();
    }

    public void HandlePourOilIntoPanCompleteEvent(PourOilIntoPanComplete evt)
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
