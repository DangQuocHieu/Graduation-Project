using UnityEngine;

public class CuttingBoard : GrabbaleObject
{
    public ItemContainer ItemContainer;
    public PlaceableSurface PlaceableSurface;

    public void InteractWithIngredient(Ingredient ingredient)
    {
        ItemContainer.Add(ingredient.gameObject);
    }

}
