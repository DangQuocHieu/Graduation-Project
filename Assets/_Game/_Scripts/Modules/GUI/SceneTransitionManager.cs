using DG.Tweening;
using DQHieu.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneTransitionManager : PersistentSingleton<SceneTransitionManager>
{
    public RectTransform circleRect;

    public void LoadScene(string sceneName)
    {
        StartCoroutine(TransitionRoutine(sceneName));
    }

    private IEnumerator TransitionRoutine(string sceneName)
    {
        bool animationDone = false;
        circleRect.DOScale(10f, 1f).SetEase(Ease.InCubic).SetUpdate(true).OnComplete(() => {
            animationDone = true;
        });
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false; 
        while (!animationDone || op.progress < 0.9f)
        {
            yield return null;
        }

        op.allowSceneActivation = true;
        yield return new WaitForSeconds(0.5f); //fake loading

        circleRect.DOScale(0f, 1f).SetEase(Ease.OutCubic).SetUpdate(true);
    }
}