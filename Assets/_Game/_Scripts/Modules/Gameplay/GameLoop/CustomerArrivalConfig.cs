using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CustomerArrivalConfig
{
    public Customer customerPrefab;
    public List<IngredientType> order = new();
    public float spawnTime;
}
