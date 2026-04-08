using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "ShopItemSO", menuName = "Scriptable Objects/ShopItemSO")]
public class ShopItemSO : ScriptableObject
{
    public GrabbableObjectSO grabbableObjectSO;
    public GrabbableObject itemPrefab;
    public ItemType itemType;
    private bool IsIngredient() => itemType is ItemType.Ingredient;


}
