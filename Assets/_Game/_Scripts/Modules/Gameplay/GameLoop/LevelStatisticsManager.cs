using System;
using System.Collections.Generic;
using DQHieu.Framework;
using Sirenix.OdinInspector;
using UnityEngine;

public class LevelStatisticsManager : MonoBehaviour
{
    [ShowInInspector] public int orderAmount => currentLevel.customerArrivalConfigs.Count;
    [ShowInInspector] public int finishedOrder => dishScores.Count;
    public int expense;
    public int mealIncome;

    public int totalWaitingScore;
    public int totalTasteScore;
    public List<DishScore> dishScores = new();

    private LevelData currentLevel;

    void OnEnable()
    {
        EventBus.Subcribe<SpendMoneyEvent>(HandleSpendMoneyEvent);
        EventBus.Subcribe<CustomerPaymentReceived>(HandleCustomerPaymentReceivedEvent);
        EventBus.Subcribe<FoodServedEvent>(HandleFoodServedEvent);
    }

    void OnDisable()
    {
        EventBus.UnSubcribe<SpendMoneyEvent>(HandleSpendMoneyEvent);
        EventBus.UnSubcribe<CustomerPaymentReceived>(HandleCustomerPaymentReceivedEvent);
        EventBus.UnSubcribe<FoodServedEvent>(HandleFoodServedEvent);
    }

    public void Initialize(LevelData currentLevel)
    {
        this.currentLevel = currentLevel;
    }

    private void HandleSpendMoneyEvent(SpendMoneyEvent evt)
    {
        expense += evt.amount;
    }

    private void HandleCustomerPaymentReceivedEvent(CustomerPaymentReceived evt)
    {
        mealIncome += evt.amount;
    }

    public int GetTotalIncome()
    {
        return mealIncome - expense;
    }

    public int GetTotalWaitingScore()
    {
        float waitingScore = 0;
        foreach(var dishScore in dishScores)
        {
            waitingScore += dishScore.waitingScore;
        }
        waitingScore /= dishScores.Count;
        totalWaitingScore = Mathf.RoundToInt(waitingScore);
        return totalWaitingScore;
    }

    public int GetTotalTasteScore()
    {
        float tasteScore = 0;
        foreach(var dishScore in dishScores)
        {
            tasteScore += dishScore.tasteScore;
        }
        tasteScore /= dishScores.Count;
        totalTasteScore = Mathf.RoundToInt(tasteScore);
        return totalTasteScore;
    }

    public int GetOverallScore()
    {
        return Mathf.RoundToInt(((float)totalWaitingScore + (float)totalTasteScore)/ 2); 
    }

    private void HandleFoodServedEvent(FoodServedEvent evt)
    {
        dishScores.Add(evt.customer.dishScore);
    }


}
