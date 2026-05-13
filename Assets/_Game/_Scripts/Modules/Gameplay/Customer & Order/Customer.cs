using System.Collections;
using System.Collections.Generic;
using DQHieu.Framework;
using KinematicCharacterController;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;


public class Customer : MonoBehaviour
{
    [TabGroup("References")] public CustomerAnimator customerAnim;
    [TabGroup("References")] public CustomerMovement customerMovement;
    [TabGroup("References")] public CustomerState currentState;
    [TabGroup("References")] public OrderManager orderManager;

    [TabGroup("References")] public CustomerChopstickVisual chopstickVisual;
    [TabGroup("References")] public ChairObject attachedChairObject;
    [TabGroup("References")] public Transform paymentVisual;

    [TabGroup("AI Behaviour")] public CustomerSO customerSO;
    [TabGroup("AI Behaviour")] public float currentStateDuration;
    [TabGroup("AI Behaviour")] public float eatingStateDuration;
    [TabGroup("AI Behaviour")] public float stateTimer;
    [TabGroup("AI Behaviour")] public CustomerOrder customerOrder;
    [TabGroup("AI Behaviour")] public DishScore dishScore;
    [TabGroup("AI Behaviour")] public bool isLastCustomer;


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
            case CustomerState.Eating:
                ExitEatingState();
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
        customerMovement.MoveToPosition(orderManager.orderPoint.position);

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
        stateTimer = 0f;
        customerMovement.StartRotating(orderManager.orderPoint.rotation);
        customerAnim.SetWalking(false);
        EventBus.SendMessage<CustomerOrderComplete>(new CustomerOrderComplete(customerOrder));
    }

    private void UpdateOrderingState()
    {
        stateTimer += Time.deltaTime;
        if (stateTimer > 2f)
        {
            ChangeState(CustomerState.WaitingForFood);
        }
    }


    #endregion

    #region methods for waiting for food state
    private void EnterWaitingForFoodState()
    {
        var availableChair = orderManager.GetAvailableChair();
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

    public void HandleFoodServed(BambooTray dish)
    {
        chopstickVisual.attachedDish = dish;
        //Calculate Score Here
        dishScore.CalculateScore();
        EventBus.SendMessage<FoodServedEvent>(new FoodServedEvent(dishScore));
        ChangeState(CustomerState.Eating);
    }
    #endregion

    #region methods for EatingState
    private void EnterEatingState()
    {
        customerAnim.SetEating(true);
        chopstickVisual.visualObject.gameObject.SetActive(true);
        currentStateDuration = eatingStateDuration;
        stateTimer = 0f;
    }

    private void UpdateEatingState()
    {
        if (stateTimer >= currentStateDuration)
        {
            LeaveChair();
            chopstickVisual.visualObject.gameObject.SetActive(false);
            customerAnim.SetWalking(true);
            customerMovement.MoveToPosition(orderManager.payPoint.position);
            if (chopstickVisual.attachedDish != null)
            {
                chopstickVisual.attachedDish.ReleseDish();
            }
            StartCoroutine(WaitForReachPayPointCoroutine());
        }
        else
        {
            stateTimer += Time.deltaTime;
        }
    }

    private void ExitEatingState()
    {
    }

    private IEnumerator WaitForReachPayPointCoroutine()
    {
        yield return new WaitUntil(() => customerMovement.HasReachedDestination());
        customerMovement.StartRotating(orderManager.payPoint.rotation);
        ChangeState(CustomerState.Paying);
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
        customerMovement.MoveToPosition(orderManager.GetRandomLeavePoint().position);
    }
    #endregion
}
