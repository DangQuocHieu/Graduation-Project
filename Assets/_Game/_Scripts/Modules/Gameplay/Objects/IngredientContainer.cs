using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class IngredientContainer : MonoBehaviour
{
    public HashSet<GrabbableObject> containedItems = new();
    public Rigidbody rb;
    public HashSet<GrabbableObject> ContainedItems => containedItems;
    public Vector3 boxSize = Vector3.one;
    public List<IngredientSlot> ingredientSlots = new();
    public PlaceableSurface placeableSurface;
    public List<GrabbableObject> ContainedItemList = new();

    void Awake()
    {
        placeableSurface = GetComponent<PlaceableSurface>();
    }

    void Update()
    {
        ContainedItemList = containedItems.ToList();
    }

    public List<Collider> GetContainedColliders()
    {
        List<Collider> colliders = new();
        foreach (var obj in containedItems)
        {
            foreach (var collider in obj.objectColliders)
            {
                colliders.Add(collider);
            }
        }
        return colliders;
    }


    public List<SliceableObject> GetSliceableList()
    {
        List<SliceableObject> sliceableObjects = new();
        foreach (var obj in containedItems)
        {
            if (obj.TryGetComponent<SliceableObject>(out var sliceableObject))
            {
                sliceableObjects.Add(sliceableObject);
            }
        }
        return sliceableObjects;
    }

    public List<Ingredient> GetCookableList()
    {
        List<Ingredient> cookableObjects = new();
        foreach (var obj in containedItems)
        {
            if (obj is Ingredient ingredient && obj.GetComponent<CookableObject>() != null)
            {
                cookableObjects.Add(ingredient);
            }
        }
        return cookableObjects;
    }

    public IEnumerator FillCoroutine(List<Ingredient> ingredients)
    {
        HashSet<IngredientSlot> selectedSlots = new();
        foreach (var ingredient in ingredients)
        {
            var ingredientSlot = ingredientSlots.Find(T => T.IsEmpty() && !selectedSlots.Contains(T));
            if(ingredientSlot != null)
            {
                selectedSlots.Add(ingredientSlot);
                ingredient.RemoveRigidbodyJoin();
                ingredient.MoveToPlaceableSurface(placeableSurface, ingredientSlot.transform.position, ingredientSlot.transform.rotation);
                yield return new WaitForSeconds(0.1f);
            }
        }
    }

}
