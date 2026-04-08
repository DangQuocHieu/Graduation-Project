using DQHieu.Framework;
using UnityEngine;
[System.Serializable]
public class PlayerData
{   
    [SerializeField] private int money;
    
    public int Money 
    {
        get
        {
            return money;
        }
        set
        {
            money = value;
            EventBus.SendMessage<MoneyChangeEvent>(new MoneyChangeEvent());
        }
        
    }
}
