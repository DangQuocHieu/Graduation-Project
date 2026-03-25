
using System.Collections;
using UnityEngine;

public class CookingZone : MonoBehaviour
{
    public bool turnOn = false;
    public CanvasGroup electricImage;
    public float toggleDuration = 1f;
    public float timer = 0f;
    private Coroutine toggleCoroutine;

    public void Toggle()
    {
        if(toggleCoroutine != null)
        {
            StopCoroutine(toggleCoroutine);
            toggleCoroutine = null;
        }
        turnOn = !turnOn;
        if(turnOn)
        {
            toggleCoroutine = StartCoroutine(TurnOnCoroutine());
        }
        else
        {
            toggleCoroutine = StartCoroutine(TurnOffCoroutine());
        }
    }
    private IEnumerator TurnOnCoroutine()
    {
        while(timer < toggleDuration)
        {
            timer += Time.deltaTime;
            electricImage.alpha = timer / toggleDuration;
            yield return null;
        }
        timer = toggleDuration;
    }

    private IEnumerator TurnOffCoroutine()
    {
        while(timer > 0f)
        {
            timer -= Time.deltaTime;
            electricImage.alpha = timer / toggleDuration;
            yield return null;
        }
        timer = 0f;
    }


}
