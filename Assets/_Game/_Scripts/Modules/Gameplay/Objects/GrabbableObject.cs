using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor.Validation;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(Rigidbody))]
public class GrabbableObject : MonoBehaviour
{
    public Rigidbody rb;
    public Collider[] objectColliders;
    private Collider _playerCollider;
    private Transform _grabObjectPoint;

    [Title("Physics Configurations")]
    public float followSpeed = 20f;
    public float rotationSpeed = 10f;
    public float dropOffset = 0.5f;
    private Coroutine _moveCoroutine;
    protected Coroutine _waitForPickupCompleteCoroutine;

    [Title("Base References")]
    // Đã đổi từ FixedJoint sang ConfigurableJoint
    public ConfigurableJoint configurableJoint;

    [Title("Runtime Tracking")]
    public bool isWaitingForSurfaceImpact = false;
    public PlaceableSurface targetSurface;
    public ItemContainer attachedItemContainer;


    protected virtual void Awake()
    {
        SetUpRigidbody();
    }



    private void LateUpdate()
    {
        if (_grabObjectPoint != null)
        {
            HandleFollowToTarget();
            HandleRotateToTarget();
        }
    }

    private void SetUpRigidbody()
    {
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.isKinematic = false;
    }


    public virtual void OnPickUp(Transform grabObjectPoint, Collider collider)
    {
        if (_moveCoroutine != null)
        {
            StopCoroutine(_moveCoroutine);
            _moveCoroutine = null;
        }

        RemoveRigidbodyJoin();
        RemoveAttachedItemContainer();
        _grabObjectPoint = grabObjectPoint;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;
        _playerCollider = collider;
        ToggleCollider(isTrigger: true);
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
        ToggleCollider(isTrigger: false);
        rb.isKinematic = false;
        if (_grabObjectPoint == null)
        {
            yield break;
        }

    }

    private void HandleFollowToTarget()
    {
        if (_grabObjectPoint == null) return;

        Vector3 targetPosition = _grabObjectPoint.position;
        targetPosition.y = Mathf.Clamp(targetPosition.y, 3f, 4f);

        Vector3 directionToTarget = targetPosition - transform.position;

        if (rb.isKinematic)
        {
            // Tính toán vị trí tiếp theo dựa trên hướng và tốc độ
            Vector3 nextPosition = transform.position + (directionToTarget * followSpeed * Time.fixedDeltaTime);
            rb.MovePosition(nextPosition);
        }
        else
        {
            // Giữ nguyên logic cũ nếu là Dynamic
            rb.linearVelocity = directionToTarget * followSpeed;
        }
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
        rb.isKinematic = false;
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
        // rb.isKinematic = true;
        var itemContainer = GetComponent<ItemContainer>();

        if (itemContainer != null)
        {
            foreach (var item in itemContainer.containedItems)
            {

                item.ToggleCollider(isTrigger: true);
            }
        }
        ToggleCollider(isTrigger: true);

        while (Vector3.Distance(transform.position, dropPosition) > 0.01f)
        {
            // Vector3 newPos = Vector3.MoveTowards(transform.position, dropPosition, followSpeed * Time.fixedDeltaTime);
            // rb.MovePosition(newPos);
            Vector3 direction = dropPosition - transform.position;
            rb.linearVelocity = followSpeed * direction;
            yield return new WaitForFixedUpdate();
        }

        rb.MovePosition(dropPosition);
        yield return new WaitForFixedUpdate();
        if (itemContainer != null)
        {
            foreach (var item in itemContainer.containedItems)
            {
                item.rb.linearVelocity = Vector3.zero;
                item.rb.angularVelocity = Vector3.zero;
                item.ToggleCollider(isTrigger: false);
            }
        }

        ToggleCollider(isTrigger: false);

        yield return new WaitForFixedUpdate();
        // rb.isKinematic = false;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        targetSurface = placeableSurface;
        isWaitingForSurfaceImpact = true;

        yield return new WaitUntil(() => targetSurface == null);
        yield return new WaitForFixedUpdate();
        rb.constraints = RigidbodyConstraints.None;
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
            pickupAndDropHandler.DropObject();
            MoveToPlaceableSurface(kitchenArea.placeableSurface, hit);
        }
    }

    // --- CÁC HÀM XỬ LÝ JOINT ĐÃ ĐƯỢC CẬP NHẬT Ở ĐÂY ---
    public void JoinWithOtherRigidbody(Rigidbody other)
    {
        configurableJoint = gameObject.AddComponent<ConfigurableJoint>();
        configurableJoint.connectedBody = other;
        
        // Khóa tất cả các trục để mô phỏng lại hành vi của FixedJoint
        configurableJoint.xMotion = ConfigurableJointMotion.Locked;
        configurableJoint.yMotion = ConfigurableJointMotion.Locked;
        configurableJoint.zMotion = ConfigurableJointMotion.Locked;
        configurableJoint.angularXMotion = ConfigurableJointMotion.Locked;
        configurableJoint.angularYMotion = ConfigurableJointMotion.Locked;
        configurableJoint.angularZMotion = ConfigurableJointMotion.Locked;
    }

    public void RemoveRigidbodyJoin()
    {
        if (configurableJoint != null)
        {
            Destroy(configurableJoint);
            configurableJoint = null;
        }
    }

    public void RemoveAttachedItemContainer()
    {
        if (attachedItemContainer != null)
        {
            attachedItemContainer.Remove(this);
            attachedItemContainer = null;
        }
    }
    // ------------------------------------------------

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
                        attachedItemContainer = targetSurface.itemContainer;
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

    public void ToggleCollider(bool isTrigger)
    {
        foreach (var col in objectColliders)
        {
            col.isTrigger = isTrigger;
        }
    }
}