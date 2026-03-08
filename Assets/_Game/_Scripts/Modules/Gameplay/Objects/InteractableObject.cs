using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
public class InteractableObject : MonoBehaviour
{
    private Rigidbody _rb;
    private Collider _collider;
    private Transform _grabObjectPoint;
    [SerializeField] private float _followSpeed = 20f;
    [SerializeField] private float _rotationSpeed = 10f;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
        SetUpRigidbody();
    }

    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if(_grabObjectPoint != null)
        {
            HandleFollowToTarget();
            HandleRotateToTarget();
        }
    }

    private void SetUpRigidbody()
    {
        _rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }

    public void OnPickUp(Transform grabObjectPoint, Collider collider)
    {
        _grabObjectPoint = grabObjectPoint;
        _rb.useGravity = false;
        _rb.linearVelocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
        _rb.freezeRotation = true;
        Physics.IgnoreCollision(collider, _collider, true);
    }

    private void HandleFollowToTarget()
    {
        Vector3 directionToTarget = _grabObjectPoint.position - transform.position;
        _rb.linearVelocity = directionToTarget * _followSpeed;
    }

    private void HandleRotateToTarget()
    {
        Quaternion targetRotation = Quaternion.Slerp(transform.rotation, _grabObjectPoint.rotation, _rotationSpeed * Time.fixedDeltaTime);
        _rb.MoveRotation(targetRotation);
    }

    public void OnDrop(Collider collider)
    {
        Debug.Log("Drop: " + gameObject.name);
        _grabObjectPoint = null;
        _rb.useGravity = true;
        _rb.freezeRotation = false;
        Physics.IgnoreCollision(collider, _collider, false);
    }





}
