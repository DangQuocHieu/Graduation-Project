using UnityEngine;

public class RiceNoodleContainer : GrabbableObject
{
    public Transform riceNoodleVisual;
    public Ingredient riceNoodlePiecePrefab;
    public Transform ingredientSpawnPoint;

    public float capacity;
    public float currentRiceNoodleWeight;

    protected override void Awake()
    {
        base.Awake();
        riceNoodleVisual.localScale = new Vector3(1f, currentRiceNoodleWeight/capacity, 1f);
    }
    public Ingredient Get()
    {
        if(currentRiceNoodleWeight <= 0f)
        {
            return null;
        }
        var riceNoodlePiece = Instantiate(riceNoodlePiecePrefab, ingredientSpawnPoint.position, ingredientSpawnPoint.rotation, null);
        currentRiceNoodleWeight -= riceNoodlePiece.weight;
        return riceNoodlePiece;
    }

    public void Fill(float weight)
    {
        if(currentRiceNoodleWeight >= capacity)
        {
            return;
        }
        currentRiceNoodleWeight += weight;
        float scaleY = currentRiceNoodleWeight / weight;
        riceNoodleVisual.localScale = new Vector3(1f, scaleY, 1f);

    }
    
}
