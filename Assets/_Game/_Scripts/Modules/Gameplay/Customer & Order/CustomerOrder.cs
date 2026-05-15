 using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class CustomerOrder 
{
    public List<IngredientType> ingredientTypes = new();
    public SauceType sauceType;
}
