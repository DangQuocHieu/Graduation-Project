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

    public void ShowDialogueBox()
    {
        tutorialDialogueBox.ShowDialogueBox(tutorialStrings[currentStringIndex]);
        ++currentStringIndex;
    }

    public IEnumerator WaitForReachTutorialArrowPosition(TutorialArrow arrow)
    {
        arrow.gameObject.SetActive(true);
        ShowDialogueBox();
        yield return new WaitUntil(() => !arrow.gameObject.activeSelf);
    }

    public abstract IEnumerator ExecuteStep();

}
