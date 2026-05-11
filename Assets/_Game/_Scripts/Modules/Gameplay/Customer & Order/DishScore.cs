using UnityEngine;

public class DishScore : MonoBehaviour
{
    public float waitingScore;
    public float portionScore;
    public float tasteScore;
    public int guestPaidAmount;


    public int displayWatingScore => Mathf.RoundToInt(waitingScore);
    public int displayPortionScore => Mathf.RoundToInt(portionScore);
    public int displayTasteScore => Mathf.RoundToInt(tasteScore);
    public int GetTotalScore()
    {
        float totalScore = (waitingScore + portionScore + tasteScore) / 3;
        return Mathf.RoundToInt(totalScore);
    }
}
