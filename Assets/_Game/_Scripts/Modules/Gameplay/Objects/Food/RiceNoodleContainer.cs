using UnityEngine;

public class IngredientContainer : GrabbableObject
{
    public Transform ingredientVisual;
    public Ingredient ingredientPrefab;
    public Transform ingredientSpawnPoint;

    public float capacity;
    public float currentIngredientWeight;

    protected override void Awake()
    {
        base.Awake();
        ingredientVisual.localScale = new Vector3(1f, currentIngredientWeight/capacity, 1f);
    }
    public Ingredient Get()
    {
        if(currentIngredientWeight <= 0f)
        {
            return null;
        }
        var ingredient = Instantiate(ingredientPrefab, ingredientSpawnPoint.position, ingredientSpawnPoint.rotation, null);
        currentIngredientWeight -= ingredient.weight;
        return ingredient;
    }

    public void Fill(float weight)
    {
        if(currentIngredientWeight >= capacity)
        {
            return;
        }
        currentIngredientWeight += weight;
        float scaleY = currentIngredientWeight / weight;
        ingredientVisual.localScale = new Vector3(1f, scaleY, 1f);

    }
    
}
