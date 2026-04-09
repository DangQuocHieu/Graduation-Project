using UnityEngine;

[CreateAssetMenu(fileName = "IngredientSO", menuName = "Scriptable Objects/IngredientSO")]
public class IngredientSO : GrabbableObjectSO
{
    public IngredientType ingredientType;
    public float initialAmount; //g
}
