using System.Collections;
using DQHieu.Framework;
using Sirenix.OdinInspector;
using UnityEngine;

public class Tutorial_CookingStep : TutorialStep
{
    [TabGroup("References")]
    public HoverController hoverController;
    [TabGroup("References")]
    public PickupAndDropHandler pickupAndDropHandler;

    [TabGroup("References")]
    public HingedObject leftFridgeDoor;
    [TabGroup("References")]
    public HingedObject rightFridgeDoor;

    [TabGroup("GrabbableObject Spawner")]
    public GrabbableObjectSpawner traySpawner;
    [TabGroup("GrabbableObject Spawner")]
    public GrabbableObjectSpawner fryingPanSpawner;
    [TabGroup("GrabbableObject Spawner")]
    public GrabbableObjectSpawner bowlSpawner;

    [TabGroup("Object Instruction Arrow")]
    public GameObject riceNoodleInstructionArrow;
    [TabGroup("Object Instruction Arrow")]
    public GameObject herbInstructionArrow;
    [TabGroup("Object Instruction Arrow")]
    public GameObject putPanOnStoveArrow;
    [TabGroup("Object Instruction Arrow")]
    public GameObject oilBottleArrow;

    [TabGroup("Tutorial Arrow")]
    public TutorialArrow moveToFridgeArrow;
    [TabGroup("Tutorial Arrow")]
    public TutorialArrow dropTrayArrow;
    [TabGroup("Tutorial Arrow")]
    public TutorialArrow moveToStoveArrow;

    [TabGroup("Runtime Tracking")]
    public BambooTray currentBambooTray;
    [TabGroup("Runtime Tracking")]
    public FryingPan currentFryingPan;
    [TabGroup("Runtime Tracking")]
    void OnEnable()
    {

    }

    void OnDisable()
    {

    }
    public override IEnumerator ExecuteStep()
    {
        yield return null;
        pickupAndDropHandler.SetBlockDrop(true);
        ShowDialogueBox();
        yield return new WaitUntil(() => Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift));
        ShowDialogueBox();
        yield return WaitForObjectHovered(GrabbableObjectType.BambooTray);
        ShowDialogueBox();
        pickupAndDropHandler.SetBlockPickup(false);
        yield return new WaitUntil(() => pickupAndDropHandler._objectInHand != null && pickupAndDropHandler._objectInHand is BambooTray bambooTray);
        currentBambooTray = pickupAndDropHandler._objectInHand as BambooTray;
        traySpawner.enabled = false;

        yield return WaitForReachTutorialArrowPosition(moveToFridgeArrow);

        ShowDialogueBox();
        yield return new WaitUntil(() => leftFridgeDoor.isOpened);
        ShowDialogueBox();
        yield return new WaitUntil(() => rightFridgeDoor.isOpened);

        ShowDialogueBox();
        riceNoodleInstructionArrow.gameObject.SetActive(true);
        yield return new WaitUntil(() => currentBambooTray.IsIngredientEnough(IngredientType.RiceNoodle));
        riceNoodleInstructionArrow.gameObject.SetActive(false);

        ShowDialogueBox();
        herbInstructionArrow.gameObject.SetActive(true);
        yield return new WaitUntil(() => currentBambooTray.IsIngredientEnough(IngredientType.Herb));
        herbInstructionArrow.gameObject.SetActive(false);

        yield return WaitForReachTutorialArrowPosition(dropTrayArrow);

        pickupAndDropHandler.SetBlockDrop(false);
        ShowDialogueBox();
        yield return new WaitUntil(() => pickupAndDropHandler._objectInHand == null);

        ShowDialogueBox();
        yield return new WaitUntil(() => pickupAndDropHandler._objectInHand != null && pickupAndDropHandler._objectInHand is FryingPan fryingPan);
        currentFryingPan = pickupAndDropHandler._objectInHand as FryingPan;
        pickupAndDropHandler.SetBlockDrop(true);
        fryingPanSpawner.enabled = false;

        yield return WaitForReachTutorialArrowPosition(moveToStoveArrow);

        ShowDialogueBox();
        putPanOnStoveArrow.gameObject.SetActive(true);
        pickupAndDropHandler.SetBlockDrop(false);
        yield return new WaitUntil(() => currentFryingPan.attachedCookingZone != null);
        putPanOnStoveArrow.gameObject.SetActive(false);

        ShowDialogueBox();
        pickupAndDropHandler.SetBlockDrop(true);
        oilBottleArrow.gameObject.SetActive(true);
        yield return new WaitUntil(() => pickupAndDropHandler._objectInHand != null && pickupAndDropHandler._objectInHand is CookingOilBottle cookingOilBottle);
        oilBottleArrow.gameObject.SetActive(false);

        ShowDialogueBox();
        putPanOnStoveArrow.gameObject.SetActive(true);
        yield return new WaitUntil(() => currentFryingPan.containCookingOil);
        putPanOnStoveArrow.gameObject.SetActive(false);
        yield return new WaitUntil(() => currentFryingPan.fillCompleted);

        ShowDialogueBox();
        pickupAndDropHandler.SetBlockDrop(false);
        yield return new WaitUntil(() => pickupAndDropHandler._objectInHand == null);


    }

    private IEnumerator WaitForObjectHovered(GrabbableObjectType type)
    {
        yield return new WaitUntil(
            () => hoverController.currentHoveredObject != null &&
                  hoverController.currentHoveredObject.TryGetComponent<GrabbableObjectSpawner>(out var spawner) &&
                  spawner.objectType == type
                   );
    }



}
