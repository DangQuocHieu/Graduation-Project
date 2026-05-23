using UnityEngine;
using UnityEngine.AI;

public class TutorialArrow : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float lineWidth = 0.1f;
    public Transform point;
    private LineRenderer lineRenderer;
    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)

        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }
        lineRenderer.useWorldSpace = true;
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
    }



    void Update()
    {
        if (playerTransform == null) return;
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, point.position);
        lineRenderer.SetPosition(1, playerTransform.position);
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<PlayerController>(out _))
        {
            gameObject.SetActive(false);
        }
    }

}