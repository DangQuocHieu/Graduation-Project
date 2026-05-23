using System.Collections;
using System.Collections.Generic;
using CoreGame.Movement;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;


public class TutorialManager : MonoBehaviour
{
    public List<TutorialStep> tutorialSteps = new();

    public void StartTutorial()
    {
        StartCoroutine(TutorialCoroutine());
    }

    private IEnumerator TutorialCoroutine()
    {
        foreach(var step in tutorialSteps)
        {
            if(step.gameObject.activeSelf)
            yield return step.ExecuteStep();
        }   
    }
}
