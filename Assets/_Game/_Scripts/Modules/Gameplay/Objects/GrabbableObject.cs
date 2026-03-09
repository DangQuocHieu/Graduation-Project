using System.Collections;
using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
public class GrabbaleObject : MonoBehaviour
{
    private Rigidbody _rb;
    private Collider _collider;
    private Collider _playerCollider;
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


    public void OnPickUp(Transform grabObjectPoint, Collider collider)
    {
        _grabObjectPoint = grabObjectPoint;
        _rb.constraints = RigidbodyConstraints.FreezeRotation;
        _rb.useGravity = false;
        _rb.linearVelocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
        _playerCollider = collider;
        Physics.IgnoreCollision(collider, _collider, true);
    }

    private void HandleFollowToTarget()
    {
        Vector3 directionToTarget = _grabObjectPoint.position - transform.position;
        _rb.linearVelocity = directionToTarget * _followSpeed;
    }

    private void HandleRotateToTarget()
    {
        float targetY = _grabObjectPoint.eulerAngles.y;
        Quaternion targetRotation = Quaternion.Euler(0, targetY, 0);
        Quaternion smoothedRotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * Time.fixedDeltaTime);
        _rb.MoveRotation(smoothedRotation);
    }

    public void OnDrop()
    {
        Debug.Log("Drop: " + gameObject.name);
        _grabObjectPoint = null;
        _rb.useGravity = true;
        _rb.constraints = RigidbodyConstraints.None;
        if (_playerCollider != null)
        {
            Physics.IgnoreCollision(_collider, _playerCollider, false);
            _playerCollider = null;
        }
    }

    public void MoveToPlaceableSurface(Vector3 dropPosition)
    {
        Debug.Log("Object: " + gameObject.name + " move to surface");
        _grabObjectPoint = null;
        dropPosition += Vector3.up * 0.5f;
        if (_moveToPlaceableSurfaceCoroutine != null)
        {
            StopCoroutine(_moveToPlaceableSurfaceCoroutine);
            _moveToPlaceableSurfaceCoroutine = null;
        }
        _moveToPlaceableSurfaceCoroutine = StartCoroutine(MoveToPlaceableSurfaceCoroutine(dropPosition));
    }

    private IEnumerator MoveToPlaceableSurfaceCoroutine(Vector3 dropPosition)
    {
        while (Vector3.Distance(transform.position, dropPosition) > 0.05f)
        {
            Vector3 newPos = Vector3.MoveTowards(transform.position, dropPosition, _moveSpeed * Time.fixedDeltaTime);
            _rb.MovePosition(newPos);
            _rb.linearVelocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;

            yield return new WaitForFixedUpdate();
        }
        transform.position = dropPosition;
        _rb.linearVelocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
        yield return new WaitForFixedUpdate();
        _rb.useGravity = true;
        _rb.constraints = RigidbodyConstraints.None;
        if (_playerCollider != null)
        {
            Physics.IgnoreCollision(_collider, _playerCollider, false);
            _playerCollider = null;
        }
        _moveToPlaceableSurfaceCoroutine = null;
    }

    public virtual void InteractWith(RaycastHit hit, PickupAndDropHandler pickupAndDropHandler)
    {
        if(hit.collider.TryGetComponent<PlaceableSurface>(out var placeableSurface))
        {
            Vector3 dropPosition = placeableSurface.SnapPoint == null ? hit.point : placeableSurface.SnapPoint.position;
            MoveToPlaceableSurface(dropPosition);
            pickupAndDropHandler.DropObject();
        }
    }
}
