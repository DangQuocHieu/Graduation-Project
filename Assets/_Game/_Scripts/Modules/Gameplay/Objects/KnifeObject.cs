using UnityEngine;

public class KnifeObject : InteractableObject
{
    public override void InteractWith(InteractableObject other)
    {
        if(other.TryGetComponent<CuttingBoard>(out var cuttingBoard))
        {
            
        }   
    }


}
