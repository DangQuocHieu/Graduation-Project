using UnityEngine;

public class CuttingBoard : GrabbaleObject
{
    public PlaceableSurface PlaceableSurface;

    public void InteractWithIngredient(Ingredient ingredient)
    {
        ItemContainer.Add(ingredient);
    }


}
