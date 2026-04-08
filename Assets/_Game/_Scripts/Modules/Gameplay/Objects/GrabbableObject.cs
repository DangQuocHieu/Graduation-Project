using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(ObjectHover))]
public class GrabbableObject : MonoBehaviour
{

    [Title("Object Info")]
    public GrabbableObjectSO grabbableObjectSO;

    [Title("Base References")]
    // Đã đổi từ FixedJoint sang ConfigurableJoint
    public FixedJoint fixedJoint;
    public Rigidbody rb;
    public Collider[] objectColliders;
    private Transform _grabObjectPoint;

    [Title("Physics Configurations")]
    public float followSpeed = 20f;
    public float rotationSpeed = 10f;
    public float dropOffset = 0.5f;
    private Coroutine _moveCoroutine;
    protected Coroutine _waitForPickupCompleteCoroutine;


    [Title("Runtime Tracking")]
    public bool isWaitingForSurfaceImpact = false;
    public PlaceableSurface targetSurface;
    public IngredientContainer attachedIngredientContainer;



    protected virtual void Awake()
    {
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
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.isKinematic = false;
    }

    public virtual void OnPickUp(Transform grabObjectPoint)
    {
        if (_moveCoroutine != null)
        {
            StopCoroutine(_moveCoroutine);
            _moveCoroutine = null;
        }

        RemoveRigidbodyJoin();
        _grabObjectPoint = grabObjectPoint;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.useGravity = true;
        rb.isKinematic = true;
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
    }

    public void MoveToPlaceableSurface(PlaceableSurface placeableSurface, RaycastHit hit, Quaternion? targetRotation = null)
    {
        _grabObjectPoint = null;
        Vector3 dropPosition = placeableSurface.SnapPoint == null ? hit.point : placeableSurface.SnapPoint.position;
        dropPosition += Vector3.up * dropOffset;
        if (_moveCoroutine != null)
        {
            StopCoroutine(_moveCoroutine);
            _moveCoroutine = null;
        }
        _moveCoroutine = StartCoroutine(MoveToSurfaceCoroutine(dropPosition, placeableSurface, targetRotation));
    }

    public void MoveToPlaceableSurface(PlaceableSurface placeableSurface, Vector3 position, Quaternion? targetRotation = null)
    {
        _grabObjectPoint = null;
        Vector3 dropPosition = position + Vector3.up * dropOffset;
        if (_moveCoroutine != null)
        {
            StopCoroutine(_moveCoroutine);
            _moveCoroutine = null;
        }
        _moveCoroutine = StartCoroutine(MoveToSurfaceCoroutine(dropPosition, placeableSurface, targetRotation));
    }


    public virtual IEnumerator MoveToSurfaceCoroutine(Vector3 dropPosition, PlaceableSurface placeableSurface, Quaternion? targetRotation = null)
    {

        rb.isKinematic = true;
        var itemContainer = GetComponent<IngredientContainer>();

        if (itemContainer != null)
        {
            foreach (var item in itemContainer.containedItems)
            {

                item.ToggleCollider(isTrigger: true);
            }
        }
        ToggleCollider(isTrigger: true);

        float initialDistance = Vector3.Distance(transform.position, dropPosition);
        Quaternion startRotation = transform.rotation;

        while (Vector3.Distance(transform.position, dropPosition) > 0.02f)
        {
            Vector3 newPos = Vector3.MoveTowards(transform.position, dropPosition, followSpeed * Time.fixedDeltaTime);
            rb.MovePosition(newPos);

            if (targetRotation.HasValue && initialDistance > 0f)
            {
                float currentDistance = Vector3.Distance(newPos, dropPosition);
                float progress = 1f - (currentDistance / initialDistance);
                rb.MoveRotation(Quaternion.Slerp(startRotation, targetRotation.Value, progress));
            }

            yield return new WaitForFixedUpdate();
            // Vector3 direction = dropPosition - transform.position;
            // rb.linearVelocity = followSpeed * direction;
            // yield return new WaitForFixedUpdate();
        }

        rb.MovePosition(dropPosition);
        if (targetRotation.HasValue)
        {
            rb.MoveRotation(targetRotation.Value);
        }
        yield return new WaitForFixedUpdate();
        rb.isKinematic = false;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.useGravity = false;
        ToggleCollider(isTrigger: false);
        if (itemContainer != null)
        {
            foreach (var item in itemContainer.containedItems)
            {
                item.rb.linearVelocity = Vector3.zero;
                item.rb.angularVelocity = Vector3.zero;
                item.ToggleCollider(isTrigger: false);
            }
        }
        yield return new WaitForFixedUpdate();
        rb.useGravity = true;
        targetSurface = placeableSurface;
        isWaitingForSurfaceImpact = true;

        yield return new WaitUntil(() => targetSurface == null);
        yield return new WaitForFixedUpdate();

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

        fixedJoint = gameObject.AddComponent<FixedJoint>();
        fixedJoint.connectedBody = other;
        // fixedJoint.enableCollision = true;
        // fixedJoint.enablePreprocessing = true;

    }

    public void RemoveRigidbodyJoin()
    {
        if (fixedJoint != null)
        {
            fixedJoint.connectedBody = null;
            Destroy(fixedJoint);
        }
    }

    // ------------------------------------------------

    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (isWaitingForSurfaceImpact)
        {
            if (collision.gameObject.TryGetComponent<PlaceableSurface>(out var placeableSurface))
            {

                if (placeableSurface.gameObject == targetSurface.gameObject)
                {
                    isWaitingForSurfaceImpact = false;
                    targetSurface = null;

                    if (placeableSurface.ingredientContainer != null)
                    {
                        placeableSurface.ingredientContainer.containedItems.Add(this);
                        attachedIngredientContainer = placeableSurface.ingredientContainer;
                        JoinWithOtherRigidbody(placeableSurface.ingredientContainer.rb);
                    }
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