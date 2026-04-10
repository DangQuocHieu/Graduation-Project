using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CustomerManager : MonoBehaviour
{
    public List<Customer> allCustomers = new();
    public Transform orderPoint;

    public List<ChairObject> chairObjects = new();

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Vector3 boxSize = new Vector3(1f, 2f, 1f);
        Vector3 center = orderPoint.position + new Vector3(0, boxSize.y / 2f, 0);
        Gizmos.DrawWireCube(center, boxSize);
        Gizmos.color = new Color(1f, 0.92f, 0.016f, 0.2f); 
        Gizmos.DrawCube(center, boxSize);
    }

    public ChairObject GetAvailableChair()
    {
        var availableSeats = chairObjects.Where(T => T.isEmpty).ToList();
        return availableSeats[Random.Range(0, availableSeats.Count)];
    }
}
