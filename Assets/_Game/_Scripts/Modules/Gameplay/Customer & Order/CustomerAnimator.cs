using UnityEngine;

public class CustomerAnimator : MonoBehaviour
{
    
    public const string IDLE = "Idle";
    public const string WALKING = "Walking";
    public const string EATING = "Eating";
    public const string SITTING_IDLE = "Sitting Idle";
    public const string PAYING = "Paying";
    public Animator animator;

    public void SetIdle()
    {
        animator.Play(IDLE);
    }

    public void SetEating(bool isEating)
    {
        animator.SetBool(EATING, isEating);
    }

    public void SetWalking(bool isWalking)
    {
        animator.SetBool(WALKING, isWalking);
    }

    public void SetSittingIdle()
    {
        animator.Play(SITTING_IDLE);
    }

    public void SetPaying(bool isPaying)
    {
        animator.SetBool(PAYING, isPaying);
    }
}
