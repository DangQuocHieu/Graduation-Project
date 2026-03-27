using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GrabbableObject : MonoBehaviour
{
    private Rigidbody _rb;
    public Collider[] objectColliders; // Đã đổi thành mảng
    private Collider _playerCollider;
    private Transform _grabObjectPoint;

    [Title("Physics Configurations")]
    public float moveSpeed = 10f;
    public float followSpeed = 20f;
    public float rotationSpeed = 10f;
    private Coroutine _moveToPlaceableSurfaceCoroutine;
    private Coroutine _waitForPickupCompleteCoroutine;

    [Title("Base References")]
    public FixedJoint fixedJoint;
    public ItemContainer ItemContainer;

    [Title("Runtime Tracking")]
    public bool isWaitingForSurfaceImpact = false;
    public PlaceableSurface targetSurface;


    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
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

    }


    public void OnPickUp(Transform grabObjectPoint, Collider collider)
    {
        if (_moveToPlaceableSurfaceCoroutine != null)
        {
            StopCoroutine(_moveToPlaceableSurfaceCoroutine);
            _moveToPlaceableSurfaceCoroutine = null;
        }
        RemoveRigidbodyJoin();
        _grabObjectPoint = grabObjectPoint;
        _rb.isKinematic = false;
        _rb.constraints = RigidbodyConstraints.FreezeRotation;
        _rb.useGravity = false;

        _rb.linearVelocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
        _playerCollider = collider;
        foreach (var col in objectColliders)
        {
            col.isTrigger = true;
        }
        if (_waitForPickupCompleteCoroutine != null)
        {
            StopCoroutine(_waitForPickupCompleteCoroutine);
            _waitForPickupCompleteCoroutine = null;
        }
        _waitForPickupCompleteCoroutine = StartCoroutine(WaitForPickupComplete());
    }

    private IEnumerator WaitForPickupComplete()
    {
        yield return new WaitUntil(() => Vector3.Distance(transform.position, _grabObjectPoint.position) <= 0.05f);
        foreach (var collider in objectColliders)
        {
            collider.isTrigger = false;
        }

    }

    private void HandleFollowToTarget()
    {
        Vector3 directionToTarget = _grabObjectPoint.position - transform.position;
        _rb.linearVelocity = directionToTarget * followSpeed;
    }

    private void HandleRotateToTarget()
    {
        float targetY = _grabObjectPoint.eulerAngles.y;
        Quaternion targetRotation = Quaternion.Euler(0, targetY, 0);
        Quaternion smoothedRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
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
            _playerCollider = null;
        }
    }

    public void MoveToPlaceableSurface(PlaceableSurface placeableSurface, RaycastHit hit)
    {
        Debug.Log("Object: " + gameObject.name + " move to surface");
        _grabObjectPoint = null;
        Vector3 dropPosition = placeableSurface.SnapPoint == null ? hit.point : placeableSurface.SnapPoint.position;
        dropPosition += Vector3.up * 0.5f;

        if (_moveToPlaceableSurfaceCoroutine != null)
        {
            StopCoroutine(_moveToPlaceableSurfaceCoroutine);
            _moveToPlaceableSurfaceCoroutine = null;
        }
        _moveToPlaceableSurfaceCoroutine = StartCoroutine(MoveToPlaceableSurfaceCoroutine(dropPosition, placeableSurface));
    }

    private IEnumerator MoveToPlaceableSurfaceCoroutine(Vector3 dropPosition, PlaceableSurface placeableSurface)
    {
        _rb.isKinematic = true;

        // 1. BẬT isTrigger để bay xuyên qua các vật khác trên đường đi
        foreach (var col in objectColliders)
        {
            col.isTrigger = true;
        }

        while (Vector3.Distance(transform.position, dropPosition) > 0.05f)
        {
            Vector3 newPos = Vector3.MoveTowards(transform.position, dropPosition, moveSpeed * Time.fixedDeltaTime);
            _rb.MovePosition(newPos);
            yield return new WaitForFixedUpdate();
        }

        transform.position = dropPosition;
        yield return new WaitForFixedUpdate();

        // 2. TẮT isTrigger để chuẩn bị rơi tự do xuống mặt phẳng
        foreach (var col in objectColliders)
        {
            col.isTrigger = false;
        }

        _rb.isKinematic = false;
        _rb.linearVelocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
        _rb.useGravity = true;
        _rb.constraints = RigidbodyConstraints.None;
        targetSurface = placeableSurface;
        isWaitingForSurfaceImpact = true;

        yield return new WaitUntil(() => targetSurface == null);

        if (_playerCollider != null)
        {
            _playerCollider = null;
        }

        _moveToPlaceableSurfaceCoroutine = null;
    }

    public virtual void InteractWith(RaycastHit hit, PickupAndDropHandler pickupAndDropHandler)
    {
        if (hit.collider.TryGetComponent<PlaceableArea>(out var placeableArea))
        {
            MoveToPlaceableSurface(placeableArea.placeableSurface, hit);
            pickupAndDropHandler.DropObject();
        }
    }

    public void JoinWithOtherRigidbody(Rigidbody other)
    {
        fixedJoint = gameObject.AddComponent<FixedJoint>();
        fixedJoint.connectedBody = other;
    }

    public void RemoveRigidbodyJoin()
    {
        if (fixedJoint != null)
        {
            Destroy(fixedJoint);
            fixedJoint = null;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collide with: " + collision.gameObject.name);
        if (isWaitingForSurfaceImpact)
        {
            if (collision.gameObject.TryGetComponent<PlaceableSurface>(out var placeableSurface))
            {

                if (placeableSurface.gameObject == targetSurface.gameObject)
                {
                    if (targetSurface.ItemContainer != null)
                    {
                        JoinWithOtherRigidbody(targetSurface.ItemContainer.rb);
                    }
                    isWaitingForSurfaceImpact = false;
                    targetSurface = null;
                }
                else
                {
                    isWaitingForSurfaceImpact = false;
                    targetSurface = null;
                }
            }
        }
    }
}