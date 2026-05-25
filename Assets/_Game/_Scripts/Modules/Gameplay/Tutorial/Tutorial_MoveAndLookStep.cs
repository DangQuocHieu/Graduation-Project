using System.Collections;
using DQHieu.Framework;
using Sirenix.OdinInspector;
using UnityEngine;

public class Tutorial_MoveAndLookStep : TutorialStep
{
    [TabGroup("Tutorial Arrow")]
    public TutorialArrow tutorialArrow;
    public PickupAndDropHandler pickupAndDropHandler;
    protected override void OnDisable()
    {
        base.OnDisable();
    }
    public override IEnumerator ExecuteStep()
    {
        pickupAndDropHandler.SetBlockPickup(true);

        CursorHelper.ShowCursor();
        playerController.DisablePickup();
        playerController.kccManager.SetAllInputBlocked(true);
        tutorialScreen.moveAndLookScreen.ToggleScreen(true);
        yield return new WaitUntil(() => tutorialScreen.moveAndLookScreen.acceptButtonClicked);
        playerController.EnablePickup();
        playerController.kccManager.SetAllInputBlocked(false);
        CursorHelper.HideCursor();  
        tutorialArrow.gameObject.SetActive(true);
        ShowDialogueBox("Go to the green arrow");
        yield return new WaitUntil(() => !tutorialArrow.gameObject.activeSelf);
        tutorialArrow.gameObject.SetActive(false);
    }
}
