using System.Collections.Generic;
using UnityEngine;

public class ItemContainer : MonoBehaviour
{
    public HashSet<GrabbableObject> containedItems = new();
    public Rigidbody rb;
    public HashSet<GrabbableObject> ContainedItems => containedItems;

    public AnchorPoint[] anchorPoints;

    public void Add(GrabbableObject objectToAdd)
    { 
        containedItems.Add(objectToAdd);

    }

    public void Remove(GrabbableObject objectToRemove)
    {
        containedItems.Remove(objectToRemove);
    }

    public List<Collider> GetContainedColliders()
    {
        List<Collider> colliders = new();
        foreach(var obj in containedItems)
        {
            foreach(var collider in obj.objectColliders)
            {
                colliders.Add(collider);
            }
        }
        return colliders;
    }


    public void GetSliceableList(List<SliceableObject> sliceableObjects)
    {
        sliceableObjects.Clear();
        foreach(var obj in containedItems)
        {
            if(obj.TryGetComponent<SliceableObject>(out var sliceableObject))
            {
                sliceableObjects.Add(sliceableObject);
            }
        }
    }


}
