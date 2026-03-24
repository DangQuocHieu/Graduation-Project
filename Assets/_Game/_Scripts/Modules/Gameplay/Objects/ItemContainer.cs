using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemContainer : MonoBehaviour
{
    public HashSet<GrabbaleObject> containedItems = new();
    public Rigidbody rb;
    public HashSet<GrabbaleObject> ContainedItems => containedItems;

    public void Add(GrabbaleObject objectToAdd)
    { 
        containedItems.Add(objectToAdd);

    }

    public void Remove(GrabbaleObject objectToRemove)
    {
        containedItems.Remove(objectToRemove);
    }

    public List<Collider> GetContainedColliders()
    {
        List<Collider> colliders = new();
        foreach(var obj in containedItems)
        {
            colliders.Add(obj.objectCollider);
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
