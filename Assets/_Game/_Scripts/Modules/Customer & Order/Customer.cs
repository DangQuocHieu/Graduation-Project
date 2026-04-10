using System.Collections;
using KinematicCharacterController;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;


public class Customer : MonoBehaviour
{
    [TabGroup("References")] public Animator customerAnim;
    [TabGroup("References")] public CustomerMovement customerMovement;
    [TabGroup("References")] public CustomerState currentState;
    [TabGroup("References")] public CustomerManager customerManager;

    [TabGroup("GUI")] public TakeOrderButton takeOrderButton;

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
        }
    }

    private void ExitState(CustomerState state)
    {
        switch (state)
        {
            case CustomerState.Coming:

                break;
            case CustomerState.Ordering:

                break;
        }
    }

    public void ChangeState(CustomerState state)
    {
        if (currentState == state)
        {
            return;
        }
        ExitState(state);
        currentState = state;
        EnterState(currentState);
    }

    #region Methods for Coming State
    private void EnterComingState()
    {
        customerAnim.SetBool(GameConstant.WALKING, true);
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
        customerAnim.SetBool(GameConstant.WALKING, false);
        takeOrderButton.gameObject.SetActive(true);
    }

    private void UpdateOrderingState()
    {
        if(takeOrderButton.orderAccepted)
        {
            ChangeState(CustomerState.WaitingForFood);
        }
    }

    #endregion

    #region methods for waiting for food state
    private void EnterWaitingForFoodState()
    {
        var availableChair = customerManager.GetAvailableChair();
        customerAnim.SetBool(GameConstant.WALKING, true);
        customerMovement.MoveToPosition(availableChair.transform.position);
        StartCoroutine(WaitForSittingOnChairCoroutine(availableChair));        
    }

    private IEnumerator WaitForSittingOnChairCoroutine(ChairObject chairObject)
    {
        yield return new WaitUntil(()=> customerMovement.HasReachedDestination());
        SittingOnChair(chairObject);
    }
    
    private void SittingOnChair(ChairObject chairObject)
    {
        customerMovement.DisableMovement();
        customerMovement.MoveToPositionImmediately(chairObject.sittingPoint.position);
        customerMovement.RotateToTargetImmediately(Quaternion.Euler(chairObject.sittingRotation));
        customerAnim.Play(GameConstant.SITTING_IDLE);
        
    }

    #endregion
}
