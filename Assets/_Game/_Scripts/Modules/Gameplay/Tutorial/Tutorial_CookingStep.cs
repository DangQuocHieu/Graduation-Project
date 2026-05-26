using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DQHieu.Framework;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
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
    [TabGroup("References")]
    public StoveSwitch stoveSwitch;
    [TabGroup("References")]
    public CuttingBoard cuttingBoard;
    [TabGroup("References")]
    public Customer sampleCustomer;

    [TabGroup("GrabbableObject Spawner")]
    public GrabbableObjectSpawner traySpawner;
    [TabGroup("GrabbableObject Spawner")]
    public GrabbableObjectSpawner fryingPanSpawner;
    [TabGroup("GrabbableObject Spawner")]
    public GrabbableObjectSpawner bowlSpawner;
    [TabGroup("GrabbableObject Spawner")]
    public ShopItem tofuShopItem;
    [TabGroup("GrabbableObject Spawner")]
    public ShopItem cucumberShopItem;
    [TabGroup("GrabbableObject Spawner")]
    public ShopItem riceNoodleShopItem;
    [TabGroup("GrabbableObject Spawner")]
    public ShopItem herbShopItem;

    [TabGroup("Object Instruction Arrow")]
    public GameObject riceNoodleInstructionArrow;
    [TabGroup("Object Instruction Arrow")]
    public GameObject herbInstructionArrow;
    [TabGroup("Object Instruction Arrow")]
    public GameObject putPanOnStoveArrow;
    [TabGroup("Object Instruction Arrow")]
    public GameObject oilBottleArrow;
    [TabGroup("Object Instruction Arrow")]
    public GameObject tofuInstructionArrow;
    [TabGroup("Object Instruction Arrow")]
    public GameObject stoveSwitchInstructionArrow;
    [TabGroup("Object Instruction Arrow")]
    public GameObject cucumberInstructionArrow;
    [TabGroup("Object Instruction Arrow")]
    public GameObject knifeInstructionArrow;
    [TabGroup("Object Instruction Arrow")]
    public GameObject cuttingBoardInstructionArrow;
    [TabGroup("Object Instruction Arrow")]
    public GameObject sauceBottleInstructionArrow;
    [TabGroup("Object Instruction Arrow")]
    public GameObject paymentInstructionArrow;


    [TabGroup("Tutorial Arrow")]
    public TutorialArrow moveToFridgeArrow;
    [TabGroup("Tutorial Arrow")]
    public TutorialArrow dropTrayArrow;
    [TabGroup("Tutorial Arrow")]
    public TutorialArrow moveToStoveArrow;
    [TabGroup("Tutorial Arrow")]
    public TutorialArrow moveToCuttingBoardArrow;
    [TabGroup("Tutorial Arrow")]
    public TutorialArrow paymentTutorialArrow;


    [TabGroup("Runtime Tracking")]
    public BambooTray currentBambooTray;
    [TabGroup("Runtime Tracking")]
    public FryingPan currentFryingPan;
    [TabGroup("Runtime Tracking")]
    public Ingredient currentTofuObject;
    [TabGroup("Runtime Tracking")]
    public Ingredient currentCucumberObject;
    protected override void OnDisable()
    {
        base.OnDisable();
    }

    public override IEnumerator ExecuteStep()
    {
        yield return null;
        pickupAndDropHandler.SetBlockPickup(false);
        pickupAndDropHandler.SetBlockDrop(true);
        ShowDialogueBox("Hold SHIFT to crouch");
        yield return new WaitUntil(() => Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift));

        // 1. Pickup Bamboo Tray (with Aim step & lock input)
        yield return WaitForAimAndPickup<BambooTray>(
            "Find bamboo tray",
            "Press LEFT CLICK to pick up the bamboo tray",
            () => hoverController.currentHoveredObject != null &&
                  hoverController.currentHoveredObject.TryGetComponent<GrabbableObjectSpawner>(out var spawner) &&
                  spawner.objectType == GrabbableObjectType.BambooTray,
            () => pickupAndDropHandler._objectInHand as BambooTray,
            () => pickupAndDropHandler.SetBlockPickup(false)
        );
        currentBambooTray = pickupAndDropHandler._objectInHand as BambooTray;
        traySpawner.enabled = false;

        yield return WaitForReachTutorialArrowPosition(moveToFridgeArrow, "Move to the fridge");

        // 2. Open fridge doors
        yield return WaitForInteraction("Press LEFT CLICK to open left door", null, () => leftFridgeDoor.isOpened);
        yield return WaitForInteraction("Open right door", null, () => rightFridgeDoor.isOpened);

        // 3. Add ingredients
        yield return WaitForIngredientAdded(
            "Press LEFT CLICK to add rice noodles to your tray",
            riceNoodleInstructionArrow,
            IngredientType.RiceNoodle,
            riceNoodleShopItem
        );

        yield return WaitForIngredientAdded(
            "Press LEFT CLICK to add herbs to your tray",
            herbInstructionArrow,
            IngredientType.Herb,
            herbShopItem
        );

        yield return WaitForReachTutorialArrowPosition(dropTrayArrow, "Go to the green arrow");

        // 4. Put tray down
        yield return WaitForInteraction(
            "Press LEFT CLICK to put the tray down",
            null,
            () => pickupAndDropHandler._objectInHand == null,
            () => pickupAndDropHandler.SetBlockDrop(false)
        );

        // 5. Pickup Frying Pan (with Aim step & lock input)
        yield return WaitForAimAndPickup<FryingPan>(
            "Aim at the frying pan",
            "Find and pickup frying pan",
            () => hoverController.currentHoveredObject != null &&
                  hoverController.currentHoveredObject.TryGetComponent<GrabbableObjectSpawner>(out var spawner) &&
                  spawner.objectType == GrabbableObjectType.FryingPan,
            () => pickupAndDropHandler._objectInHand as FryingPan
        );
        currentFryingPan = pickupAndDropHandler._objectInHand as FryingPan;
        pickupAndDropHandler.SetBlockDrop(true);
        fryingPanSpawner.enabled = false;

        yield return WaitForReachTutorialArrowPosition(moveToStoveArrow, "Go to the stove");

        // 6. Place pan on the stove
        yield return WaitForInteraction(
            "Press LEFT CLICK to place pan on the stove",
            putPanOnStoveArrow,
            () => currentFryingPan.attachedCookingZone != null,
            () => pickupAndDropHandler.SetBlockDrop(false)
        );

        // 7. Pickup Cooking Oil Bottle (with Aim step & lock input)
        pickupAndDropHandler.SetBlockDrop(true);
        oilBottleArrow.gameObject.SetActive(true);
        yield return WaitForAimAndPickup<CookingOilBottle>(
            "Aim at the cooking oil bottle",
            "Pick up the cooking oil bottle",
            () => hoverController.currentGrabbableObject is CookingOilBottle,
            () => pickupAndDropHandler._objectInHand as CookingOilBottle
        );
        oilBottleArrow.gameObject.SetActive(false);

        // 8. Pour oil into the pan
        yield return WaitForInteraction(
            "Press LEFT CLICK to pour oil into the pan",
            putPanOnStoveArrow,
            () => currentFryingPan.containCookingOil
        );
        yield return new WaitUntil(() => currentFryingPan.fillCompleted);
        yield return new WaitUntil(() => pickupAndDropHandler._objectInHand != null);

        // 9. Put oil bottle down
        yield return WaitForInteraction(
            "Put the cooking oil bottle down",
            null,
            () => pickupAndDropHandler._objectInHand == null,
            () => {
                pickupAndDropHandler.SetBlockDrop(false);
                oilBottleArrow.gameObject.SetActive(false);
            }
        );

        // 10. Pickup Tofu from fridge (with Aim step & lock input)
        tofuInstructionArrow.gameObject.SetActive(true);
        moveToFridgeArrow.gameObject.SetActive(true);
        yield return WaitForAimAndPickup<GrabbableObject>(
            "Aim at the tofu in the fridge",
            "Pickup tofu from fridge",
            () => hoverController.currentHoveredObject != null &&
                  hoverController.currentHoveredObject.TryGetComponent<ShopItem>(out var shopItem) &&
                  shopItem == tofuShopItem,
            () => {
                if (pickupAndDropHandler._objectInHand != null && 
                    pickupAndDropHandler._objectInHand.TryGetComponent<Ingredient>(out var ing) && 
                    ing.ingredientType == IngredientType.Tofu)
                {
                    currentTofuObject = ing;
                    currentTofuObject.cookableObject.totalTimeToBurn /= 2;
                    return pickupAndDropHandler._objectInHand;
                }
                return null;
            }
        );
        currentTofuObject.cookableObject.isTutorialObject = true;
        tofuInstructionArrow.gameObject.SetActive(false);
        tofuShopItem.enabled = false;

        // 11. Drop Tofu into pan
        yield return WaitForInteraction(
            "Drop tofu into the pan",
            putPanOnStoveArrow,
            () => pickupAndDropHandler._objectInHand == null && 
                  currentFryingPan.placeableSurface.ingredientContainer.containedItems.Count != 0 &&
                  currentTofuObject != null &&
                  currentTofuObject.GetComponent<Collider>() != null &&
                  currentFryingPan.oilContainer.liquid.GetComponent<Collider>() != null &&
                  currentTofuObject.GetComponent<Collider>().bounds.Intersects(currentFryingPan.oilContainer.liquid.GetComponent<Collider>().bounds),
            () => {
                moveToStoveArrow.gameObject.SetActive(true);
            }
        );
        pickupAndDropHandler.SetBlockPickup(true);

        // 12. Turn on stove
        yield return WaitForInteraction(
            "Turn on the stove",
            stoveSwitchInstructionArrow,
            () => stoveSwitch.isOn
        );

        // 13. Aim tofu and cook it
        ShowDialogueBox("Aim at the tofu");
        yield return new WaitUntil(() => hoverController.currentHoveredObject != null && hoverController.currentHoveredObject.gameObject == currentTofuObject.gameObject);
        ShowDialogueBox("Wait until the tofu is cooked");
        playerController.kccManager.SetAllInputBlocked(true);
        yield return new WaitUntil(() => currentTofuObject.cookableObject.cookProgress >= currentTofuObject.cookableObject.perfectCookLevel);

        // 14. Take Tofu out of frying pan (with Aim step & lock input)
        yield return WaitForAimAndPickup<Ingredient>(
            "Aim at the tofu in the frying pan",
            "Take the tofu out of the frying pan",
            () => hoverController.currentGrabbableObject == currentTofuObject,
            () => (pickupAndDropHandler._objectInHand != null && pickupAndDropHandler._objectInHand.gameObject == currentTofuObject.gameObject) ? currentTofuObject : null,
            () => {
                playerController.kccManager.SetAllInputBlocked(false);
                pickupAndDropHandler.SetBlockPickup(false);
            }
        );

        // 15. Place Tofu on cutting board
        yield return WaitForInteraction(
            "Place the tofu on the cutting board",
            cuttingBoardInstructionArrow,
            () => currentTofuObject.attachedIngredientContainer != null && currentTofuObject.attachedIngredientContainer.gameObject == cuttingBoard.PlaceableSurface.ingredientContainer.gameObject,
            () => moveToCuttingBoardArrow.gameObject.SetActive(true)
        );

        // 16. Pickup Cucumber from fridge (with Aim step & lock input)
        cucumberInstructionArrow.gameObject.SetActive(true);
        moveToFridgeArrow.gameObject.SetActive(true);
        yield return WaitForAimAndPickup<GrabbableObject>(
            "Aim at the cucumber in the fridge",
            "Pickup cucumber from fridge",
            () => hoverController.currentHoveredObject != null &&
                  hoverController.currentHoveredObject.TryGetComponent<ShopItem>(out var shopItem) &&
                  shopItem == cucumberShopItem,
            () => {
                if (pickupAndDropHandler._objectInHand != null && 
                    pickupAndDropHandler._objectInHand.TryGetComponent<Ingredient>(out var ing) && 
                    ing.ingredientType == IngredientType.Cucumber)
                {
                    currentCucumberObject = ing;
                    return pickupAndDropHandler._objectInHand;
                }
                return null;
            }
        );
        cucumberInstructionArrow.gameObject.SetActive(false);
        cucumberShopItem.enabled = false;

        // 17. Place Cucumber on cutting board
        yield return WaitForInteraction(
            "Place the cucumber on the cutting board",
            null,
            () => currentCucumberObject.attachedIngredientContainer != null && currentCucumberObject.attachedIngredientContainer.gameObject == cuttingBoard.PlaceableSurface.ingredientContainer.gameObject,
            () => moveToCuttingBoardArrow.gameObject.SetActive(true)
        );

        // 18. Pickup Knife (with Aim step & lock input)
        knifeInstructionArrow.gameObject.SetActive(true);
        yield return WaitForAimAndPickup<KnifeObject>(
            "Aim at the knife",
            "Pickup knife",
            () => hoverController.currentGrabbableObject is KnifeObject,
            () => pickupAndDropHandler._objectInHand as KnifeObject
        );
        pickupAndDropHandler.SetBlockDrop(true);
        knifeInstructionArrow.gameObject.SetActive(false);

        // 19. Cut Tofu
        yield return WaitForCutIngredient("Aim at the tofu", "Press LEFT CLICK to cut", currentTofuObject, () => currentTofuObject == null);
        playerController.kccManager.SetLookInputBlocked(false);

        // 20. Cut Cucumber
        yield return WaitForCutIngredient("Aim at the cucumber", "Press LEFT CLICK to cut", currentCucumberObject, () => currentCucumberObject == null);
        playerController.kccManager.SetAllInputBlocked(false);

        // 21. Drop Knife
        yield return WaitForInteraction(
            "Drop the knife",
            null,
            () => pickupAndDropHandler._objectInHand == null,
            () => pickupAndDropHandler.SetBlockDrop(false)
        );

        // 22. Pickup Cutting Board and interact with Bamboo Tray
        yield return WaitForAimAndPickup<CuttingBoard>(
            "Aim at the cutting board",
            "Pickup the cutting board",
            () => hoverController.currentGrabbableObject is CuttingBoard,
            () => pickupAndDropHandler._objectInHand as CuttingBoard
        );

        ShowDialogueBox("Aim at the bamboo tray");
        bool hasBeenTrayAimed = false;
        bool interactionCompleted = false;

        while (!interactionCompleted)
        {
            bool isCurrentlyHovered = hoverController.currentGrabbableObject is BambooTray;

            if (isCurrentlyHovered)
            {
                if (!hasBeenTrayAimed)
                {
                    hasBeenTrayAimed = true;
                    ShowDialogueBox("Press LEFT CLICK to place ingredients onto the tray");
                }

                if (Input.GetMouseButtonDown(0))
                {
                    interactionCompleted = true;
                }
            }
            else
            {
                if (hasBeenTrayAimed)
                {
                    hasBeenTrayAimed = false;
                    ShowDialogueBox("Aim at the bamboo tray"); 
                }
            }

            yield return null;
        }

        // 23. Drop the Cutting Board down
        yield return WaitForInteraction(
            "Drop the cutting board down",
            null,
            () => pickupAndDropHandler._objectInHand == null,
            () => pickupAndDropHandler.SetBlockDrop(false)
        );

        // 23a. Aim and pickup Sauce Bowl from spawner (with Aim step & lock input)
        yield return WaitForAimAndPickup<SauceBowl>(
            "Aim at the bowls",
            "Press LEFT CLICK to pick up a sauce bowl",
            () => hoverController.currentHoveredObject != null &&
                  hoverController.currentHoveredObject.TryGetComponent<GrabbableObjectSpawner>(out var spawner) &&
                  spawner.objectType == GrabbableObjectType.SauceBowl,
            () => pickupAndDropHandler._objectInHand as SauceBowl
        );

        // 23b. Place the bowl on the bamboo tray
        ShowDialogueBox("Aim at the bamboo tray");
        bool hasBeenBowlTrayAimed = false;

        while (currentBambooTray.attachedBowl == null)
        {
            bool isCurrentlyHovered = hoverController.currentGrabbableObject is BambooTray;

            if (isCurrentlyHovered)
            {
                if (!hasBeenBowlTrayAimed)
                {
                    hasBeenBowlTrayAimed = true;
                    ShowDialogueBox("Press LEFT CLICK to place the bowl on the tray");
                }
            }
            else
            {
                if (hasBeenBowlTrayAimed)
                {
                    hasBeenBowlTrayAimed = false;
                    ShowDialogueBox("Aim at the bamboo tray");
                }
            }

            yield return null;
        }

        // 23c. Pickup Shrimp Paste Bottle (with Aim step & lock input)
        sauceBottleInstructionArrow.SetActive(true);
        yield return WaitForAimAndPickup<SauceBottle>(
            "Aim at the shrimp paste bottle",
            "Press LEFT CLICK to pick up the shrimp paste bottle",
            () => hoverController.currentGrabbableObject is SauceBottle bottle && bottle.sauceType == SauceType.ShrimpPaste,
            () => pickupAndDropHandler._objectInHand as SauceBottle
        );
        sauceBottleInstructionArrow.SetActive(false);

        // 23d. Pour Shrimp Paste into the Sauce Bowl
        ShowDialogueBox("Aim at the sauce bowl on the tray");
        bool hasBeenBowlAimed = false;
        bool pourStarted = false;

        while (currentBambooTray.attachedBowl != null && (!currentBambooTray.attachedBowl.fill || pickupAndDropHandler._objectInHand == null))
        {
            if (currentBambooTray.attachedBowl.containSauce)
            {
                pourStarted = true;
            }

            bool isCurrentlyHovered = hoverController.currentGrabbableObject == currentBambooTray.attachedBowl;

            if (isCurrentlyHovered && !pourStarted)
            {
                if (!hasBeenBowlAimed)
                {
                    hasBeenBowlAimed = true;
                    ShowDialogueBox("Press LEFT CLICK to pour the shrimp paste");
                }
            }
            else
            {
                if (hasBeenBowlAimed && !pourStarted)
                {
                    hasBeenBowlAimed = false;
                    ShowDialogueBox("Aim at the sauce bowl on the tray");
                }

                if (pourStarted)
                {
                    ShowDialogueBox("Pouring shrimp paste...");
                }
            }

            yield return null;
        }

        // 23e. Drop the Shrimp Paste Bottle down
        yield return WaitForInteraction(
            "Drop the shrimp paste bottle down",
            null,
            () => pickupAndDropHandler._objectInHand == null,
            () => pickupAndDropHandler.SetBlockDrop(false)
        );

        // 24. Pickup Bamboo Tray (with Aim step & lock input)
        yield return WaitForAimAndPickup<BambooTray>(
            "Aim at the bamboo tray",
            "Pickup the bamboo tray",
            () => hoverController.currentGrabbableObject is BambooTray,
            () => pickupAndDropHandler._objectInHand as BambooTray
        );

        // 25. Serve to the customer
        ShowDialogueBox("Press LEFT CLICK on the customer to serve the food");
        yield return new WaitUntil(() => sampleCustomer.foodServed);

        // 27. Wait for customer to pay
        ShowDialogueBox("Waiting for the customer to pay...");
        yield return new WaitUntil(() => sampleCustomer.currentState == CustomerState.Paying && sampleCustomer.paymentVisual.gameObject.activeSelf);
        if (paymentInstructionArrow != null) paymentInstructionArrow.SetActive(true);

        // 28. Aim and collect payment
        ShowDialogueBox("Aim at the cash on the counter");
        bool hasBeenCashAimed = false;

        while (sampleCustomer.currentState == CustomerState.Paying)
        {
            Ray ray = hoverController.mainCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            bool isCurrentlyHovered = Physics.Raycast(ray, out RaycastHit hit, 3f) && 
                                      hit.collider.TryGetComponent<CashObject>(out var cash) && 
                                      cash.attachedCustomer == sampleCustomer;

            if (isCurrentlyHovered)
            {
                if (!hasBeenCashAimed)
                {
                    hasBeenCashAimed = true;
                    ShowDialogueBox("Press LEFT CLICK to collect the payment");
                }
            }
            else
            {
                if (hasBeenCashAimed)
                {
                    hasBeenCashAimed = false;
                    ShowDialogueBox("Aim at the cash on the counter");
                }
            }

            yield return null;
        }

        if (paymentInstructionArrow != null) paymentInstructionArrow.SetActive(false);

        if (playerController != null && playerController.kccManager != null)
        {
            playerController.kccManager.SetMoveInputBlocked(false);
            playerController.kccManager.SetLookInputBlocked(false);
        }
    }

    private IEnumerator WaitForInteraction(string text, GameObject arrow, System.Func<bool> condition, System.Action onStart = null)
    {
        ShowDialogueBox(text);
        if (arrow != null) arrow.SetActive(true);
        onStart?.Invoke();
        yield return new WaitUntil(condition);
        if (arrow != null) arrow.SetActive(false);
    }

    private IEnumerator WaitForIngredientAdded(string text, GameObject instructionArrow, IngredientType ingredientType, ShopItem shopItem)
    {
        ShowDialogueBox(text);
        instructionArrow.SetActive(true);
        yield return new WaitUntil(() => currentBambooTray.IsIngredientEnough(ingredientType));
        instructionArrow.SetActive(false);
        shopItem.enabled = false;
    }

    private IEnumerator WaitForCutIngredient(string aimText, string cutText, Ingredient ingredient, System.Func<bool> isIngredientDestroyed)
    {
        ShowDialogueBox(aimText);
        yield return new WaitUntil(() => 
        {
            try
            {
                if (ingredient == null) return false;
                var ingredientGO = ingredient.gameObject;
                if (ingredientGO == null) return false;

                if (hoverController.currentHoveredObject == null) return false;
                var hoveredGO = hoverController.currentHoveredObject.gameObject;
                if (hoveredGO == null) return false;

                return hoveredGO == ingredientGO;
            }
            catch (System.Exception)
            {
                return false;
            }
        });
        playerController.kccManager.SetAllInputBlocked(true);
        ShowDialogueBox(cutText);
        yield return new WaitUntil(() =>
        {
            try
            {
                return isIngredientDestroyed() || ingredient == null;
            }
            catch (System.Exception)
            {
                return true;
            }
        });
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
