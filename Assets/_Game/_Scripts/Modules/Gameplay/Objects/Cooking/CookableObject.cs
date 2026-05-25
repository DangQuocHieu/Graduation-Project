using DG.Tweening;
using DQHieu.Framework.Audio;
using Sirenix.OdinInspector;
using UnityEngine;

public class CookableObject : MonoBehaviour
{
    private Material foodMat;
    public MeshRenderer meshRenderer;
    [Header("Cooking Stats")]
    public float cookProgress = 0f;
    public float perfectCookLevel = 100f;
    public float maxBurnLevel = 200f;
    public float totalTimeToBurn = 10f;
    public float perfectCookOffset = 10f;
    public float tasteScore;
    public bool isTutorialObject;

    [Header("Visual")]
    public Gradient cookingColors;
    public Tween cookingTween;

    [ShowInInspector] public float cookProgressSliderValue => cookProgress / perfectCookLevel;


    void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        foodMat = meshRenderer.material;
    }

    void Update()
    {
        GetTasteScore();
    }

    public void StartCooking()
    {
        if (cookingTween != null) return;
        float remainingPercentage = 1f - (cookProgress / maxBurnLevel);
        float timeRemaining = totalTimeToBurn * remainingPercentage;
        cookingTween = DOTween.To(() => cookProgress, x => cookProgress = x, maxBurnLevel, timeRemaining)
            .SetEase(Ease.Linear)
            .SetLink(gameObject)
            .OnUpdate(() =>
            {
                UpdateFoodColor();
                CheckTutorialLimit();
            });
    }

    private void CheckTutorialLimit()
    {
        if(isTutorialObject && cookProgress >= perfectCookLevel)
        {
            cookProgress = perfectCookLevel;
        }
    }

    public void PauseCooking()
    {
        cookingTween?.Kill();
        cookingTween = null;
    }

    public void ApplyCookableObject(CookableObject cookableObject)
    {
        cookProgress = cookableObject.cookProgress;
        UpdateFoodColor();
    }

    private void UpdateFoodColor()
    {
        float percentage = cookProgress / maxBurnLevel;
        foodMat.color = cookingColors.Evaluate(percentage);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<CookingOil>(out var cookingOil))
        {
            if (cookingOil.isHot)
            {
                StartCooking();
            }
        }
    }
    void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent<CookingOil>(out var cookingOil))
        {
            if (cookingOil.isHot)
            {
                StartCooking();
            }
            else
            {
                PauseCooking();
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<CookingOil>(out var cookingOil))
        {
            PauseCooking();
        }
    }

    public float GetTasteScore()
    {
        float perfectMin = perfectCookLevel - perfectCookOffset;
        float perfectMax = perfectCookLevel + perfectCookOffset;
        if (cookProgress >= perfectMin && cookProgress <= perfectMax)
        {
            return tasteScore = 1f; 
        }

        if (cookProgress < perfectMin)
        {
            return tasteScore = Mathf.InverseLerp(0f, perfectMin, cookProgress);
        }

        if (cookProgress > perfectMax)
        {
            return tasteScore = Mathf.InverseLerp(maxBurnLevel, perfectMax, cookProgress);
        }

        return tasteScore = 0f; // Đề phòng lỗi logic (Fallback)
    }

}
