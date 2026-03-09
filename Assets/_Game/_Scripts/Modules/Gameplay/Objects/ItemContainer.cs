using System.Collections.Generic;
using UnityEngine;

public class ItemContainer : MonoBehaviour
{
    [SerializeField] private HashSet<GameObject> _containedItems = new();

    public void Add(GameObject objectToAdd)
    {
        if(!_containedItems.Contains(objectToAdd))
        {
            _containedItems.Add(objectToAdd);
        }
    }

    public void Remove(GameObject objectToRemove)
    {
        if(_containedItems.Contains(objectToRemove))
        {
            _containedItems.Remove(objectToRemove);
        }
    }
}
