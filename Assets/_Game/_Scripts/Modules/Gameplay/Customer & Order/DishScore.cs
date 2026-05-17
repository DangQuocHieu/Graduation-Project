using UnityEngine;

public class DishScore : MonoBehaviour
{
    public Customer attachedCustomer;
    public float waitingScore;
    public float tasteScore;
    public int guestPaidAmount;

    public int displayWatingScore => Mathf.RoundToInt(waitingScore);
    public int displayTasteScore => Mathf.RoundToInt(tasteScore);
    public int GetTotalScore()
    {
        float totalScore = (waitingScore + tasteScore) / 2;
        return Mathf.RoundToInt(totalScore);
    }

    public void CalculateScore()
    {
        CalculateWaitingScore();
        CalculateTasteScore();
    }

    private void CalculateWaitingScore()
    {
        if(attachedCustomer.serviceDelayed)
        {
            waitingScore = (attachedCustomer.stateTimer / attachedCustomer.customerOrder.gracePeriodDuration) * 100f;
        }
        else
        {
            waitingScore = 100f;
        }
    }

    private void CalculateTasteScore()
    {
        
    }

    public void ResetScore()
    {
        waitingScore = 0;
        tasteScore = 0;
        guestPaidAmount = 0;
    }
}
