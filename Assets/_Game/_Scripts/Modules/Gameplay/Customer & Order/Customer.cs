using System.Collections;
using KinematicCharacterController;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;


public class Customer : MonoBehaviour
{
    [TabGroup("References")] public CustomerAnimator customerAnim;
    [TabGroup("References")] public CustomerMovement customerMovement;
    [TabGroup("References")] public CustomerState currentState;
    [TabGroup("References")] public CustomerManager customerManager;

    [TabGroup("References")] public CustomerOrderController orderController;
    [TabGroup("References")] public Transform chopstickVisual;
    [TabGroup("References")] public ChairObject attachedChairObject;
    [TabGroup("References")] public Transform paymentVisual;

    [TabGroup("AI Behaviour")] public float currentStateDuration;
    [TabGroup("AI Behaviour")] public float eatingStateDuration;
    [TabGroup("AI Behaviour")] public float stateTimer;

    void Start()
    {
        EnterState(currentState);
    }

    void Update()
    {
        UpdateState(currentState);
    }
    private void EnterState(CustomerState state)
    {
        switch (state)
        {
            case CustomerState.Coming:
                EnterComingState();
                break;
            case CustomerState.Ordering:
                EnterOrderingState();
                break;
            case CustomerState.WaitingForFood:
                EnterWaitingForFoodState();
                break;
            case CustomerState.Eating:
                EnterEatingState();
                break;
            case CustomerState.Paying:
                EnterPayingState();
                break;
            case CustomerState.Leaving:
                EnterLeavingState();
                break;

        }
    }

    private void UpdateState(CustomerState state)
    {
        switch (state)
        {
            case CustomerState.Coming:
                UpdateComingState();
                break;
            case CustomerState.Ordering:
                UpdateOrderingState();
                break;
            case CustomerState.Eating:
                UpdateEatingState();
                break;
        }
    }

    private void ExitState(CustomerState state)
    {
        switch (state)
        {
            case CustomerState.Paying:
                ExitPayingState();
                break;
        }
    }

    public void ChangeState(CustomerState state)
    {
        if (currentState == state)
        {
            return;
        }
        ExitState(currentState);
        currentState = state;
        EnterState(currentState);
    }

    #region Methods for Coming State
    private void EnterComingState()
    {
        customerAnim.SetWalking(true);
        customerMovement.MoveToPosition(customerManager.orderPoint.position);

    }

    private void UpdateComingState()
    {
        if (customerMovement.HasReachedDestination())
        {
            ChangeState(CustomerState.Ordering);
        }
    }

    #endregion

    #region methods for ordering state
    private void EnterOrderingState()
    {
        customerMovement.StartRotating(customerManager.orderPoint.rotation);
        customerAnim.SetWalking(false);
        orderController.takeOrderButton.gameObject.SetActive(true);

    }

    private void UpdateOrderingState()
    {
        if (orderController.orderAccepted)
        {
            ChangeState(CustomerState.WaitingForFood);
        }
    }

    #endregion

    #region methods for waiting for food state
    private void EnterWaitingForFoodState()
    {
        var availableChair = customerManager.GetAvailableChair();
        customerAnim.SetWalking(true);
        customerMovement.MoveToPosition(availableChair.transform.position);
        StartCoroutine(WaitForSittingOnChairCoroutine(availableChair));
    }

    private IEnumerator WaitForSittingOnChairCoroutine(ChairObject chairObject)
    {
        yield return new WaitUntil(() => customerMovement.HasReachedDestination());
        SittingOnChair(chairObject);
    }

    private void SittingOnChair(ChairObject chairObject)
    {
        customerMovement.DisableMovement();
        customerMovement.MoveToPositionImmediately(chairObject.sittingPoint.position);
        customerMovement.RotateToTargetImmediately(Quaternion.Euler(chairObject.sittingRotation));
        customerAnim.SetWalking(false);
        customerAnim.SetSittingIdle();
        attachedChairObject = chairObject;

    }

    private void LeaveChair()
    {
        if (attachedChairObject != null)
        {
            customerMovement.MoveToPositionImmediately(attachedChairObject.leavePoint.position);
            customerMovement.EnableMovement();
            attachedChairObject = null;
        }
    }

    public void HandleFoodServed()
    {
        if (orderController.orderAccepted)
        {
            ChangeState(CustomerState.Eating);
        }
    }
    #endregion

    #region methods for EatingState
    private void EnterEatingState()
    {
        customerAnim.SetEating(true);
        chopstickVisual.gameObject.SetActive(true);
        currentStateDuration = eatingStateDuration;
        stateTimer = 0f;
    }

    private void UpdateEatingState()
    {
        stateTimer += Time.deltaTime;
        if (stateTimer >= currentStateDuration)
        {
            LeaveChair();
            chopstickVisual.gameObject.SetActive(false);
            customerAnim.SetWalking(true);
            customerMovement.MoveToPosition(customerManager.payPoint.position);
            if (customerMovement.HasReachedDestination())
            {
                customerMovement.StartRotating(customerManager.payPoint.rotation);
                ChangeState(CustomerState.Paying);
            }
        }

    }

    #endregion

    #region methods for paying state
    private void EnterPayingState()
    {
        paymentVisual.gameObject.SetActive(true);
        customerAnim.SetPaying(true);
    }

    private void ExitPayingState()
    {
        customerAnim.SetPaying(false);   
    }

    #endregion

    #region methods for leaving state
    private void EnterLeavingState()
    {
        customerAnim.SetWalking(true);
        customerMovement.MoveToPosition(customerManager.GetRandomLeavePoint().position);
    }
    #endregion
}
