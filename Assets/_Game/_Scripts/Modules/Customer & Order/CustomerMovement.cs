using UnityEngine;
using UnityEngine.AI;
using System.Collections; // Cần thiết để dùng Coroutine

public class CustomerMovement : MonoBehaviour
{
    public NavMeshAgent agent;
    
    [Header("Rotation Settings")]
    public float rotationSpeed = 5f; 

    private Coroutine rotationCoroutine;

    public void MoveToPosition(Vector3 position)
    {
        agent.SetDestination(position);
    }

    public bool HasReachedDestination()
    {
        if (!agent.pathPending)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    return true;
                }
            }
        }
        return false;
    }

    // Hàm public để bắt đầu xoay từ script khác
    public void StartRotating(Quaternion targetRotation)
    {
        // Dừng coroutine cũ nếu đang chạy để tránh xung đột
        if (rotationCoroutine != null)
        {
            StopCoroutine(rotationCoroutine);
        }
        rotationCoroutine = StartCoroutine(RotateTowardsCoroutine(targetRotation));
    }

    private IEnumerator RotateTowardsCoroutine(Quaternion targetRotation)
    {
        // Tiếp tục xoay cho đến khi góc lệch giữa hiện tại và đích rất nhỏ
        while (Quaternion.Angle(transform.rotation, targetRotation) > 0.1f)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation, 
                targetRotation, 
                rotationSpeed * Time.deltaTime
            );
             
            yield return null;
        }

        transform.rotation = targetRotation;
        rotationCoroutine = null;
    }

    public void DisableMovement()
    {
        agent.ResetPath();
        agent.enabled = false;
    }

    public void MoveToPositionImmediately(Vector3 position)
    {
        transform.position = position;
    }

    public void RotateToTargetImmediately(Quaternion rotation)
    {
        transform.rotation = rotation;
    }
}