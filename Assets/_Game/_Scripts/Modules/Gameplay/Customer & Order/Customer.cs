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
    [TabGroup("AI Behaviour")] public bool serviceDelayed = false;  
    [TabGroup("AI Behaviour")] public bool foodServed = false;
    [TabGroup("AI Behaviour")] public BambooTray attachedDish;

    void Awake()
    {
        dishScore.attachedCustomer = this;
    }
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
            case CustomerState.ServiceDelayed:
                EnterServiceDelayedState();
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
            case CustomerState.WaitingForFood:
                UpdateWaitingForFoodState();
                break;
            case CustomerState.Eating:
                UpdateEatingState();
                break;
            case CustomerState.ServiceDelayed:
                UpdateServiceDelayedState();
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
            case CustomerState.WaitingForFood:
                ExitWaitingForFoodState();
                break;
            case CustomerState.ServiceDelayed:
                ExitServiceDelayedState();
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
        EventBus.Raise<CustomerOrderComplete>(new CustomerOrderComplete(this));
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
        availableChair.isEmpty = false;
        customerAnim.SetWalking(true);
        customerMovement.MoveToPosition(availableChair.transform.position);
        stateTimer = customerOrder.serveDuration;
        StartCoroutine(WaitForSittingOnChairCoroutine(availableChair));
    }

    private void UpdateWaitingForFoodState()
    {
        stateTimer -= Time.deltaTime;
        if(stateTimer <= 0f)
        {
            ChangeState(CustomerState.ServiceDelayed);
        }
    }

    private void ExitWaitingForFoodState()
    {
        stateTimer = 0f;
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
        foodServed = true;
        chopstickVisual.attachedDish = dish;
        attachedDish = dish;
        //Calculate Score Here
        dishScore.CalculateScore();
        EventBus.Raise<FoodServedEvent>(new FoodServedEvent(this));
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
        if(attachedChairObject != null)
        {
            LeaveChair();
        }
        customerMovement.EnableMovement();
        EventBus.Raise<CustomerLeaveEvent>(new CustomerLeaveEvent(this));
        customerAnim.SetWalking(true);
        customerMovement.MoveToPosition(orderManager.GetRandomLeavePoint().position);
    }
    #endregion

    #region  methods for service delayed state

    private void EnterServiceDelayedState()
    {
        serviceDelayed = true;
        stateTimer = customerOrder.gracePeriodDuration;
    }

    private void UpdateServiceDelayedState()
    {
        stateTimer -= Time.deltaTime;
        if(stateTimer <= 0f)
        {
            ChangeState(CustomerState.Leaving);
        }
    }

    private void ExitServiceDelayedState()
    {
        
    }
    #endregion
}
