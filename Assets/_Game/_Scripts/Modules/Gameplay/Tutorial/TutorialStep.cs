using System.Collections;
using System.Collections.Generic;
using CoreGame.Movement;
using UnityEngine;

public abstract class TutorialStep : MonoBehaviour
{
    public PlayerController playerController;
    public TutorialScreen tutorialScreen;
    public TutorialDialogueBox tutorialDialogueBox;
    public List<string> tutorialStrings = new();
    public int currentStringIndex = 0;


    void Awake()
    {
        tutorialDialogueBox = tutorialScreen.tutorialDialogueBox;
    }

    protected virtual void OnDisable()
    {
        if (playerController != null && playerController.kccManager != null)
        {
            playerController.kccManager.SetMoveInputBlocked(false);
            playerController.kccManager.SetLookInputBlocked(false);
        }
    }

    public void ShowDialogueBox(string text)
    {
        tutorialDialogueBox.ShowDialogueBox(text);
    }

    public IEnumerator WaitForReachTutorialArrowPosition(TutorialArrow arrow, string text)
    {
        arrow.gameObject.SetActive(true);
        ShowDialogueBox(text);
        yield return new WaitUntil(() => !arrow.gameObject.activeSelf);
    }

    public IEnumerator WaitForAimAndPickup<T>(
        string aimText, 
        string pickupText, 
        System.Func<bool> isHoveringCondition, 
        System.Func<T> getPickedUpObject,
        System.Action onAimed = null) where T : GrabbableObject
    {
        ShowDialogueBox(aimText);

        bool hasBeenAimed = false;
        PickupAndDropHandler pickupAndDropHandler = FindObjectOfType<PickupAndDropHandler>();

        while (getPickedUpObject() == null)
        {
            bool isCurrentlyHovered = isHoveringCondition();

            if (pickupAndDropHandler != null)
            {
                pickupAndDropHandler.SetBlockPickup(!isCurrentlyHovered);
            }

            if (isCurrentlyHovered)
            {
                if (!hasBeenAimed)
                {
                    hasBeenAimed = true;
                    ShowDialogueBox(pickupText);
                    onAimed?.Invoke();
                }
            }
            else
            {
                if (hasBeenAimed)
                {
                    hasBeenAimed = false;
                    ShowDialogueBox(aimText);
                }
            }

            yield return null;
        }
    }

    public abstract IEnumerator ExecuteStep();

}
