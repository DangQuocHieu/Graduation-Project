using DG.Tweening;
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

    [Header("Visual")]
    public Gradient cookingColors;
    public Tween cookingTween;

    void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        foodMat = meshRenderer.material;
    }

    public void StartCooking()
    {
        if (cookingTween != null) return;
        float remainingPercentage = 1f - (cookProgress / maxBurnLevel);
        float timeRemaining = totalTimeToBurn * remainingPercentage;
        cookingTween = DOTween.To(() => cookProgress, x => cookProgress = x, maxBurnLevel, timeRemaining)
            .SetEase(Ease.Linear)
            .SetLink(gameObject)
            .OnUpdate(() => UpdateFoodColor());
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
        if(other.TryGetComponent<CookingOil>(out var cookingOil))
        {
            PauseCooking();
        }
    }

}
