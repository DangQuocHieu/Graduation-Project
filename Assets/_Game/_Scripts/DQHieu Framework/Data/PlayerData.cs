using DQHieu.Framework;
using UnityEngine;
[System.Serializable]
public class PlayerData
{   
    [SerializeField] private int money;
    [SerializeField] private int currentLevelIndex;
    public const int maxLevel = 3;
    
    public int Money 
    {
        get
        {
            return money;
        }
        set
        {
            money = value;
            EventBus.Raise<MoneyChangeEvent>(new MoneyChangeEvent(money));
        }
        
    }

    public int CurrentLevelIndex
    {
        get
        {
            return currentLevelIndex;
        }
        set
        {
            currentLevelIndex = Mathf.Min(value, maxLevel - 1);
        }
    }
}
