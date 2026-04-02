using UnityEngine;

public class SliceableObject : MonoBehaviour 
{
    public GrabbableObject grabbaleObject;
    public SlicePieces slicePiecesPrefab;
    public void OnSlice(CuttingBoard cuttingBoard)
    {
        var slicePieces = Instantiate(slicePiecesPrefab, transform.position, transform.rotation, transform.parent);
        foreach(var piece in slicePieces.pieces)
        {
            cuttingBoard.itemContainer.Add(piece);
            piece.JoinWithOtherRigidbody(cuttingBoard.itemContainer.rb);
        }
        Destroy(gameObject);
    }
}
