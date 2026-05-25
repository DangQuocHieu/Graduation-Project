using System.Collections;
using DQHieu.Framework;
using TMPro;
using Unity.Profiling;
using UnityEngine;

public class Tutorial_OrderStep : TutorialStep
{
    public Customer sampleCustomer;
    public TutorialArrow orderTutorialArrow;
    public GameObject orderTutorialPointer;
    private bool orderButtonClicked = false;

    void OnEnable()
    {
        EventBus.Subcribe<CustomerOrderButtonClicked>(HandleCustomerOrderButtonClicked);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        EventBus.UnSubcribe<CustomerOrderButtonClicked>(HandleCustomerOrderButtonClicked);
    }

    public override IEnumerator ExecuteStep()
    {
        ShowDialogueBox("Waiting for the customer to enter");
        playerController.kccManager.SetMoveInputBlocked(true);
        sampleCustomer.gameObject.SetActive(true);
        yield return new WaitUntil(() => sampleCustomer.attachedChairObject != null);
        ShowDialogueBox("Check the customer's order");
        playerController.kccManager.SetMoveInputBlocked(false);
        orderTutorialArrow.gameObject.SetActive(true);
        yield return new WaitUntil(() => !orderTutorialArrow.gameObject.activeSelf);
        ShowDialogueBox("Press LEFT CLICK to the customer's order");
        orderTutorialPointer.gameObject.SetActive(true); 
        yield return new WaitUntil(() => orderButtonClicked);
        orderTutorialPointer.gameObject.SetActive(false);
        playerController.kccManager.SetAllInputBlocked(true);
        CursorHelper.ShowCursor();
        tutorialScreen.orderScreen.ToggleScreen(true);
        yield return new WaitUntil(() => tutorialScreen.orderScreen.acceptButtonClicked);
        playerController.kccManager.SetAllInputBlocked(false);
        CursorHelper.HideCursor();
    }

    private void HandleCustomerOrderButtonClicked(CustomerOrderButtonClicked evt)
    {
        orderButtonClicked = true;
    }
}
