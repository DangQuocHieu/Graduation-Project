using UnityEngine;

public class SliceableObject : MonoBehaviour
{
    public GrabbaleObject grabbaleObject;
    public SlicePieces slicePiecesPrefab;
    public void OnSlice(CuttingBoard cuttingBoard)
    {
        var slicePieces = Instantiate(slicePiecesPrefab, transform.position, Quaternion.identity, transform.parent);
        foreach(var piece in slicePieces.pieces)
        {
            cuttingBoard.ItemContainer.Add(piece);
            piece.JoinWithOtherRigidbody(cuttingBoard.ItemContainer.rb);
        }
        Destroy(gameObject);
    }
}
