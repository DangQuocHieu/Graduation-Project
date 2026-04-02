using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class BambooTray : GrabbableObject
{
    private Dictionary<IngredientType, List<AnchorPoint>> anchorPointsDic = new();
    public PlaceableSurface placeableSurface;

    [Title("Sauce")]
    public Transform sauceAnchor;

    [Title("Tofu")]
    public List<AnchorPoint> tofuAnchorPoints = new();

    [Title("Rice Noodle")]
    public List<AnchorPoint> riceNoodleAnchorPoints = new();

    protected override void Awake()
    {
        base.Awake();
        anchorPointsDic.Add(IngredientType.Tofu, tofuAnchorPoints);
        anchorPointsDic.Add(IngredientType.RiceNoodle, riceNoodleAnchorPoints);
    }


    public AnchorPoint GetAnchorPoint(IngredientType ingredientType)
    {
        var anchorPoint = anchorPointsDic[ingredientType];
        return anchorPoint.Find(T => T.isEmpty);
    }

}

[System.Serializable]
public class AnchorPoint
{
    public Transform anchor;
    public bool isEmpty = true;
}
