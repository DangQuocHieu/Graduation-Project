using UnityEngine;

public class DishScore : MonoBehaviour
{
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
        
    }
}
