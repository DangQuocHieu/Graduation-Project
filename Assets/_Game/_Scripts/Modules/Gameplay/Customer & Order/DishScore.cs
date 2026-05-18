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
        if (attachedCustomer.serviceDelayed)
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
        var dish = attachedCustomer.attachedDish;
        int count = 0;
        foreach (var anchors in dish.IngredientAnchorsDic.Values)
        {
            foreach (var anchor in anchors)
            {
                if (anchor.attachedIngredient != null && anchor.attachedIngredient.TryGetComponent<CookableObject>(out var cookableObject))
                {
                    tasteScore += cookableObject.GetTasteScore();
                    ++count;
                }
            }
        }
        if (count > 0)
        {
            tasteScore /= count;
        }
        else
        {
            tasteScore = 0f;
        }

        var sauceType = attachedCustomer.customerOrder.sauceType;
        if(dish.attachedBowl == null || dish.attachedBowl.sauceType != sauceType)
        {
            tasteScore -= 0.2f; //wrong sauce type penalty
        }

        tasteScore = Mathf.Max(0f, tasteScore);

    }

    public void ResetScore()
    {
        waitingScore = 0;
        tasteScore = 0;
        guestPaidAmount = 0;
    }
}
