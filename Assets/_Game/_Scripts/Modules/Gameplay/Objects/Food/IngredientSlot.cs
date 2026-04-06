using System.Collections.Generic;
using UnityEngine;

public class IngredientSlot : MonoBehaviour
{
    public Vector3 boxSize;

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        // Thiết lập ma trận để Gizmo xoay và di chuyển theo Transform
        Gizmos.matrix = transform.localToWorldMatrix;

        // Vẽ tại Vector3.zero vì ma trận đã xử lý vị trí rồi
        Gizmos.DrawWireCube(Vector3.zero, boxSize);
    }

    public bool IsEmpty()
    {
        var hits = Physics.OverlapBox(transform.position, boxSize / 2, transform.rotation);
        foreach (var hit in hits)
        {
            if (hit.GetComponent<Ingredient>() != null)
            {
                return false;
            }
        }
        return true;
    }
}
