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
        base.InteractWith(hit, pickupAndDropHandler);
    }

    private IEnumerator FillRiceNoodleCoroutine(RiceNoodleContainer riceNoodleContainer)
    {
        while (true)
        {
            var anchorPoint = GetAnchorPoint(IngredientType.RiceNoodle);
            if (anchorPoint == null)
            {
                yield break;
            }
            var riceNoodle = riceNoodleContainer.Get();
            if (riceNoodle == null)
            {
                yield break;
            }
            if (riceNoodle.HandleInteractWithBambooTray(this, null))
            {
                yield return new WaitForSeconds(0.2f);
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
