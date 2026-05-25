using DQHieu.Framework.Audio;
using UnityEngine;

public class CustomerAnimator : MonoBehaviour
{
    public AudioData sfxFootstep;
    private AudioEmitter _footstepEmitter;
    
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
        if (isWalking)
        {
            if (sfxFootstep != null && _footstepEmitter == null)
            {
                _footstepEmitter = AudioManager.Instance.PlaySFX(sfxFootstep, transform);
            }
        }
        else
        {
            if (_footstepEmitter != null)
            {
                AudioManager.Instance.StopSFX(_footstepEmitter, 0.2f);
                _footstepEmitter = null;
            }
        }
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
