using UnityEngine;

public class SliceableObject : MonoBehaviour 
{
    public GrabbableObject grabbaleObject;
    public SlicePieces slicePiecesPrefab;
    public void OnSlice(CuttingBoard cuttingBoard)
    {
        cuttingBoard.PlaceableSurface.ingredientContainer.containedItems.Remove(grabbaleObject);
        var slicePieces = Instantiate(slicePiecesPrefab, transform.position, transform.rotation, transform.parent);
        foreach(var piece in slicePieces.pieces)
        {
            piece.JoinWithOtherRigidbody(cuttingBoard.PlaceableSurface.ingredientContainer.rb);
            if(piece is Ingredient ingredient)
            {
                ingredient.attachedIngredientContainer = cuttingBoard.PlaceableSurface.ingredientContainer;
                cuttingBoard.PlaceableSurface.ingredientContainer.containedItems.Add(piece);
            }
        }
        Destroy(gameObject);
    }
}
