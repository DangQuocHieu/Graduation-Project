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
    public Rigidbody rb;
    public Collider[] objectColliders; // Đã đổi thành mảng
    private Collider _playerCollider;
    private Transform _grabObjectPoint;

    [Title("Physics Configurations")]
    public float moveSpeed = 10f;
    public float followSpeed = 20f;
    public float rotationSpeed = 10f;
    public float dropOffset = 0.5f;
    private Coroutine _moveCoroutine;
    protected Coroutine _waitForPickupCompleteCoroutine;

    [Title("Base References")]
    public FixedJoint fixedJoint;
    [Title("Runtime Tracking")]
    public bool isWaitingForSurfaceImpact = false;
    public PlaceableSurface targetSurface;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        SetUpRigidbody();
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


    public virtual void OnPickUp(Transform grabObjectPoint, Collider collider)
    {
        if (_moveCoroutine != null)
        {
            StopCoroutine(_moveCoroutine);
            _moveCoroutine = null;
        }
        RemoveRigidbodyJoin();
        _grabObjectPoint = grabObjectPoint;
        rb.isKinematic = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.useGravity = false;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        _playerCollider = collider;
        foreach (var col in objectColliders)
        {
            col.isTrigger = true;
        }
        StopWaitForPickupCompleteCoroutine();
        _waitForPickupCompleteCoroutine = StartCoroutine(WaitForPickupComplete());
    }



    protected void StopWaitForPickupCompleteCoroutine()
    {
        if (_waitForPickupCompleteCoroutine != null)
        {
            StopCoroutine(_waitForPickupCompleteCoroutine);
            _waitForPickupCompleteCoroutine = null;
        }
    }

    protected IEnumerator WaitForPickupComplete()
    {
        yield return new WaitUntil(() =>
        {
            if (_grabObjectPoint == null)
            {
                return true;
            }
            Vector3 currentPosXZ = new Vector3(transform.position.x, 0f, transform.position.z);
            Vector3 targetPosXZ = new Vector3(_grabObjectPoint.position.x, 0f, _grabObjectPoint.position.z);
            return Vector3.Distance(currentPosXZ, targetPosXZ) <= 0.05f;
        });
        if (_grabObjectPoint == null)
        {
            yield break;
        }
        foreach (var collider in objectColliders)
        {
            collider.isTrigger = false;
        }
    }

    // private void HandleFollowToTarget()
    // {
    //     // float holdDistance = Vector3.Distance(Camera.main.transform.position, _grabObjectPoint.position);
    //     // Vector3 centerScreenVector = new Vector3(Screen.width / 2f, Screen.height / 2f, holdDistance);
    //     // Vector3 exactCenterWorldPos = Camera.main.ScreenToWorldPoint(centerScreenVector);
    //     // Vector3 targetPos = new Vector3(_grabObjectPoint.position.x, exactCenterWorldPos.y, _grabObjectPoint.position.z);
    //     // Vector3 directionToTarget = targetPos - transform.position;
    //     // directionToTarget -= new Vector3(0f, 0.5f, 0f);
    //     // rb.linearVelocity = directionToTarget * followSpeed;

    //     Vector3 directionToTarget = _grabObjectPoint.position - transform.position;
    //     rb.linearVelocity = directionToTarget * followSpeed;

    // }

    private void HandleFollowToTarget()
    {
        if (_grabObjectPoint == null) return;
        // Lấy vị trí hiện tại của điểm bám (grab point)
        Vector3 targetPosition = _grabObjectPoint.position;

        // Clamp (Giới hạn) trục Y của vị trí đích trong khoảng từ 3f đến 4f
        targetPosition.y = Mathf.Clamp(targetPosition.y, 3f, 4f);

        // Tính toán hướng đi và áp dụng vận tốc
        Vector3 directionToTarget = targetPosition - transform.position;
        rb.linearVelocity = directionToTarget * followSpeed;
    }

    private void HandleRotateToTarget()
    {
        float targetY = _grabObjectPoint.eulerAngles.y;
        Quaternion targetRotation = Quaternion.Euler(0, targetY, 0);
        Quaternion smoothedRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        rb.MoveRotation(smoothedRotation);
    }

    public void OnDrop()
    {
        if (_waitForPickupCompleteCoroutine != null)
        {
            StopCoroutine(_waitForPickupCompleteCoroutine);
            _waitForPickupCompleteCoroutine = null;
        }
        _grabObjectPoint = null;
        rb.useGravity = true;
        rb.constraints = RigidbodyConstraints.None;
        if (_playerCollider != null)
        {
            _playerCollider = null;
        }
    }

    public void MoveToPlaceableSurface(PlaceableSurface placeableSurface, RaycastHit hit)
    {
        _grabObjectPoint = null;
        Vector3 dropPosition = placeableSurface.SnapPoint == null ? hit.point : placeableSurface.SnapPoint.position;
        dropPosition += Vector3.up * dropOffset;
        if (_moveCoroutine != null)
        {
            StopCoroutine(_moveCoroutine);
            _moveCoroutine = null;
        }
        _moveCoroutine = StartCoroutine(MoveToSurfaceCoroutine(dropPosition, placeableSurface));
    }

    public void MoveToPlaceableSurface(PlaceableSurface placeableSurface, Vector3 position)
    {
        _grabObjectPoint = null;
        Vector3 dropPosition = position + Vector3.up * dropOffset;
        if (_moveCoroutine != null)
        {
            StopCoroutine(_moveCoroutine);
            _moveCoroutine = null;
        }
        _moveCoroutine = StartCoroutine(MoveToSurfaceCoroutine(dropPosition, placeableSurface));
    }


    public IEnumerator MoveToSurfaceCoroutine(Vector3 dropPosition, PlaceableSurface placeableSurface)
    {
        rb.isKinematic = true; 
        foreach (var col in objectColliders)
        {
            col.isTrigger = true;
        }

        while (Vector3.Distance(transform.position, dropPosition) > 0.05f)
        {
            Vector3 newPos = Vector3.MoveTowards(transform.position, dropPosition, moveSpeed * Time.fixedDeltaTime);
            rb.MovePosition(newPos);
            yield return new WaitForFixedUpdate();
        }

        transform.position = dropPosition;
        yield return new WaitForFixedUpdate();

        // 2. TẮT isTrigger để chuẩn bị rơi tự do xuống mặt phẳng
        foreach (var col in objectColliders)
        {
            col.isTrigger = false;
        }

        rb.isKinematic = false;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.useGravity = true;
        rb.constraints = RigidbodyConstraints.None;
        targetSurface = placeableSurface;
        isWaitingForSurfaceImpact = true;

        yield return new WaitUntil(() => targetSurface == null);

        if (_playerCollider != null)
        {
            _playerCollider = null;
        }

        _moveCoroutine = null;
    }

    public virtual void InteractWith(RaycastHit hit, PickupAndDropHandler pickupAndDropHandler)
    {
        if (hit.collider.TryGetComponent<KitchenArea>(out var kitchenArea))
        {
            Debug.Log("PLACEABLE AREA OF " + hit.collider.name);
            MoveToPlaceableSurface(kitchenArea.placeableSurface, hit);
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
                    if (targetSurface.itemContainer != null)
                    {
                        JoinWithOtherRigidbody(targetSurface.itemContainer.rb);
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