using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CustomerManager : MonoBehaviour
{
    public List<Customer> allCustomers = new();
    public Transform orderPoint;
    public Transform payPoint;
    public Transform[] leavePoints;

    public List<ChairObject> chairObjects = new();

    void OnDrawGizmos()
    {
        DrawBoxGizmo(orderPoint.position, Color.red);
        DrawBoxGizmo(payPoint.position, Color.yellow);
        foreach(var leavePoint in leavePoints)
        {
            DrawBoxGizmo(leavePoint.position, Color.black);
        }
    }

    public ChairObject GetAvailableChair()
    {
        var availableSeats = chairObjects.Where(T => T.isEmpty).ToList();
        return availableSeats[Random.Range(0, availableSeats.Count)];
    }

    private void DrawBoxGizmo(Vector3 position, Color gizmoColor)
    {
        Gizmos.color = gizmoColor;
        Vector3 boxSize = new Vector3(1f, 2f, 1f);
        Vector3 center = position + new Vector3(0, boxSize.y / 2f, 0);
        Gizmos.DrawWireCube(center, boxSize);
        Gizmos.color = new Color(1f, 0.92f, 0.016f, 0.2f);
        Gizmos.DrawCube(center, boxSize);
    }

    public Transform GetRandomLeavePoint()
    {
        return leavePoints[Random.Range(0, leavePoints.Length)];
    }
}
