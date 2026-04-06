using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

public class BambooTray : GrabbableObject
{
    private Dictionary<IngredientType, List<AnchorPoint>> anchorPointsDic = new();
    public PlaceableSurface placeableSurface;

    [Title("Sauce")]
    public Transform sauceAnchor;

    [Title("Tofu")]
    public Transform tofuAnchorParent;
    public List<AnchorPoint> tofuAnchorPoints = new();

    [Title("Rice Noodle")]
    public Transform riceNoodleAnchorParent;
    public List<AnchorPoint> riceNoodleAnchorPoints = new();

    [Title("Cucumber")]
    public Transform cucumberAnchorParent;
    public List<AnchorPoint> cucumberAnchorPoints = new();

    [Title("Herb")]
    public Transform herbAnchor;
    public List<AnchorPoint> herbAnchorPoints = new();

    protected override void Awake()
    {
        base.Awake();
        InitializeAnchorPoints();
        anchorPointsDic.Add(IngredientType.Tofu, tofuAnchorPoints);
        anchorPointsDic.Add(IngredientType.RiceNoodle, riceNoodleAnchorPoints);
        anchorPointsDic.Add(IngredientType.Cucumber, cucumberAnchorPoints);
        anchorPointsDic.Add(IngredientType.Herb, herbAnchorPoints);

    }

    private void InitializeAnchorPoints()
    {
        for (int i = 0; i < tofuAnchorParent.childCount; i++)
        {
            var anchorPoint = new AnchorPoint() { anchor = tofuAnchorParent.GetChild(i), isEmpty = true };
            tofuAnchorPoints.Add(anchorPoint);
        }
        for (int i = 0; i < riceNoodleAnchorParent.childCount; i++)
        {
            var anchorPoint = new AnchorPoint() { anchor = riceNoodleAnchorParent.GetChild(i), isEmpty = true };
            riceNoodleAnchorPoints.Add(anchorPoint);
        }
        for (int i = 0; i < cucumberAnchorParent.childCount; i++)
        {
            var anchorPoint = new AnchorPoint() { anchor = cucumberAnchorParent.GetChild(i), isEmpty = true };
            cucumberAnchorPoints.Add(anchorPoint);
        }
        for (int i = 0; i < herbAnchor.childCount; i++)
        {
            var anchorPoint = new AnchorPoint() { anchor = herbAnchor.GetChild(i), isEmpty = true };
            herbAnchorPoints.Add(anchorPoint);
        }
    }
    public AnchorPoint GetAnchorPoint(IngredientType ingredientType)
    {
        var anchorPoint = anchorPointsDic[ingredientType];
        return anchorPoint.Find(T => T.isEmpty);
    }

    public override void InteractWith(RaycastHit hit, PickupAndDropHandler pickupAndDropHandler)
    {
        if (hit.collider.TryGetComponent<RiceNoodleContainer>(out var riceNoodleContainer))
        {
            StartCoroutine(FillRiceNoodleCoroutine(riceNoodleContainer));
        }
        else if (hit.collider.GetComponentInParent<FryingPan>())
        {
            var fryingPan = hit.collider.GetComponentInParent<FryingPan>();
        }
        base.InteractWith(hit, pickupAndDropHandler);
    }

    public IEnumerator FillCookableObjectCoroutine(List<Ingredient> cookableObjects)
    {
        Debug.Log("SFSFSF");
        foreach (var cookableobject in cookableObjects)
        {
            var anchorPoint = GetAnchorPoint(cookableobject.ingredientType);
            if (anchorPoint != null)
            {
                anchorPoint.isEmpty = false;
                cookableobject.attachedAnchorPoint = anchorPoint;
                cookableobject.RemoveRigidbodyJoin();
                cookableobject.MoveToPlaceableSurface(placeableSurface, anchorPoint.anchor.position, anchorPoint.anchor.rotation);
                yield return new WaitForSeconds(0.1f);
            }
        }
    }

    private IEnumerator FillRiceNoodleCoroutine(RiceNoodleContainer riceNoodleContainer)
    {
        Ingredient previousRiceNoodle = null;
        while (true)
        {
            var anchorPoint = GetAnchorPoint(IngredientType.RiceNoodle);
            if (anchorPoint == null)
            {
                yield return new WaitUntil(() => previousRiceNoodle.targetSurface == null);
                yield return new WaitForSeconds(0.2f);
                yield break;
            }
            var riceNoodle = riceNoodleContainer.Get();
            if (riceNoodle == null)
            {
                yield return new WaitForSeconds(0.2f);
                yield break;
            }
            if (riceNoodle.HandleInteractWithBambooTray(this, null))
            {
                previousRiceNoodle = riceNoodle;
                yield return new WaitForSeconds(0.1f);
            }

        }
    }

}

[System.Serializable]
public class AnchorPoint
{
    public Transform anchor;
    public bool isEmpty = true;
}
