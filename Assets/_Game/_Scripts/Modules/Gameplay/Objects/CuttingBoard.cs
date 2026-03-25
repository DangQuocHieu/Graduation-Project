using UnityEngine;

public class CuttingBoard : GrabbableObject
{
    public PlaceableSurface PlaceableSurface;

    public void InteractWithIngredient(Ingredient ingredient)
    {
        ItemContainer.Add(ingredient);
    }


}
