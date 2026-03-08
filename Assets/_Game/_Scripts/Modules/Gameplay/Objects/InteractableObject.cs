using System.Collections;
using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
public class InteractableObject : MonoBehaviour
{
    private Rigidbody _rb;
    private Collider _collider;
    private Transform _grabObjectPoint;
    [SerializeField] private float _moveSpeed = 10f; //speed use when move to placeable surface
    [SerializeField] private float _followSpeed = 20f; //speed use when follow player
    [SerializeField] private float _rotationSpeed = 10f;
    private Coroutine _moveToPlaceableSurfaceCoroutine;

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
        if (_grabObjectPoint != null)
        {
            HandleFollowToTarget();
            HandleRotateToTarget();
        }
    }

    private void SetUpRigidbody()
    {
        _rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }

    public virtual void InteractWith(InteractableObject other)
    {

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

    public void MoveToPlaceableSurface(Vector3 dropPosition, Collider collider)
    {
        Debug.Log("Object: " + gameObject.name + " move to surface");
        _grabObjectPoint = null;
        dropPosition += Vector3.up * 0.5f;
        if (_moveToPlaceableSurfaceCoroutine != null)
        {
            StopCoroutine(_moveToPlaceableSurfaceCoroutine);
            _moveToPlaceableSurfaceCoroutine = null;
        }
        _moveToPlaceableSurfaceCoroutine = StartCoroutine(MoveToPlaceableSurfaceCoroutine(dropPosition, collider));
    }

    private IEnumerator MoveToPlaceableSurfaceCoroutine(Vector3 dropPosition, Collider collider)
    {
        while (Vector3.Distance(transform.position, dropPosition) >= 0.1f)
        {
            Vector3 direction = dropPosition - transform.position;
            _rb.linearVelocity = direction * _moveSpeed;
            yield return new WaitForFixedUpdate();
        }

        _rb.constraints = RigidbodyConstraints.None;
        _rb.linearVelocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
        _rb.useGravity = true;
        Physics.IgnoreCollision(_collider, collider, false);

    }




}
