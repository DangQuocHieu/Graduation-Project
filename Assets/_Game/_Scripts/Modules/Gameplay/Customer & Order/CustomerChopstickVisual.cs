using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class CustomerChopstickVisual : SerializedMonoBehaviour
{
    public Dictionary<IngredientType, IngredientVisual> ingredientVisualDict = new();
    public IngredientType[] ingredientTypes;
    public IngredientVisual displayIngredient;
    public BambooTray attachedDish;

    public void DisplayIngredient(IngredientType type)
    {
        displayIngredient = ingredientVisualDict[type];
        displayIngredient.gameObject.SetActive(true);
    }

    public void HideCurrentIngredient()
    {
        if(displayIngredient != null)
        {
            displayIngredient.gameObject.SetActive(false);
            displayIngredient = null;
        }
    }

    public void DisplayRandomIngredient()
    {
        DisplayIngredient(ingredientTypes[Random.Range(0, ingredientTypes.Length)]);
    }
}
